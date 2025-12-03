import { useEffect, useState, useCallback, useRef } from "react";
import "../styles/components/FeedList.css";
import PostCard from "./PostCard/PostCard";
import type { Post } from "../../data/@types/Post";
import PostCreator from "./PostCreator";
import { useFeed } from "../hooks/useFeed";

interface FeedListProps {
  isMapExpanded: boolean;
  feedRef: React.RefObject<HTMLDivElement>;
  items?: Post[];
  onMapItemClick?: (post: Post) => void;
}

const FeedList: React.FC<FeedListProps> = ({
  isMapExpanded,
  feedRef,
  items: externalItems,
  onMapItemClick,
}) => {
  const [isMobile, setIsMobile] = useState<boolean>(false);
  const observerRef = useRef<IntersectionObserver | null>(null);
  const loadMoreRef = useRef<HTMLDivElement | null>(null);

  // Usa o hook customizado para gerenciar o feed
  const { posts, loading, error, hasMore, loadMore, refresh } = useFeed({
    pageSize: 20,
  });

  // Usa items externos se fornecidos, senão usa os posts do hook
  const displayItems = externalItems || posts;

  useEffect(() => {
    const checkMobile = () => setIsMobile(window.innerWidth < 768);
    checkMobile();
    window.addEventListener("resize", checkMobile);
    return () => window.removeEventListener("resize", checkMobile);
  }, []);

  /**
   * Implementa Intersection Observer para infinite scroll
   */
  const setupIntersectionObserver = useCallback(() => {
    if (observerRef.current) observerRef.current.disconnect();

    observerRef.current = new IntersectionObserver(
      (entries) => {
        const firstEntry = entries[0];
        if (firstEntry.isIntersecting && hasMore && !loading) {
          loadMore();
        }
      },
      {
        root: null,
        rootMargin: "200px", // Começa a carregar 200px antes do fim
        threshold: 0.1,
      }
    );

    if (loadMoreRef.current) {
      observerRef.current.observe(loadMoreRef.current);
    }
  }, [hasMore, loading, loadMore]);

  useEffect(() => {
    // Só configura observer se não há items externos (modo dinâmico)
    if (!externalItems) {
      setupIntersectionObserver();
    }

    return () => {
      if (observerRef.current) {
        observerRef.current.disconnect();
      }
    };
  }, [setupIntersectionObserver, externalItems]);

  return (
    <div
      ref={feedRef}
      className={`feed-list ${
        isMapExpanded && isMobile ? "fade-out" : "fade-in"
      } `}
    >
      <PostCreator />

      {error && (
        <div className="feed-error">
          <p>Erro ao carregar feed: {error}</p>
          <button onClick={refresh} className="retry-button">
            Tentar novamente
          </button>
        </div>
      )}

      {displayItems && displayItems.length > 0 ? (
        <>
          {displayItems
            .filter((item) => item && item.id && item.author) // Filtra posts inválidos
            .map((item: Post) => (
              <PostCard
                key={item.id}
                post={item}
                onMap={onMapItemClick}
              />
            ))}

          {/* Elemento sentinel para infinite scroll */}
          {!externalItems && hasMore && (
            <div ref={loadMoreRef} className="load-more-trigger">
              {loading && (
                <div className="loading-spinner">
                  <div className="spinner"></div>
                  <p>Carregando mais posts...</p>
                </div>
              )}
            </div>
          )}
        </>
      ) : !loading ? (
        <div className="no-posts">Nenhum post disponível.</div>
      ) : null}

      {/* Loading inicial */}
      {loading && displayItems.length === 0 && (
        <div className="loading-spinner">
          <div className="spinner"></div>
          <p>Carregando feed...</p>
        </div>
      )}
    </div>
  );
};

export default FeedList;
