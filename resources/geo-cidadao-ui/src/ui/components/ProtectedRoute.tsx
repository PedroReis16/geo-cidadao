import { useKeycloak } from '@react-keycloak/web';
import { useEffect, useRef } from 'react';
import type { ReactNode } from 'react';
import AuthLoading from './AuthLoading';

interface ProtectedRouteProps {
  children: ReactNode;
}

const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
  const { keycloak, initialized } = useKeycloak();
  const loginAttempted = useRef(false);

  useEffect(() => {
    // Se não estiver autenticado e ainda não tentou fazer login
    if (initialized && !keycloak.authenticated && !loginAttempted.current) {
      loginAttempted.current = true;
      
      // Salva a URL atual para retornar após login
      const currentPath = window.location.pathname + window.location.search;
      sessionStorage.setItem('keycloak_return_url', currentPath);
      
      // Faz login redirecionando para a página de callback
      keycloak.login({
        redirectUri: window.location.origin + '/auth/callback',
      });
    }
  }, [initialized, keycloak]);

  // Enquanto o Keycloak estiver inicializando
  if (!initialized) {
    return <AuthLoading />;
  }

  // Se não estiver autenticado, mostra loading enquanto redireciona
  if (!keycloak.authenticated) {
    return <AuthLoading />;
  }

  // Se estiver autenticado, renderiza o conteúdo
  return <>{children}</>;
};

export default ProtectedRoute;
