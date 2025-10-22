import { useMemo } from 'react';
import { useHttpClient } from './useHttpClient';
import PostService from '../services/PostService';

/**
 * Hook para obter uma instância do PostService
 */
export const usePostService = () => {
  const httpClient = useHttpClient();

  const postService = useMemo(() => {
    return new PostService(httpClient);
  }, [httpClient]);

  return postService;
};
