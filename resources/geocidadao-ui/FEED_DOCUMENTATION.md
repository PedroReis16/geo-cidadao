# Feed Dinâmico - Documentação

## Visão Geral

O sistema de feed foi otimizado para carregar posts de forma dinâmica com infinite scroll, consumindo a API do `FeedController`.

## Arquitetura

### Componentes

1. **FeedList** (`ui/components/FeedList.tsx`)
   - Componente principal do feed
   - Suporta modo dinâmico (com API) e modo estático (com props)
   - Implementa infinite scroll com Intersection Observer
   - Gerencia estados de loading e erro

2. **PostCard** (`ui/components/PostCard/PostCard.tsx`)
   - Renderiza um card individual de post
   - Adaptado para usar `author` ao invés de `user`
   - Usa `likesCount` e `commentsCount` da API

3. **PostHeader** (`ui/components/PostCard/PostHeader.tsx`)
   - Exibe informações do autor do post
   - Formata timestamp usando utilitário de datas

### Hooks

**useFeed** (`ui/hooks/useFeed.ts`)
- Hook customizado para gerenciar estado do feed
- Carrega posts com paginação
- Implementa refresh e loadMore
- Converte `FeedPost` (API) para `Post` (UI)

### Services

**feedService** (`data/services/feedService.ts`)
- Serviço de comunicação com a API
- Função `getFeed(page, pageSize)` para buscar posts
- Gerenciamento automático de autenticação Keycloak

### Types

**Post.d.ts** (`data/@types/Post.d.ts`)
```typescript
export interface Post {
  id: string;
  author: Author;
  content: string;
  media?: MediaItem[];
  location?: Location;
  createdAt: string;
  likesCount: number;
  commentsCount: number;
  timestamp: string;
  coordinates?: Coordinates;
  isLiked?: boolean;
}

export interface FeedPost {
  id: string;
  media: MediaItem[];
  author: Author;
  content: string;
  location: Location | null;
  createdAt: string;
  likesCount: number;
  commentsCount: number;
  timestamp: string;
}
```

**Author.d.ts** (`data/@types/Author.d.ts`)
```typescript
export interface Author {
  id: string;
  name: string;
  username: string;
  profilePictureUrl: string;
}
```

## Uso

### Modo Dinâmico (Padrão)

```tsx
import FeedList from "./components/FeedList";

function App() {
  return (
    <FeedList 
      isMapExpanded={false}
      feedRef={feedRef}
      onMapItemClick={handleMapClick}
    />
  );
}
```

O componente automaticamente:
- Carrega posts da API
- Implementa infinite scroll
- Exibe loading e estados de erro
- Permite refresh

### Modo Estático (com Props)

```tsx
import FeedList from "./components/FeedList";

function App() {
  const customPosts = [...]; // Array de Post
  
  return (
    <FeedList 
      isMapExpanded={false}
      feedRef={feedRef}
      items={customPosts}  // Desabilita API e usa posts fornecidos
      onMapItemClick={handleMapClick}
    />
  );
}
```

### Hook useFeed (uso direto)

```tsx
import { useFeed } from "./hooks/useFeed";

function CustomFeed() {
  const { posts, loading, error, hasMore, loadMore, refresh } = useFeed({
    pageSize: 20
  });

  return (
    <div>
      {posts.map(post => <PostCard key={post.id} post={post} />)}
      {hasMore && <button onClick={loadMore}>Carregar mais</button>}
      <button onClick={refresh}>Atualizar</button>
    </div>
  );
}
```

## Funcionalidades

### Infinite Scroll
- Usa Intersection Observer para detectar quando usuário chega ao fim da lista
- Carrega próxima página automaticamente
- Margem de 200px antes do fim para pré-carregar

### Estados de Loading
- Loading inicial: Exibe spinner ao carregar primeira página
- Loading de paginação: Exibe spinner ao carregar mais posts
- Previne múltiplas requisições simultâneas

### Tratamento de Erros
- Exibe mensagem de erro amigável
- Botão "Tentar novamente" para retry
- Logs de erro no console para debug

### Otimizações
- Conversão eficiente de tipos (FeedPost → Post)
- Cleanup de observers ao desmontar componente
- Previne race conditions com ref de loading
- Reutilização de componentes com React.memo (recomendado)

## Configuração da API

Certifique-se de que a variável de ambiente está configurada:

```env
VITE_API_URL=http://localhost:8090
```

O endpoint do feed será: `http://localhost:8090/feed-service/Feed`

## Próximos Passos

### Melhorias Sugeridas

1. **Implementar Like/Unlike**
   ```tsx
   const handleLike = async (postId: string) => {
     // Chamar API de engagement
     // Atualizar estado local
   };
   ```

2. **Implementar Comentários**
   - Modal ou drawer para exibir comentários
   - Formulário para adicionar novo comentário

3. **Cache de Posts**
   - Usar React Query ou SWR
   - Cache em localStorage para offline

4. **Otimização de Imagens**
   - Lazy loading de imagens
   - Placeholder enquanto carrega

5. **Pull-to-Refresh**
   - Implementar gesto de arrastar para atualizar (mobile)

6. **Virtualização**
   - Para listas muito grandes, usar react-window ou react-virtualized

## Troubleshooting

### Posts não carregam
- Verificar se Keycloak está autenticado
- Verificar URL da API nas variáveis de ambiente
- Verificar console do navegador para erros

### Infinite scroll não funciona
- Verificar se `hasMore` está sendo atualizado corretamente
- Verificar se o observer está sendo inicializado
- Verificar altura do container do feed

### Performance ruim
- Considerar implementar React.memo nos componentes
- Virtualizar lista se houver muitos posts
- Otimizar re-renders com useCallback e useMemo
