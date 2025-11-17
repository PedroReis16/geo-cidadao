# Mapeamento de Localização de Posts - API Documentation

## Visão Geral

Esta funcionalidade permite que posts sejam criados com informações de localização geográfica e posteriormente consultados por região, possibilitando a visualização dos posts em um mapa interativo.

## Características Principais

- **Localização Opcional**: Posts podem ou não ter localização associada
- **Consultas Geográficas**: Buscar posts por coordenadas + raio, cidade, estado
- **Formato GeoJSON**: Endpoint dedicado para visualização em mapas
- **PostGIS**: Utiliza funções espaciais para consultas eficientes

## Endpoints Disponíveis

### 1. Criar Post com Localização

**Endpoint**: `POST /posts`

**Autenticação**: Bearer Token (Policy: Posts.Create)

**Body**:
```json
{
  "content": "Conteúdo do post",
  "position": {
    "latitude": "-23.5505",
    "longitude": "-46.6333",
    "address": "Av. Paulista, 1000",
    "city": "São Paulo",
    "state": "SP",
    "country": "Brasil",
    "zipCode": "01310-100"
  }
}
```

**Nota**: O campo `position` é opcional. Se não fornecido, o post será criado sem localização.

### 2. Buscar Posts por Localização

**Endpoint**: `GET /posts/by-location`

**Autenticação**: Bearer Token (Policy: Posts.Read)

**Query Parameters**:
- `latitude` (double, opcional): Latitude do ponto central
- `longitude` (double, opcional): Longitude do ponto central
- `radiusKm` (double, opcional): Raio de busca em quilômetros
- `city` (string, opcional): Filtrar por cidade
- `state` (string, opcional): Filtrar por estado
- `country` (string, opcional): Filtrar por país
- `itemsCount` (int, opcional): Número máximo de resultados
- `pageNumber` (int, opcional): Número da página (inicia em 1)

**Exemplo de Uso**:
```
GET /posts/by-location?latitude=-23.5505&longitude=-46.6333&radiusKm=5&itemsCount=20
```

**Response**:
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "content": "Conteúdo do post",
    "userId": "660e8400-e29b-41d4-a716-446655440000",
    "createdAt": "2025-01-15T10:30:00Z",
    "location": {
      "latitude": -23.5505,
      "longitude": -46.6333,
      "address": "Av. Paulista, 1000",
      "city": "São Paulo",
      "state": "SP",
      "country": "Brasil",
      "zipCode": "01310-100"
    }
  }
]
```

### 3. Obter Posts em Formato de Mapa (GeoJSON)

**Endpoint**: `GET /posts/locations/map`

**Autenticação**: Bearer Token (Policy: Posts.Read)

**Query Parameters**: (mesmos da busca por localização)
- `latitude` (double, opcional)
- `longitude` (double, opcional)
- `radiusKm` (double, opcional)
- `city` (string, opcional)
- `state` (string, opcional)
- `country` (string, opcional)
- `itemsCount` (int, opcional)
- `pageNumber` (int, opcional)

**Exemplo de Uso**:
```
GET /posts/locations/map?latitude=-23.5505&longitude=-46.6333&radiusKm=10
```

**Response** (GeoJSON FeatureCollection):
```json
{
  "type": "FeatureCollection",
  "features": [
    {
      "type": "Feature",
      "geometry": {
        "type": "Point",
        "coordinates": [-46.6333, -23.5505]
      },
      "properties": {
        "id": "550e8400-e29b-41d4-a716-446655440000",
        "content": "Conteúdo do post",
        "userId": "660e8400-e29b-41d4-a716-446655440000",
        "createdAt": "2025-01-15T10:30:00Z",
        "address": "Av. Paulista, 1000",
        "city": "São Paulo",
        "state": "SP"
      }
    }
  ]
}
```

## Formato GeoJSON

O formato GeoJSON retornado pelo endpoint `/posts/locations/map` segue o padrão [RFC 7946](https://tools.ietf.org/html/rfc7946) e pode ser usado diretamente com bibliotecas de mapas como:

- **Leaflet**: `L.geoJSON(data)`
- **Mapbox GL JS**: `map.addSource('posts', { type: 'geojson', data: data })`
- **Google Maps**: Usando `data.loadGeoJson()`

## Modos de Consulta

### 1. Por Coordenadas e Raio
Requer: `latitude`, `longitude`, `radiusKm`

Retorna posts dentro de um raio específico de um ponto central.

```
GET /posts/by-location?latitude=-23.5505&longitude=-46.6333&radiusKm=5
```

### 2. Por Cidade/Estado
Requer: `city` e/ou `state`

**Nota**: Esta funcionalidade atualmente retorna lista vazia pois requer campos adicionais na entidade PostLocation. Para implementação completa, seria necessário:
- Adicionar campos City, State, Country na entidade PostLocation
- Atualizar a migração do banco de dados
- Implementar a lógica de filtro por texto

### 3. Feed Geral (Sem Filtros)
Se nenhum parâmetro de localização for fornecido, a busca retorna lista vazia. Use o endpoint `/posts/{userId}/posts` para obter posts de um usuário específico.

## Regras de Validação

1. Ao criar um post com localização, as coordenadas devem ser válidas (latitude e longitude parseáveis para double)
2. Se as coordenadas forem inválidas, o post é criado mas a localização é ignorada
3. Posts sem localização não aparecem nas consultas geográficas
4. O raio de busca é especificado em quilômetros
5. As coordenadas seguem o padrão WGS84 (SRID 4326)

## Detalhes Técnicos

### Sistema de Coordenadas
- **SRID**: 4326 (WGS84)
- **Formato**: [longitude, latitude] (padrão GeoJSON)
- **Armazenamento**: NetTopologySuite Point no PostgreSQL/PostGIS

### Cálculo de Distância
- Usa a função `Distance()` do PostGIS
- Distâncias em metros (convertidas de/para quilômetros na API)
- Ordenação por proximidade ao ponto central

### Performance
- Índices espaciais na coluna `position` da tabela `post_locations`
- Paginação disponível via `itemsCount` e `pageNumber`
- Consultas otimizadas usando LINQ to SQL com funções PostGIS

## Exemplos de Integração

### JavaScript (Leaflet)
```javascript
fetch('/posts/locations/map?latitude=-23.5505&longitude=-46.6333&radiusKm=10', {
  headers: { 'Authorization': 'Bearer YOUR_TOKEN' }
})
.then(response => response.json())
.then(data => {
  L.geoJSON(data, {
    onEachFeature: (feature, layer) => {
      layer.bindPopup(feature.properties.content);
    }
  }).addTo(map);
});
```

### C# Client
```csharp
var query = new LocationQueryDTO
{
    Latitude = -23.5505,
    Longitude = -46.6333,
    RadiusKm = 5,
    ItemsCount = 20
};

var posts = await postService.GetPostsByLocationAsync(query);
```

## Próximos Passos Sugeridos

1. **Adicionar campos de texto para localização**: City, State, Country na entidade PostLocation
2. **Geocoding**: Integrar serviço de geocoding para converter endereços em coordenadas
3. **Reverse Geocoding**: Preencher automaticamente cidade/estado a partir de coordenadas
4. **Clustering**: Agrupar posts próximos para melhor visualização no mapa
5. **Filtros Combinados**: Permitir filtrar por categoria + localização
6. **Cache**: Implementar cache para consultas geográficas frequentes

## Suporte

Para dúvidas ou problemas, consulte a documentação da API via Swagger em `/swagger`.
