import { useEffect, useState, useCallback, useRef } from "react";
import type { FeedPost, Post } from "../../data/@types/Post";
import type { MediaItem } from "../../data/@types/MediaItem";
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
 * Converte URL de mídia em MediaItem
 * Detecta o tipo baseado na extensão do arquivo ou assume imagem por padrão
 */
const convertMediaUrlToMediaItem = (url: string): MediaItem => {
  const lowerUrl = url.toLowerCase();
  const isVideo = lowerUrl.includes('.mp4') || 
                  lowerUrl.includes('.webm') || 
                  lowerUrl.includes('.ogg') ||
                  lowerUrl.includes('video');
  
  const type: "image" | "video" = isVideo ? "video" : "image";
  const mediaItem: MediaItem = {
    type,
    url: url,
  };
  
  console.log('🔄 Convertendo mídia:', url, '→', mediaItem.type);
  return mediaItem;
};

/**
 * Converte FeedPost da API para Post do componente
 */
const convertFeedPostToPost = (feedPost: FeedPost): Post | null => {
  // Validações básicas
  if (!feedPost) {
    console.warn("❌ Post nulo recebido da API");
    return null;
  }
  
  if (!feedPost.id) {
    console.warn("❌ Post sem ID:", feedPost);
    return null;
  }
  
  if (!feedPost.author) {
    console.warn("❌ Post sem autor:", feedPost);
    return null;
  }

  try {
    const converted: Post = {
      id: feedPost.id,
      author: {
        id: feedPost.author.id || "",
        name: feedPost.author.name || "Usuário Desconhecido",
        username: feedPost.author.username || "desconhecido",
        profilePictureUrl: feedPost.author.profilePictureUrl || undefined,
      },
      content: feedPost.content || "",
      media: Array.isArray(feedPost.media) 
        ? feedPost.media.map(convertMediaUrlToMediaItem)
        : [],
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
    
    return converted;
  } catch (error) {
    console.error("❌ Erro ao converter post:", error, feedPost);
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
