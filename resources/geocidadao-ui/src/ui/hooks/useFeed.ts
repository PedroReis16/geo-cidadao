import { useEffect, useState, useCallback, useRef } from "react";
import type { FeedPost, Post } from "../../data/@types/Post";
import { getFeed } from "../../data/services/feedService";

interface UseFeedOptions {
  initialPage?: number;
  pageSize?: number;
}

interface UseFeedReturn {
  posts: Post[];
  loading: boolean;
  error: string | null;
  hasMore: boolean;
  loadMore: () => void;
  refresh: () => void;
}

/**
 * Converte FeedPost da API para Post do componente
 */
const convertFeedPostToPost = (feedPost: FeedPost): Post | null => {
  // Validações básicas
  if (!feedPost || !feedPost.id || !feedPost.author) {
    console.warn("Post inválido recebido da API:", feedPost);
    return null;
  }

  try {
    return {
      id: feedPost.id,
      author: {
        id: feedPost.author?.id || "",
        name: feedPost.author?.name || "Usuário Desconhecido",
        username: feedPost.author?.username || "desconhecido",
        profilePictureUrl: feedPost.author?.profilePictureUrl,
      },
      content: feedPost.content || "",
      media: feedPost.media || [],
      location: feedPost.location || undefined,
      createdAt: feedPost.createdAt || new Date().toISOString(),
      likesCount: feedPost.likesCount || 0,
      commentsCount: feedPost.commentsCount || 0,
      timestamp: feedPost.timestamp || feedPost.createdAt || new Date().toISOString(),
      isLiked: false,
      coordinates: feedPost.location
        ? {
            lat: feedPost.location.latitude,
            lng: feedPost.location.longitude,
          }
        : undefined,
    };
  } catch (error) {
    console.error("Erro ao converter post:", error, feedPost);
    return null;
  }
};

/**
 * Hook customizado para gerenciar o feed com paginação infinita
 */
export const useFeed = ({
  initialPage = 1,
  pageSize = 20,
}: UseFeedOptions = {}): UseFeedReturn => {
  const [posts, setPosts] = useState<Post[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState<number>(initialPage);
  const [hasMore, setHasMore] = useState<boolean>(true);
  const isLoadingRef = useRef<boolean>(false);

  /**
   * Carrega posts do feed
   */
  const loadPosts = useCallback(
    async (pageToLoad: number, append: boolean = false) => {
      if (isLoadingRef.current) return;

      try {
        isLoadingRef.current = true;
        setLoading(true);
        setError(null);

        const feedPosts = await getFeed(pageToLoad, pageSize);
        const convertedPosts = feedPosts
          .map(convertFeedPostToPost)
          .filter((post): post is Post => post !== null);

        if (convertedPosts.length < pageSize) {
          setHasMore(false);
        }

        setPosts((prevPosts) =>
          append ? [...prevPosts, ...convertedPosts] : convertedPosts
        );
      } catch (err) {
        const errorMessage =
          err instanceof Error ? err.message : "Erro ao carregar feed";
        setError(errorMessage);
        console.error("Erro ao carregar feed:", err);
      } finally {
        setLoading(false);
        isLoadingRef.current = false;
      }
    },
    [pageSize]
  );

  /**
   * Carrega mais posts (próxima página)
   */
  const loadMore = useCallback(() => {
    if (!hasMore || isLoadingRef.current) return;

    const nextPage = page + 1;
    setPage(nextPage);
    loadPosts(nextPage, true);
  }, [hasMore, page, loadPosts]);

  /**
   * Recarrega o feed do início
   */
  const refresh = useCallback(() => {
    setPage(initialPage);
    setHasMore(true);
    setPosts([]);
    loadPosts(initialPage, false);
  }, [initialPage, loadPosts]);

  /**
   * Carrega posts iniciais
   */
  useEffect(() => {
    loadPosts(initialPage, false);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return {
    posts,
    loading,
    error,
    hasMore,
    loadMore,
    refresh,
  };
};
