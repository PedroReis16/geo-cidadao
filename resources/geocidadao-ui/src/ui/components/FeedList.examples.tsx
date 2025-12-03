// Exemplo de uso do FeedList otimizado

import { useRef } from "react";
import FeedList from "./components/FeedList";
import type { Post } from "../data/@types/Post";

/**
 * Exemplo 1: Uso básico com API (modo dinâmico)
 * O feed carrega automaticamente da API com infinite scroll
 */
export function BasicFeedExample() {
  const feedRef = useRef<HTMLDivElement>(null);

  return (
    <FeedList
      isMapExpanded={false}
      feedRef={feedRef}
      onMapItemClick={(post) => {
        console.log("Post clicado no mapa:", post);
        // Navegar para localização no mapa
      }}
    />
  );
}

/**
 * Exemplo 2: Feed com integração de mapa
 * Expande/colapsa o mapa baseado na interação do usuário
 */
export function FeedWithMapExample() {
  const feedRef = useRef<HTMLDivElement>(null);
  const [isMapExpanded, setIsMapExpanded] = useState(false);
  const [selectedPost, setSelectedPost] = useState<Post | null>(null);

  const handleMapItemClick = (post: Post) => {
    setSelectedPost(post);
    setIsMapExpanded(true);
    // Centralizar mapa nas coordenadas do post
    if (post.coordinates) {
      // Lógica de centralização do mapa
    }
  };

  return (
    <div className="feed-map-container">
      <FeedList
        isMapExpanded={isMapExpanded}
        feedRef={feedRef}
        onMapItemClick={handleMapItemClick}
      />
      {/* Componente do mapa */}
      <Map
        isExpanded={isMapExpanded}
        selectedPost={selectedPost}
        onCollapse={() => setIsMapExpanded(false)}
      />
    </div>
  );
}

/**
 * Exemplo 3: Feed com posts customizados (modo estático)
 * Útil para preview, busca filtrada, ou posts de um usuário específico
 */
export function CustomPostsFeedExample() {
  const feedRef = useRef<HTMLDivElement>(null);
  const [customPosts, setCustomPosts] = useState<Post[]>([]);

  // Carregar posts customizados
  useEffect(() => {
    async function loadUserPosts() {
      const response = await fetch("/api/user/123/posts");
      const posts = await response.json();
      setCustomPosts(posts);
    }
    loadUserPosts();
  }, []);

  return (
    <FeedList
      isMapExpanded={false}
      feedRef={feedRef}
      items={customPosts} // Desabilita API e usa posts fornecidos
    />
  );
}

/**
 * Exemplo 4: Uso do hook useFeed diretamente
 * Para controle fino sobre o comportamento do feed
 */
export function AdvancedFeedExample() {
  const { posts, loading, error, hasMore, loadMore, refresh } = useFeed({
    pageSize: 15,
  });

  if (error) {
    return (
      <div className="error-container">
        <p>Erro: {error}</p>
        <button onClick={refresh}>Tentar novamente</button>
      </div>
    );
  }

  return (
    <div className="custom-feed">
      <div className="feed-header">
        <h2>Meu Feed</h2>
        <button onClick={refresh} disabled={loading}>
          Atualizar
        </button>
      </div>

      <div className="posts-container">
        {posts.map((post) => (
          <PostCard
            key={post.id}
            post={post}
            onLike={handleLike}
            onComment={handleComment}
          />
        ))}
      </div>

      {loading && <LoadingSpinner />}

      {hasMore && !loading && (
        <button onClick={loadMore} className="load-more-btn">
          Carregar mais
        </button>
      )}
    </div>
  );
}

/**
 * Exemplo 5: Feed com filtros
 * Combinar com sistema de busca ou filtros
 */
export function FilteredFeedExample() {
  const [filter, setFilter] = useState<"all" | "images" | "videos">("all");
  const { posts, loading, refresh } = useFeed();

  const filteredPosts = useMemo(() => {
    if (filter === "all") return posts;
    
    return posts.filter((post) => {
      if (filter === "images") {
        return post.media?.some((m) => m.type === "image");
      }
      if (filter === "videos") {
        return post.media?.some((m) => m.type === "video");
      }
      return true;
    });
  }, [posts, filter]);

  return (
    <div>
      <div className="filters">
        <button onClick={() => setFilter("all")}>Todos</button>
        <button onClick={() => setFilter("images")}>Fotos</button>
        <button onClick={() => setFilter("videos")}>Vídeos</button>
      </div>

      <FeedList
        isMapExpanded={false}
        feedRef={useRef(null)}
        items={filteredPosts}
      />
    </div>
  );
}
