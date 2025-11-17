# Analytics Service API

## Visão Geral

O Analytics Service API é responsável por coletar, processar e disponibilizar dados analíticos sobre os problemas reportados na plataforma GeoCidadao. Este serviço permite que gestores e moderadores acessem informações consolidadas sobre:

- Problemas mais relevantes por região
- Regiões com maior concentração de problemas (hotspots)
- Tipos de problemas mais frequentes
- Tendências temporais de problemas

## Arquitetura

### Fluxo de Dados

```
Post com Localização Criado
    ↓
GerenciamentoPostsAPI publica evento (PostAnalyticsMessage)
    ↓
RabbitMQ (posts_management_topic_exchange)
    ↓
AnalyticsServiceAPI consome evento
    ↓
Armazena ProblemEvent no banco de dados
    ↓
Dados disponíveis via endpoints REST
```

### Componentes Principais

1. **PostAnalyticsConsumerService** (Background Service)
   - Consome mensagens da fila RabbitMQ
   - Processa eventos de analytics de forma assíncrona
   - Implementa retry automático via Dead Letter Queue (DLQ)

2. **AnalyticsProcessingService**
   - Processa eventos de analytics
   - Cria ou atualiza registros de ProblemEvent
   - Mantém métricas atualizadas (likes, comments, relevance score)

3. **AnalyticsService**
   - Fornece métodos de consulta para dados analíticos
   - Agrega dados por região, categoria e período

4. **AnalyticsController**
   - Expõe endpoints REST para acesso aos dados
   - Implementa autorização baseada em roles

## Endpoints

### GET /analytics/regions/summary

Retorna um resumo de problemas para uma região específica.

**Parâmetros de Query:**
- `city` (opcional): Nome da cidade
- `state` (opcional): Nome do estado

**Resposta:**
```json
{
  "city": "São Paulo",
  "state": "SP",
  "totalProblems": 150,
  "problemsByCategory": {
    "Infrastructure": 45,
    "Safety": 30,
    "Health": 25,
    "Environment": 20,
    "Education": 15,
    "Transportation": 15
  },
  "mostRelevantProblems": [...]
}
```

**Autorização:** Requer role `Analytics.Read`

### GET /analytics/top-problems

Retorna os problemas mais relevantes filtrados por região, categoria e período.

**Parâmetros de Query:**
- `city` (opcional): Nome da cidade
- `state` (opcional): Nome do estado
- `category` (opcional): Categoria do problema (Infrastructure, Safety, Health, etc.)
- `startDate` (opcional): Data inicial do período (ISO 8601)
- `endDate` (opcional): Data final do período (ISO 8601)
- `limit` (opcional, padrão: 10): Número máximo de resultados

**Resposta:**
```json
[
  {
    "id": "guid",
    "postId": "guid",
    "title": "Título do problema",
    "description": "Descrição completa",
    "category": "Infrastructure",
    "city": "São Paulo",
    "state": "SP",
    "latitude": -23.5505,
    "longitude": -46.6333,
    "eventTimestamp": "2025-11-16T19:00:00Z",
    "likesCount": 42,
    "commentsCount": 15,
    "relevanceScore": 85.5
  }
]
```

**Autorização:** Requer role `Analytics.Read`

### GET /analytics/hotspots

Retorna as cidades com maior concentração de problemas (mapa de calor).

**Parâmetros de Query:**
- `state` (opcional): Nome do estado
- `limit` (opcional, padrão: 20): Número máximo de resultados

**Resposta:**
```json
[
  {
    "city": "São Paulo",
    "problemCount": 150
  },
  {
    "city": "Rio de Janeiro",
    "problemCount": 120
  }
]
```

**Autorização:** Requer role `Analytics.Read`

## Modelo de Dados

### ProblemEvent

Armazena informações sobre cada problema reportado com localização.

**Campos:**
- `Id`: Identificador único do evento
- `PostId`: ID do post original
- `Title`: Título extraído do conteúdo (primeiros 100 caracteres)
- `Description`: Conteúdo completo do post
- `Category`: Categoria do problema
- `Region`: Região (se disponível)
- `City`: Cidade
- `State`: Estado
- `Latitude`: Coordenada de latitude
- `Longitude`: Coordenada de longitude
- `EventTimestamp`: Data/hora do evento
- `LikesCount`: Número de curtidas
- `CommentsCount`: Número de comentários
- `RelevanceScore`: Pontuação de relevância
- `CreatedAt`: Data de criação do registro
- `UpdatedAt`: Data da última atualização

**Índices:**
- PostId (para busca rápida por post)
- Category (para filtros por categoria)
- EventTimestamp (para filtros temporais)
- City, State (para filtros geográficos)
- RelevanceScore (para ordenação por relevância)

## Mensageria

### PostAnalyticsMessage

Mensagem publicada pelo GerenciamentoPostsAPI quando um post com localização é criado.

**Estrutura:**
```json
{
  "postId": "guid",
  "title": "string",
  "description": "string",
  "category": "PostCategory",
  "region": "string",
  "city": "string",
  "state": "string",
  "latitude": 0.0,
  "longitude": 0.0,
  "eventTimestamp": "datetime",
  "likesCount": 0,
  "commentsCount": 0,
  "relevanceScore": 0.0
}
```

### Configuração RabbitMQ

**Exchange:** `posts_management_topic_exchange` (Topic)
**Routing Key:** `post.analytics`
**Queue:** `analytics_service_post_analytics_queue`
**DLQ Exchange:** `posts_management_topic_exchange_dlq`
**DLQ Routing Key:** `post.analytics.dlq`
**DLQ Queue:** `analytics_service_post_analytics_dlq`
**Delivery Limit:** 3 tentativas

## Segurança

### Controle de Acesso

Apenas usuários com as seguintes roles podem acessar os endpoints de analytics:

- **Analytics.Read**: Permissão para leitura de dados analíticos
  - Mapeado para: `res:geocidadao-analytics-api:analytics:read` no Keycloak

### Grupos Autorizados

Os seguintes grupos têm acesso aos dados de analytics:
- **Managers** (`/managers`)
- **Moderators** (`/moderators`)

Usuários comuns (`/users`) **não** têm acesso aos endpoints de analytics.

## Configuração

### appsettings.json

```json
{
  "BasePath": "analytics-service",
  "ConnectionStrings": {
    "GeoDb": "Host=localhost;Database=geodb;Username=geo;Password=geo"
  },
  "OAuth": {
    "Authority": "http://localhost:8082/realms/geocidadao",
    "Audience": "geocidadao-analytics-api",
    "ClaimRoles": {
      "Analytics.Read": "res:geocidadao-analytics-api:analytics:read"
    }
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Username": "admin",
    "Password": "admin"
  }
}
```

## Executando o Serviço

### Pré-requisitos

1. PostgreSQL com extensão PostGIS
2. RabbitMQ
3. Keycloak configurado com o realm `geocidadao`

### Migrações de Banco de Dados

O serviço utiliza o mesmo banco de dados do GeoCidadao. Execute as migrações:

```bash
cd Libraries/GeoCidadao.Database
dotnet ef database update
```

### Executar o Serviço

```bash
cd GeoCidadao.AnalyticsServiceAPI
dotnet run
```

O serviço estará disponível em:
- **HTTP:** http://localhost:5000/analytics-service
- **Swagger:** http://localhost:5000/analytics-service/swagger

## Testes

### Testando o Fluxo Completo

1. **Criar um post com localização** via GerenciamentoPostsAPI
   ```bash
   POST /posts
   {
     "content": "Buraco na rua principal",
     "position": {
       "latitude": "-23.5505",
       "longitude": "-46.6333"
     }
   }
   ```

2. **Verificar que o evento foi publicado** - Checar logs do GerenciamentoPostsAPI

3. **Verificar que o evento foi consumido** - Checar logs do AnalyticsServiceAPI

4. **Consultar analytics**
   ```bash
   GET /analytics/top-problems?city=São Paulo&state=SP
   ```

### Verificando a Fila RabbitMQ

Acesse o RabbitMQ Management (http://localhost:15672):
- Verifique a queue `analytics_service_post_analytics_queue`
- Monitore a DLQ `analytics_service_post_analytics_dlq` para eventos com falha

## Otimizações

### Performance

- Índices criados em campos comuns de filtro (city, state, category, timestamp)
- Queries otimizadas com filtros apropriados
- Uso de paginação (limit) para controlar volume de dados retornados

### Escalabilidade

- Background service processa mensagens de forma assíncrona
- Dead Letter Queue garante que mensagens com falha não sejam perdidas
- Suporta múltiplas instâncias do serviço (consumer groups)

## Próximos Passos

1. **Cache**: Implementar cache para consultas frequentes
2. **Agregações em tempo real**: Adicionar métricas pré-calculadas
3. **Dashboards**: Criar visualizações gráficas para gestores
4. **Alertas**: Notificar gestores sobre hotspots críticos
5. **Machine Learning**: Identificar padrões e prever tendências
