# Sistema de Carregamento Dinâmico do Mapa

## 📋 Visão Geral

O sistema de mapa foi aprimorado com **carregamento dinâmico de posts** baseado na área visível (bounds) e nível de zoom. Isso garante:

- ✅ **Carregamento sob demanda**: Posts são carregados conforme necessário
- ✅ **Otimização de memória**: Cache inteligente e limpeza automática
- ✅ **Performance**: Debounce e cancelamento de requisições
- ✅ **Escalabilidade**: Suporta milhares de posts sem degradação

---

## 🏗️ Arquitetura

### Componentes Principais

#### 1. **MapService** (`src/data/services/mapService.ts`)
Serviço responsável por comunicação com o backend.

**Funcionalidades:**
- Busca posts por bounds geográficos e zoom
- Sistema de cache com TTL de 5 minutos
- Limpeza automática de cache expirado
- Cálculo de limites dinâmicos baseado no zoom

**API:**
```typescript
interface MapPostsQuery {
  bounds: MapBounds;
  zoom: number;
  limit?: number;
}

mapService.getPostsInBounds(query) -> Promise<MapPostsResponse>
mapService.clearCache() -> void
mapService.getCacheStats() -> { total, valid, expired }
```

**Endpoint esperado:**
```
GET /feed-map/map/posts?north={lat}&south={lat}&east={lng}&west={lng}&zoom={level}&limit={count}
```

#### 2. **useMapPosts Hook** (`src/data/hooks/useMapPosts.ts`)
Hook React customizado para gerenciar carregamento dinâmico.

**Funcionalidades:**
- Carregamento automático baseado em center/zoom
- Debounce de 500ms para evitar requisições excessivas
- Cancelamento de requisições pendentes
- Cleanup automático de memória

**Uso:**
```typescript
const {
  posts,        // Posts carregados
  loading,      // Estado de carregamento
  error,        // Mensagem de erro
  totalCount,   // Total de posts disponíveis
  refresh,      // Força atualização
  clearCache,   // Limpa cache e atualiza
} = useMapPosts({
  center: { lat, lng },
  zoom: 12,
  enabled: true,        // Habilita carregamento
  debounceMs: 500,      // Tempo de debounce
});
```

#### 3. **MapComponent** (atualizado)
Componente de mapa com suporte a carregamento dinâmico.

**Nova prop:**
```typescript
enableDynamicLoading?: boolean  // Ativa carregamento do backend
```

**Comportamento:**
- `false` (padrão): Usa prop `items` (modo estático)
- `true`: Carrega posts dinamicamente do backend

---

## 🔄 Fluxo de Carregamento

```
Usuário move/zoom no mapa
  ↓
MapComponent atualiza center/zoom
  ↓
useMapPosts detecta mudança (com debounce)
  ↓
Calcula bounds da área visível
  ↓
Verifica cache
  ├─ HIT: Retorna dados em cache
  └─ MISS: Faz requisição ao backend
      ↓
      Backend retorna posts filtrados
      ↓
      Armazena no cache
      ↓
      Atualiza UI com novos posts
```

---

## 🎯 Otimizações Implementadas

### 1. **Sistema de Cache**
- Cache baseado em bounds + zoom
- TTL de 5 minutos
- Limpeza automática de entradas expiradas
- Coordenadas arredondadas para reduzir variações mínimas

### 2. **Debounce**
- 500ms de delay antes de fazer requisição
- Evita requisições enquanto usuário arrasta o mapa
- Cancelamento de timers anteriores

### 3. **Cancelamento de Requisições**
- Usa `AbortController` para cancelar fetch em andamento
- Previne race conditions
- Evita atualização com dados obsoletos

### 4. **Limite Dinâmico**
Ajusta quantidade de posts baseado no zoom:

| Zoom | Limite de Posts |
|------|----------------|
| < 10 | 50            |
| 10-11| 100           |
| 12-13| 200           |
| 14-15| 500           |
| >= 16| 1000          |

### 5. **Cleanup de Memória**
- Limpa posts ao desmontar componente
- Cancela requisições pendentes
- Remove timers ativos

---

## 🔌 Integração com Backend

### Endpoint Esperado

**URL:** `GET /feed-map/map/posts`

**Query Parameters:**
```
north: number    // Latitude norte do bounds
south: number    // Latitude sul do bounds
east: number     // Longitude leste do bounds
west: number     // Longitude oeste do bounds
zoom: number     // Nível de zoom atual
limit: number    // Máximo de posts a retornar
```

**Response:**
```typescript
{
  posts: Post[],           // Array de posts
  totalCount: number,      // Total de posts disponíveis
  bounds: MapBounds        // Bounds usados na query
}
```

### Exemplo de Implementação (.NET)

```csharp
[HttpGet("posts")]
public async Task<IActionResult> GetPostsInBounds(
    [FromQuery] double north,
    [FromQuery] double south,
    [FromQuery] double east,
    [FromQuery] double west,
    [FromQuery] int zoom,
    [FromQuery] int limit = 100)
{
    var posts = await _mapService.GetPostsInBoundsAsync(
        new BoundsQuery 
        { 
            North = north, 
            South = south, 
            East = east, 
            West = west,
            Zoom = zoom,
            Limit = limit
        }
    );

    return Ok(new 
    {
        Posts = posts,
        TotalCount = posts.Count,
        Bounds = new { north, south, east, west }
    });
}
```

### Query Espacial (PostGIS/SQL Server)

```sql
SELECT * FROM Posts
WHERE Location.STIntersects(
    geography::STGeomFromText(
        'POLYGON(({west} {north}, {east} {north}, 
                  {east} {south}, {west} {south}, 
                  {west} {north}))', 4326
    )
) = 1
ORDER BY CreatedAt DESC
LIMIT @Limit
```

---

## 🎨 UI/UX

### Indicadores Visuais

1. **Loading Indicator**
   - Aparece no topo quando carregando
   - Ícone animado de refresh
   - Auto-hide quando completo

2. **Botão de Refresh**
   - Nos controles de zoom
   - Força atualização manual
   - Mostra spinner durante loading

3. **Contador de Pontos**
   - Exibe quantidade de posts visíveis
   - Atualiza em tempo real

4. **Overlay Minimizado**
   - Mostra preview da quantidade de pontos
   - Indica estado do mapa

---

## 🧪 Testes e Debug

### Logs Console

O sistema emite logs detalhados:

```javascript
[MapService] Buscando posts: { bounds, zoom, limit }
[MapService] Retornando do cache: {key}
[useMapPosts] Carregados X posts (total: Y)
[useMapPosts] Query duplicada ignorada
[useMapPosts] Cleanup executado
```

### Stats de Cache

```typescript
const stats = mapService.getCacheStats();
console.log(stats); 
// { total: 10, valid: 8, expired: 2 }
```

### Forçar Atualização

```typescript
// No componente
const { refresh, clearCache } = useMapPosts(...);

refresh();      // Recarrega com mesmos parâmetros
clearCache();   // Limpa cache e recarrega
```

---

## 📊 Performance

### Métricas Esperadas

- **Tempo de resposta**: < 500ms
- **Cache hit rate**: > 70% em uso normal
- **Memória**: ~5MB para 1000 posts
- **Requisições**: ~2-3 por minuto em navegação ativa

### Monitoramento

```typescript
// Verificar performance
performance.mark('map-load-start');
// ... carregamento ...
performance.mark('map-load-end');
performance.measure('map-load', 'map-load-start', 'map-load-end');
```

---

## 🔒 Considerações de Segurança

1. **Autenticação**: Todas as requisições incluem token Bearer
2. **Rate Limiting**: Debounce protege contra spam
3. **Validação**: Backend deve validar bounds e limites
4. **Sanitização**: Posts devem ser sanitizados no backend

---

## 🚀 Ativação

### Modo Estático (padrão)
```tsx
<MapComponent
  items={staticPosts}
  enableDynamicLoading={false}
  // ... outras props
/>
```

### Modo Dinâmico
```tsx
<MapComponent
  items={[]}  // Array vazio, posts vêm do backend
  enableDynamicLoading={true}
  // ... outras props
/>
```

---

## 📝 Próximos Passos

- [ ] Implementar clustering de markers em zoom baixo
- [ ] Adicionar filtros (data, tipo, autor)
- [ ] Implementar paginação para grandes datasets
- [ ] WebSocket para atualizações em tempo real
- [ ] Service Worker para cache offline

---

## 🐛 Troubleshooting

### Posts não carregam
1. Verificar endpoint no `src/config/api.ts`
2. Conferir logs do console
3. Validar token de autenticação
4. Verificar CORS no backend

### Cache não funciona
1. Verificar TTL não expirou
2. Coordenadas muito variáveis (usar arredondamento)
3. Limpar cache manualmente: `mapService.clearCache()`

### Performance ruim
1. Reduzir limite de posts por zoom
2. Aumentar debounce (ex: 1000ms)
3. Implementar clustering
4. Otimizar query no backend

---

**Desenvolvido para GeoCidadão Platform** 🗺️
