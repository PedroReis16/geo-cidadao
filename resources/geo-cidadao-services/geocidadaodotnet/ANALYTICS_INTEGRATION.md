# Integração do Serviço de Analytics

## Visão Geral da Integração

Este documento descreve como o serviço de Analytics se integra com o GeoCidadao.GerenciamentoPostsAPI para fornecer dados analíticos sobre problemas reportados.

## Fluxo de Integração

### 1. Criação de Post com Localização

Quando um usuário cria um post através do GerenciamentoPostsAPI e fornece coordenadas de localização:

```
POST /posts
{
  "content": "Descrição do problema",
  "position": {
    "latitude": "-23.5505",
    "longitude": "-46.6333"
  }
}
```

### 2. Processamento no GerenciamentoPostsAPI

O `PostService` realiza as seguintes ações:

1. **Salva o post** no banco de dados
2. **Salva a localização** na tabela `post_location`
3. **Publica evento de analytics** de forma assíncrona

```csharp
// Em PostService.CreatePostAsync
await _postLocationDao.AddAsync(postLocation);

// Notificar analytics service asynchronously
_ = Task.Run(async () => 
{
    using var scope = _scopeFactory.CreateScope();
    var analyticsService = scope.ServiceProvider.GetRequiredService<INotifyPostAnalyticsService>();
    await analyticsService.NotifyPostAnalyticsAsync(postId);
});
```

### 3. Publicação do Evento

O `NotifyPostAnalyticsService`:

1. **Busca dados do post** e sua localização
2. **Constrói a mensagem** `PostAnalyticsMessage` com todos os dados necessários
3. **Publica para RabbitMQ** no exchange `posts_management_topic_exchange` com routing key `post.analytics`

```csharp
var analyticsMessage = new PostAnalyticsMessage
{
    PostId = post.Id,
    Title = post.Content.Length > 100 ? post.Content.Substring(0, 100) : post.Content,
    Description = post.Content,
    Category = post.Category,
    Latitude = location.Position.Y,
    Longitude = location.Position.X,
    EventTimestamp = DateTime.UtcNow,
    LikesCount = post.LikesCount,
    CommentsCount = post.CommentsCount,
    RelevanceScore = post.RelevanceScore
};
```

### 4. Consumo pelo AnalyticsServiceAPI

O `PostAnalyticsConsumerService` (Background Service):

1. **Consome mensagens** da fila `analytics_service_post_analytics_queue`
2. **Deserializa** a mensagem `PostAnalyticsMessage`
3. **Processa o evento** através do `AnalyticsProcessingService`

### 5. Processamento do Evento

O `AnalyticsProcessingService`:

1. **Verifica se o evento já existe** (busca por `PostId`)
2. **Cria novo registro** se não existir
3. **Atualiza registro existente** se já existir (para refletir mudanças em likes, comments, relevance)

```csharp
if (existingEvent != null)
{
    // Atualizar métricas
    existingEvent.LikesCount = message.LikesCount;
    existingEvent.CommentsCount = message.CommentsCount;
    existingEvent.RelevanceScore = message.RelevanceScore;
    await _problemEventDao.UpdateAsync(existingEvent);
}
else
{
    // Criar novo evento
    var problemEvent = new ProblemEvent { ... };
    await _problemEventDao.AddAsync(problemEvent);
}
```

### 6. Disponibilização via API

Os dados processados ficam disponíveis através dos endpoints:

- `GET /analytics/regions/summary` - Resumo por região
- `GET /analytics/top-problems` - Problemas mais relevantes
- `GET /analytics/hotspots` - Cidades com mais problemas

## Componentes de Integração

### GerenciamentoPostsAPI

**Serviços:**
- `INotifyPostAnalyticsService` / `NotifyPostAnalyticsService`

**Configuração em Program.cs:**
```csharp
builder.Services.AddSingleton<INotifyPostAnalyticsService, NotifyPostAnalyticsService>();
```

**Modificações em PostService:**
- Adicionado código para publicar evento após salvar localização

### GeoCidadao.AMQP (Biblioteca Compartilhada)

**Mensagens:**
- `PostAnalyticsMessage` - Mensagem enviada do GerenciamentoPostsAPI para o AnalyticsServiceAPI

**Configurações:**
- `RoutingKeyNames.POST_ANALYTICS_ROUTING_KEY = "post.analytics"`
- `RoutingKeyNames.DLQ_POST_ANALYTICS_ROUTING_KEY = "post.analytics.dlq"`
- `QueueNames.ANALYTICS_SERVICE_POST_ANALYTICS_QUEUE_NAME = "analytics_service_post_analytics_queue"`
- `QueueNames.DLQ_ANALYTICS_SERVICE_POST_ANALYTICS_QUEUE_NAME = "analytics_service_post_analytics_dlq"`

### AnalyticsServiceAPI

**Background Services:**
- `PostAnalyticsConsumerService` - Consome mensagens do RabbitMQ

**Serviços:**
- `IAnalyticsProcessingService` / `AnalyticsProcessingService` - Processa eventos
- `IAnalyticsService` / `AnalyticsService` - Fornece dados analíticos

**DAOs:**
- `IProblemEventDao` / `ProblemEventDao` - Acesso aos dados de eventos

**Controllers:**
- `AnalyticsController` - Endpoints REST

**Configuração em Program.cs:**
```csharp
builder.Services.AddTransient<IAnalyticsProcessingService, AnalyticsProcessingService>();
builder.Services.AddTransient<IAnalyticsService, AnalyticsService>();
builder.Services.AddTransient<IProblemEventDao, ProblemEventDao>();
builder.Services.AddHostedService<PostAnalyticsConsumerService>();
```

### GeoCidadao.Database (Biblioteca Compartilhada)

**Entidades:**
- `ProblemEvent` - Armazena eventos de analytics

**Configurações:**
- `ProblemEventConfiguration` - Mapeamento EF Core

**DbContext:**
- `DbSet<ProblemEvent> ProblemEvents` adicionado ao `GeoDbContext`

**Migrations:**
- `20251116190000_AddAnalyticsEntities` - Cria tabela `problem_event` e índices

## Tratamento de Erros e Retry

### Estratégia de Retry

1. **Primeira tentativa:** Processamento normal
2. **Falha:** Mensagem é rejeitada (BasicNack) e volta para a fila
3. **Segunda tentativa:** RabbitMQ reenvia a mensagem
4. **Falha:** Mensagem é rejeitada novamente
5. **Terceira tentativa:** RabbitMQ reenvia a mensagem pela última vez
6. **Falha:** Mensagem é enviada para a Dead Letter Queue (DLQ)

### Dead Letter Queue (DLQ)

Mensagens que falharam após 3 tentativas são movidas para:
- **DLQ Queue:** `analytics_service_post_analytics_dlq`
- **DLQ Exchange:** `posts_management_topic_exchange_dlq`
- **DLQ Routing Key:** `post.analytics.dlq`

### Monitoramento

Para monitorar falhas:
1. Acesse RabbitMQ Management UI
2. Verifique a queue `analytics_service_post_analytics_dlq`
3. Inspecione mensagens na DLQ para identificar problemas

### Logs

Ambos os serviços registram logs detalhados:

**GerenciamentoPostsAPI:**
```
[Info] Analytics event published for post {postId}
[Warning] Falha ao notificar analytics para o post {postId}
```

**AnalyticsServiceAPI:**
```
[Info] Processing analytics event for post {postId}
[Info] Created new analytics event for post {postId}
[Info] Updated analytics event for post {postId}
[Error] Error processing analytics event for post {postId}: {error}
```

## Segurança

### Isolamento de Serviços

- Cada serviço tem seu próprio contexto de segurança
- O AnalyticsServiceAPI não depende de autenticação do GerenciamentoPostsAPI
- Comunicação via RabbitMQ é assíncrona e desacoplada

### Autenticação e Autorização

**GerenciamentoPostsAPI:**
- Qualquer usuário autenticado pode criar posts
- Posts com localização disparam evento de analytics automaticamente

**AnalyticsServiceAPI:**
- Apenas usuários com role `Analytics.Read` podem consultar dados
- Típicamente: gestores e moderadores

## Considerações de Performance

### Publicação Assíncrona

O evento de analytics é publicado de forma assíncrona (`Task.Run`) para não bloquear a criação do post:

```csharp
_ = Task.Run(async () => 
{
    // Publicação assíncrona
    await analyticsService.NotifyPostAnalyticsAsync(postId);
});
```

**Vantagens:**
- Não impacta tempo de resposta da criação do post
- Falhas na publicação não impedem criação do post
- Melhor experiência do usuário

### Processamento em Background

O consumo de mensagens ocorre em um `BackgroundService`:
- Executa continuamente enquanto o serviço está rodando
- Processa mensagens conforme chegam na fila
- Não bloqueia requests HTTP

### Escalabilidade

**Horizontal:**
- Múltiplas instâncias do AnalyticsServiceAPI podem consumir da mesma fila
- RabbitMQ distribui mensagens entre os consumidores (load balancing)

**Vertical:**
- Índices no banco de dados otimizam queries
- Campos City, State, Category, EventTimestamp indexados
- Queries utilizam filtros apropriados

## Testes de Integração

### Teste Manual

1. **Iniciar serviços:**
   ```bash
   # Terminal 1 - RabbitMQ
   docker run -d -p 5672:5672 -p 15672:15672 rabbitmq:3-management
   
   # Terminal 2 - GerenciamentoPostsAPI
   cd GerenciamentoPostsAPI
   dotnet run
   
   # Terminal 3 - AnalyticsServiceAPI
   cd AnalyticsServiceAPI
   dotnet run
   ```

2. **Criar post com localização:**
   ```bash
   curl -X POST http://localhost:5001/gerenciamento-posts/posts \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer {token}" \
     -d '{
       "content": "Buraco grande na Av. Paulista",
       "position": {
         "latitude": "-23.5505",
         "longitude": "-46.6333"
       }
     }'
   ```

3. **Verificar logs:**
   - GerenciamentoPostsAPI deve mostrar: "Analytics event published for post..."
   - AnalyticsServiceAPI deve mostrar: "Created new analytics event for post..."

4. **Consultar analytics:**
   ```bash
   curl http://localhost:5002/analytics-service/analytics/top-problems?limit=10 \
     -H "Authorization: Bearer {token}"
   ```

### Verificar RabbitMQ

1. Acessar http://localhost:15672 (user: guest, password: guest)
2. Ir para Queues → `analytics_service_post_analytics_queue`
3. Verificar mensagens sendo consumidas

## Troubleshooting

### Problema: Eventos não estão sendo publicados

**Verificar:**
1. RabbitMQ está rodando?
2. Configuração `RabbitMQ` em appsettings.json está correta?
3. Post foi criado com localização válida?
4. Logs do GerenciamentoPostsAPI mostram algum erro?

### Problema: Eventos não estão sendo consumidos

**Verificar:**
1. AnalyticsServiceAPI está rodando?
2. Background service iniciou corretamente?
3. Há mensagens na fila? (RabbitMQ Management UI)
4. Há mensagens na DLQ?
5. Logs do AnalyticsServiceAPI mostram algum erro?

### Problema: Dados não aparecem nos endpoints

**Verificar:**
1. Eventos foram processados com sucesso?
2. Banco de dados contém registros em `problem_event`?
3. Filtros na query estão corretos?
4. Usuário tem permissão `Analytics.Read`?

```sql
-- Verificar eventos no banco
SELECT * FROM problem_event ORDER BY created_at DESC LIMIT 10;
```

## Manutenção

### Limpeza de Dados Antigos

Considere implementar rotina para arquivar ou remover eventos antigos:

```sql
-- Exemplo: arquivar eventos com mais de 1 ano
DELETE FROM problem_event 
WHERE event_timestamp < NOW() - INTERVAL '1 year';
```

### Monitoramento de Filas

Configure alertas para:
- Fila principal crescendo sem parar (consumidor parado?)
- DLQ com muitas mensagens (problemas recorrentes?)
- Tempo de processamento elevado

### Backup

Garanta backup regular da tabela `problem_event`:
- Dados críticos para análises históricas
- Não pode ser recriado facilmente se perdido
