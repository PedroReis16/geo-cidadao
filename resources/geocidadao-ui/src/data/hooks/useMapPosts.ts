import { useState, useEffect, useCallback, useRef } from "react";
import type { Post } from "../@types/Post";
import type { Coordinates } from "../@types/Coordinates";
import { mapService, type MapBounds, type MapPostsQuery } from "../services/mapService";

interface UseMapPostsOptions {
  center: Coordinates;
  zoom: number;
  enabled?: boolean; // Permite desabilitar o carregamento automático
  debounceMs?: number; // Tempo de debounce para evitar muitas requisições
}

interface UseMapPostsResult {
  posts: Post[];
  loading: boolean;
  error: string | null;
  totalCount: number;
  refresh: () => void;
  clearCache: () => void;
}

/**
 * Hook para gerenciar carregamento dinâmico de posts no mapa
 * Carrega posts baseado nos bounds visíveis e zoom atual
 * Implementa debounce, cache e cleanup de memória
 */
export const useMapPosts = ({
  center,
  zoom,
  enabled = true,
  debounceMs = 500,
}: UseMapPostsOptions): UseMapPostsResult => {
  const [posts, setPosts] = useState<Post[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [totalCount, setTotalCount] = useState(0);

  // Refs para controlar debounce e requisições em andamento
  const debounceTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const abortControllerRef = useRef<AbortController | null>(null);
  const lastQueryRef = useRef<string>("");

  /**
   * Busca posts para os bounds atuais
   */
  const fetchPosts = useCallback(
    async (bounds: MapBounds) => {
      // Cancela requisição anterior se ainda estiver em andamento
      if (abortControllerRef.current) {
        abortControllerRef.current.abort();
      }

      // Cria novo controller para esta requisição
      abortControllerRef.current = new AbortController();

      // Gera chave única para esta query
      const queryKey = JSON.stringify({ bounds, zoom });

      // Evita requisições duplicadas
      if (queryKey === lastQueryRef.current && posts.length > 0) {
        console.log("[useMapPosts] Query duplicada ignorada");
        return;
      }

      lastQueryRef.current = queryKey;
      setLoading(true);
      setError(null);

      try {
        const query: MapPostsQuery = { bounds, zoom };
        const response = await mapService.getPostsInBounds(query);

        // Verifica se a requisição não foi cancelada
        if (!abortControllerRef.current?.signal.aborted) {
          setPosts(response.posts);
          setTotalCount(response.totalCount);
          console.log(
            `[useMapPosts] Carregados ${response.posts.length} posts (total: ${response.totalCount})`
          );
        }
      } catch (err) {
        if (!abortControllerRef.current?.signal.aborted) {
          const errorMessage =
            err instanceof Error ? err.message : "Erro ao carregar posts";
          setError(errorMessage);
          console.error("[useMapPosts] Erro:", errorMessage);
        }
      } finally {
        if (!abortControllerRef.current?.signal.aborted) {
          setLoading(false);
        }
      }
    },
    [zoom, posts.length]
  );

  /**
   * Atualiza posts quando center ou zoom mudam (com debounce)
   */
  useEffect(() => {
    if (!enabled) {
      console.log("[useMapPosts] Carregamento desabilitado");
      return;
    }

    // Limpa timer anterior
    if (debounceTimerRef.current) {
      clearTimeout(debounceTimerRef.current);
    }

    // Agenda nova busca com debounce
    debounceTimerRef.current = setTimeout(() => {
      const bounds = mapService.calculateBoundsFromCenter(center, zoom);
      console.log("[useMapPosts] Buscando posts para:", { center, zoom, bounds });
      fetchPosts(bounds);
    }, debounceMs);

    // Cleanup
    return () => {
      if (debounceTimerRef.current) {
        clearTimeout(debounceTimerRef.current);
      }
    };
  }, [center, zoom, enabled, debounceMs, fetchPosts]);

  /**
   * Cleanup ao desmontar
   */
  useEffect(() => {
    return () => {
      // Cancela requisições pendentes
      if (abortControllerRef.current) {
        abortControllerRef.current.abort();
      }

      // Limpa timers
      if (debounceTimerRef.current) {
        clearTimeout(debounceTimerRef.current);
      }

      // Limpa posts da memória
      setPosts([]);
      console.log("[useMapPosts] Cleanup executado");
    };
  }, []);

  /**
   * Função para forçar atualização
   */
  const refresh = useCallback(() => {
    lastQueryRef.current = ""; // Força nova requisição
    const bounds = mapService.calculateBoundsFromCenter(center, zoom);
    fetchPosts(bounds);
  }, [center, zoom, fetchPosts]);

  /**
   * Limpa cache do serviço
   */
  const clearCache = useCallback(() => {
    mapService.clearCache();
    refresh();
  }, [refresh]);

  return {
    posts,
    loading,
    error,
    totalCount,
    refresh,
    clearCache,
  };
};
