# Arquitetura de Navegação Dual: Feed + Mapa

## Visão Geral

Esta aplicação implementa uma arquitetura inovadora de **navegação dual**, onde o usuário pode navegar tanto pelo feed tradicional quanto pelo mapa interativo, criando duas camadas paralelas de navegação que se complementam.

## Conceito Principal

O `MapComponent` deixou de ser um componente local da `FeedPage` e se tornou um **componente global persistente** que funciona como um "popup inteligente" presente em todas as páginas protegidas da aplicação.

### Fluxos de Navegação

#### 1. Feed → Mapa → Post Específico
```
Usuário no /feed
  ↓
Clica em localização de um post no feed
  ↓
Mapa expande e foca no ponto
  ↓
Usuário navega pelo mapa e seleciona um marcador
  ↓
Aplicação navega para /post/:id
  ↓
Mapa recolhe, mostrando detalhes do post
```

#### 2. Mapa → Feed
```
Usuário navegando pelo mapa expandido
  ↓
Clica no botão de fechar
  ↓
Mapa recolhe para minimapa
  ↓
Feed continua visível normalmente
```

#### 3. Post Específico → Mapa → Outro Post
```
Usuário em /post/:id
  ↓
Clica para expandir o mapa
  ↓
Mapa expande mostrando ponto atual + outros pontos próximos
  ↓
Seleciona outro marcador no mapa
  ↓
Aplicação navega para /post/:newId
  ↓
Mapa recolhe mostrando novo post
```

## Estrutura de Arquivos

```
src/
├── data/
│   ├── contexts/
│   │   ├── MapContext.tsx          # Interface do contexto do mapa
│   │   ├── MapProvider.tsx         # Provider com lógica de estado e navegação
│   │   └── RouteProvider.tsx       # Configuração de rotas (atualizado)
│   └── hooks/
│       └── useMap.ts               # Hook customizado para acessar MapContext
├── ui/
│   ├── components/
│   │   ├── MapComponent.tsx        # Componente do mapa (não modificado)
│   │   ├── MapLayout.tsx           # Layout global que renderiza o mapa
│   │   └── FeedList.tsx            # Lista do feed (não modificado)
│   ├── pages/
│   │   ├── FeedPage.tsx            # Página do feed (simplificada)
│   │   └── PostDetailPage.tsx      # Nova página de detalhes do post
│   └── styles/
│       ├── components/
│       │   └── MapLayout.css       # Estilos do layout global
│       └── pages/
│           ├── FeedPage.css        # Estilos do feed (simplificado)
│           └── PostDetailPage.css  # Estilos da página de post
```

## Componentes Principais

### 1. MapContext & MapProvider

**Localização**: `src/data/contexts/`

**Responsabilidades**:
- Gerenciar estado global do mapa (centro, zoom, expansão)
- Gerenciar posts carregados e post selecionado
- Fornecer funções de navegação (`navigateToPost`, `navigateToFeed`)
- Sincronizar estado com mudanças de rota

**Estado gerenciado**:
```typescript
{
  isMapExpanded: boolean,          // Se o mapa está expandido
  center: Coordinates,             // Centro do mapa
  zoom: number,                    // Nível de zoom
  selectedItem: Post | null,       // Post selecionado no mapa
  posts: Post[],                   // Todos os posts com localização
  newItemPos: Coordinates | null   // Posição para criar novo post (futuro)
}
```

### 2. MapLayout

**Localização**: `src/ui/components/MapLayout.tsx`

**Responsabilidades**:
- Renderizar o `<Outlet />` do React Router (conteúdo das páginas)
- Renderizar o `MapComponent` como camada global
- Controlar visibilidade do mapa (só aparece se houver posts com localização)

**Características**:
- Posicionamento fixo do mapa (minimapa no canto ou tela cheia)
- Transições suaves entre estados expandido/colapsado
- Responsivo (em mobile, mapa expandido oculta o conteúdo)

### 3. FeedPage (Simplificada)

**Mudanças principais**:
- Removida lógica de estado do mapa (agora no MapProvider)
- Removido `MapComponent` renderizado localmente
- Apenas carrega posts no contexto via `setPosts(POSTS)`
- Delega navegação para o contexto

### 4. PostDetailPage (Nova)

**Responsabilidades**:
- Exibir detalhes de um post específico
- Buscar post pelo ID (primeiro em memória, depois API)
- Permitir voltar ao feed
- Permitir expandir mapa para continuar navegando

## Hook Customizado: useMap

```typescript
const {
  isMapExpanded,
  setIsMapExpanded,
  posts,
  setPosts,
  selectedItem,
  setSelectedItem,
  center,
  setCenter,
  zoom,
  setZoom,
  navigateToPost,
  navigateToFeed,
  newItemPos,
  setNewItemPos
} = useMap();
```

Simplifica acesso ao contexto e garante que o componente está dentro do `MapProvider`.

## Configuração de Rotas

```tsx
<ProtectedLayout>              {/* Proteção de autenticação */}
  <MapProvider>                {/* Estado global do mapa */}
    <MapLayout>                {/* Layout com mapa global */}
      <Outlet />               {/* Páginas dinâmicas */}
    </MapLayout>
  </MapProvider>
</ProtectedLayout>
```

### Rotas disponíveis:
- `/` → Redireciona para `/feed`
- `/feed` → FeedPage
- `/post/:postId` → PostDetailPage
- `*` → NotFoundPage
- `/auth/callback` → KeycloakCallback (fora do layout protegido)

## Fluxo de Dados

### Carregamento de Posts (FeedPage)

```
FeedPage monta
  ↓
useEffect chama setPosts(POSTS)
  ↓
MapProvider atualiza contexto
  ↓
MapLayout recebe posts atualizados
  ↓
MapComponent renderiza marcadores
```

### Navegação via Mapa

```
Usuário clica em marcador no MapComponent
  ↓
onItemPreviewClick(post) é chamado
  ↓
MapProvider.navigateToPost(post) executa
  ↓
  - setSelectedItem(post)
  - setCenter(post.coordinates)
  - setZoom(16)
  - navigate('/post/:id')
  - setIsMapExpanded(false)
  ↓
React Router navega para PostDetailPage
  ↓
MapProvider detecta mudança de rota
  ↓
Mantém selectedItem e ajusta visualização
```

### Navegação via Feed

```
Usuário clica em localização no PostCard
  ↓
onMapItemClick(post) é chamado
  ↓
FeedPage chama setIsMapExpanded(true)
  ↓
MapLayout re-renderiza com mapa expandido
  ↓
MapProvider ajusta centro se selectedItem existir
```

## Responsividade

### Desktop (> 1024px)
- Minimapa: 300x250px no canto inferior direito
- Expandido: Tela cheia com overlay
- Feed e mapa visíveis simultaneamente

### Tablet (769px - 1024px)
- Minimapa: 350x280px
- Comportamento similar ao desktop

### Mobile (< 768px)
- Minimapa: Largura total - 32px, altura 200px
- Expandido: Tela cheia, oculta conteúdo completamente
- Minimapa posicionado acima da barra de navegação inferior

## Próximos Passos

### Funcionalidades Planejadas

1. **Criação de Posts com Localização**
   - Usuário clica no mapa para definir localização
   - `newItemPos` é atualizado
   - Modal/página de criação abre com coordenadas preenchidas

2. **Filtros no Mapa**
   - Filtrar por categoria de post
   - Filtrar por data
   - Filtrar por distância

3. **Clustering de Marcadores**
   - Agrupar marcadores próximos em zooms baixos
   - Expandir ao aumentar zoom

4. **Histórico de Navegação**
   - Rastrear posts visitados
   - Sugerir posts relacionados

5. **Integração com API**
   - Substituir dados mock
   - Lazy loading de posts por região
   - Cache de posts já carregados

## Benefícios da Arquitetura

✅ **Navegação Intuitiva**: Duas formas naturais de explorar conteúdo  
✅ **Estado Persistente**: Mapa mantém contexto entre navegações  
✅ **Reutilização**: MapComponent usado em múltiplas páginas  
✅ **Separação de Responsabilidades**: Cada componente tem papel claro  
✅ **Escalabilidade**: Fácil adicionar novas páginas com mapa  
✅ **Performance**: Estado compartilhado evita re-renderizações desnecessárias  

## Exemplo de Uso

### Adicionar nova página com mapa

```tsx
// src/ui/pages/ExplorePage.tsx
import { useMap } from "../../data/hooks/useMap";

const ExplorePage: React.FC = () => {
  const { setPosts, setIsMapExpanded } = useMap();

  useEffect(() => {
    // Carrega posts de exploração
    fetchExplorePosts().then(setPosts);
  }, []);

  return (
    <div className="explore-page">
      {/* Conteúdo da página */}
      {/* O mapa estará automaticamente disponível */}
    </div>
  );
};
```

### Adicionar à rota

```tsx
// RouteProvider.tsx
{
  path: "explore",
  element: <ExplorePage />,
}
```

Pronto! A página já terá acesso ao mapa global. 🎉
