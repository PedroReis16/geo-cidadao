import React, { useEffect, useRef, useState } from 'react';
import { useLocation } from 'react-router-dom';
import { useKeycloak } from '@react-keycloak/web';
import LoadingSpinner from './LoadingSpinner';
import PageTransition from './PageTransition';

interface ProtectedRouteProps {
  children: React.ReactNode;
}

const ProtectedRouter: React.FC<ProtectedRouteProps> = ({ children }) => {
  const { keycloak, initialized } = useKeycloak();
  const location = useLocation();
  const loginAttempted = useRef(false);
  const [isReady, setIsReady] = useState(false);

  useEffect(() => {
    if (!initialized) return;

    if (keycloak.authenticated) {
      // Pequeno delay para garantir transição suave
      const timer = setTimeout(() => setIsReady(true), 300);
      return () => clearTimeout(timer);
    }

    
    if (location.pathname === '/auth/callback') return;

    
    if (!loginAttempted.current) {
      loginAttempted.current = true;

      const currentPath =
        location.pathname + location.search + location.hash;

      sessionStorage.setItem('keycloak_return_url', currentPath);

      keycloak.login({
        redirectUri: `${window.location.origin}/auth/callback`,
      });
    }
  }, [initialized, keycloak, location]);

  if (!initialized) {
    return <LoadingSpinner message="Inicializando..." />;
  }

  if (!keycloak.authenticated) {
    return <LoadingSpinner message="Verificando autenticação..." />;
  }

  if (!isReady) {
    return <LoadingSpinner message="Preparando ambiente..." />;
  }

  return <PageTransition>{children}</PageTransition>;
};

export default ProtectedRouter;
