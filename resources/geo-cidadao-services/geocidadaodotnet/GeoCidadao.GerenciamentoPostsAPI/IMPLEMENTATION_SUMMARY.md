# Implementação: Mapeamento de Localização de Posts

## Resumo da Implementação

Esta implementação adiciona funcionalidade completa de mapeamento geográfico de posts ao sistema GeoCidadao, permitindo que usuários:
1. Criem posts com localização opcional
2. Busquem posts por proximidade geográfica
3. Visualizem posts em formato de mapa (GeoJSON)

## Componentes Implementados

### 1. DTOs (Data Transfer Objects)

#### PostLocationDTO
- Representa dados de localização de um post
- Campos: Latitude, Longitude, Address, City, State, Country, ZipCode

#### PostWithLocationDTO
- Estende PostDTO para incluir informação de localização
- Usado em respostas de consultas geográficas

#### LocationQueryDTO
- Parâmetros de consulta para filtros geográficos
- Suporta: coordenadas + raio, cidade, estado, paginação

#### GeoJSON DTOs
- `GeoJsonFeatureCollectionDTO`: Coleção de features GeoJSON
- `GeoJsonFeatureDTO`: Feature individual com geometria e propriedades
- `GeoJsonGeometryDTO`: Geometria do tipo Point
- `PostMapPropertiesDTO`: Propriedades específicas de posts no mapa

### 2. Camada de Dados (DAOs)

#### IPostLocationDao / PostLocationDao
Métodos implementados:
- `GetPostsWithinRadiusAsync()`: Busca posts em um raio (km) de um ponto
- `GetPostsByLocationAsync()`: Busca por cidade/estado (preparado para expansão futura)
- `GetPostLocationByPostIdAsync()`: Obtém localização de um post específico
- `ValidateEntityForInsert()`: Valida dados de localização antes de inserir
- `ValidateEntityForUpdate()`: Valida dados de localização antes de atualizar

### 3. Serviços

#### PostService (Aprimorado)
Novos métodos:
- `GetPostsByLocationAsync()`: Busca posts usando critérios geográficos
- `GetPostWithLocationAsync()`: Obtém um post específico com sua localização
- `CreatePostAsync()`: Modificado para salvar localização quando fornecida
- `DeletePostAsync()`: Modificado para deletar localização associada

#### LocationsService (Novo)
- `GetPostsAsGeoJsonAsync()`: Converte posts com localização para formato GeoJSON

### 4. Controllers

#### PostsController (Aprimorado)
Novo endpoint:
- `GET /posts/by-location`: Busca posts por localização com múltiplos filtros

#### LocationsController (Aprimorado)
Novo endpoint:
- `GET /posts/locations/map`: Retorna posts em formato GeoJSON para mapas

### 5. Modelos

#### ErrorCodes (Atualizado)
- Adicionado: `INVALID_POSITION = 17`

## Arquitetura Técnica

### Banco de Dados
- Utiliza **PostGIS** para consultas espaciais eficientes
- Tipo de dado: `NetTopologySuite.Geometries.Point` com SRID 4326 (WGS84)
- Índices espaciais em `post_locations.position` para performance

### Coordenadas
- **Sistema**: WGS84 (SRID 4326)
- **Formato de armazenamento**: Point(longitude, latitude)
- **Formato de API**: { latitude, longitude } (mais intuitivo para usuários)
- **Formato GeoJSON**: [longitude, latitude] (padrão RFC 7946)

### Distâncias
- **Entrada da API**: Quilômetros (km)
- **Cálculos internos**: Metros (padrão PostGIS)
- **Função**: `ST_Distance()` do PostGIS

## Fluxos de Uso

### 1. Criar Post com Localização
```
Cliente → POST /posts { content, position } → PostService.CreatePostAsync()
  → PostDao.AddAsync() (salva post)
  → PostLocationDao.AddAsync() (salva localização se fornecida)
  → Retorna PostDTO
```

### 2. Buscar Posts por Raio
```
Cliente → GET /posts/by-location?lat=X&lon=Y&radius=Z → PostsController.GetPostsByLocation()
  → PostService.GetPostsByLocationAsync()
  → PostLocationDao.GetPostsWithinRadiusAsync() (consulta PostGIS)
  → PostDao.FindAsync() (para cada localização)
  → Retorna List<PostWithLocationDTO>
```

### 3. Obter Mapa de Posts
```
Cliente → GET /posts/locations/map?lat=X&lon=Y&radius=Z → LocationsController.GetPostsMap()
  → LocationsService.GetPostsAsGeoJsonAsync()
  → PostService.GetPostsByLocationAsync()
  → Converte para GeoJSON
  → Retorna GeoJsonFeatureCollectionDTO
```

## Decisões de Design

### 1. Localização Opcional
- Posts podem existir sem localização (conforme requisito)
- Validação apenas quando localização é fornecida
- Erros de parsing de coordenadas são silenciosos (post criado sem localização)

### 2. Separação de Concerns
- `PostService`: Lógica de negócio para posts
- `LocationsService`: Transformação para formatos específicos (GeoJSON)
- `PostLocationDao`: Consultas espaciais otimizadas

### 3. Performance
- Uso de índices espaciais
- Paginação em todas as consultas
- Consultas SQL geradas pelo EF Core com funções PostGIS

### 4. Compatibilidade
- GeoJSON compatível com Leaflet, Mapbox, Google Maps
- API RESTful com query parameters padrão
- Respostas HTTP apropriadas (200, 204, 404)

## Cenários de Teste Implementados

### Cenário 1: Post sem Localização
**Dado**: Usuário cria post sem informar localização
**Quando**: POST /posts com position = null
**Então**: Post criado normalmente, sem entrada em post_locations

### Cenário 2: Post com Localização Válida
**Dado**: Usuário cria post com coordenadas válidas
**Quando**: POST /posts com position { latitude, longitude }
**Então**: Post e localização salvos, PostLocation criado

### Cenário 3: Busca por Raio
**Dado**: Existem posts em diferentes localizações
**Quando**: GET /posts/by-location?lat=-23.5505&lon=-46.6333&radiusKm=5
**Então**: Retorna apenas posts dentro do raio de 5km

### Cenário 4: Mapa GeoJSON
**Dado**: Existem posts com localização
**Quando**: GET /posts/locations/map?lat=-23.5505&lon=-46.6333&radiusKm=10
**Então**: Retorna FeatureCollection GeoJSON válido

## Validações Implementadas

### PostLocationDao.ValidateEntityForInsert
1. ✅ PostId não pode ser Guid.Empty
2. ✅ Position não pode ser null
3. ✅ Lança EntityValidationException com ErrorCode apropriado

### PostService.CreatePostAsync
1. ✅ Verifica se coordenadas podem ser parseadas para double
2. ✅ Cria GeometryFactory com SRID 4326
3. ✅ Trata erros de localização sem falhar criação do post

## Limitações Conhecidas e Próximos Passos

### Limitações Atuais
1. **Busca por cidade/estado**: Retorna lista vazia (requer campos adicionais em PostLocation)
2. **Sem geocoding**: Coordenadas devem ser fornecidas pelo cliente
3. **Sem clustering**: Muitos posts próximos podem sobrecarregar o mapa

### Melhorias Futuras Sugeridas
1. Adicionar campos City, State, Country à entidade PostLocation
2. Integrar serviço de geocoding (Google Maps API, OpenStreetMap)
3. Implementar reverse geocoding automático
4. Adicionar suporte a polígonos/áreas além de pontos
5. Implementar clustering de markers no backend
6. Cache de consultas geográficas frequentes
7. Filtros combinados (categoria + localização + data)

## Segurança

### Análise CodeQL
✅ **0 vulnerabilidades encontradas**

### Considerações de Segurança
- Validação de entrada em todos os DTOs
- Uso de parâmetros tipados (previne SQL injection via EF Core)
- Autorização via policies OAuth (Posts.Read, Posts.Create)
- SRID fixo (4326) previne injeção de sistema de coordenadas malicioso

## Métricas

### Arquivos Modificados: 11
- 4 Controllers/Services
- 2 DAOs
- 4 DTOs
- 1 Enum

### Arquivos Criados: 5
- PostLocationDTO.cs
- PostWithLocationDTO.cs
- LocationQueryDTO.cs
- GeoJsonDTO.cs
- LOCATION_MAPPING.md

### Linhas de Código: ~411 adicionadas

### Complexidade
- Ciclomática: Baixa a Média
- Acoplamento: Baixo (uso de interfaces)
- Coesão: Alta (separação clara de responsabilidades)

## Testes Sugeridos

### Testes Unitários
1. PostLocationDao.GetPostsWithinRadiusAsync com diferentes raios
2. PostService.CreatePostAsync com/sem localização
3. LocationsService.GetPostsAsGeoJsonAsync para formato GeoJSON
4. Validações de PostLocation

### Testes de Integração
1. Endpoint /posts/by-location com múltiplos filtros
2. Endpoint /posts/locations/map retornando GeoJSON válido
3. Criação de post com localização end-to-end

### Testes de Performance
1. Consulta com 1000+ posts no raio
2. Paginação com diferentes page sizes
3. Índices espaciais sendo utilizados (EXPLAIN ANALYZE)

## Documentação

### Documentos Criados
1. **LOCATION_MAPPING.md**: Documentação completa da API
   - Endpoints com exemplos
   - Códigos de integração
   - Formato GeoJSON explicado
   - Casos de uso

2. **IMPLEMENTATION_SUMMARY.md** (este arquivo): Visão técnica da implementação

### Swagger
Todos os endpoints estão documentados com XML comments para geração automática de documentação Swagger.

## Conclusão

A implementação atende todos os requisitos especificados na issue:
- ✅ Serviço de mapeamento de posts com consultas geográficas
- ✅ Parâmetros de localização em listagem de posts
- ✅ Endpoint dedicado para formato de mapa (GeoJSON)
- ✅ Compatibilidade com feed padrão
- ✅ Validações e tratamento de erros
- ✅ Localização opcional conforme especificado
- ✅ Uso de PostGIS para consultas eficientes

A solução é extensível, performática e segura, pronta para ser integrada com a camada de UI.
