import { useKeycloak } from '@react-keycloak/web';
import { useMemo } from 'react';
import HttpClient from '../services/HttpClient';

/**
 * Hook para obter uma instância do HttpClient configurada com o Keycloak
 */
export const useHttpClient = () => {
  const { keycloak } = useKeycloak();

  const httpClient = useMemo(() => {
    return new HttpClient({
      keycloak,
      baseURL: import.meta.env.VITE_API_BASE_URL,
    });
  }, [keycloak]);

  return httpClient;
};
