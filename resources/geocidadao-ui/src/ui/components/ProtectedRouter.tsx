import React, { useEffect, useRef } from 'react';
import { useLocation } from 'react-router-dom';
import { useKeycloak } from '@react-keycloak/web';

interface ProtectedRouteProps {
  children: React.ReactNode;
}

const ProtectedRouter: React.FC<ProtectedRouteProps> = ({ children }) => {
  const { keycloak, initialized } = useKeycloak();
  const location = useLocation();
  const loginAttempted = useRef(false);

  useEffect(() => {
    if (!initialized) return;

    if (keycloak.authenticated) return;

    
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
    return <div>Loading...</div>;
  }

  if (!keycloak.authenticated) {
    return <div>Redirecionando para o login...</div>;
  }

  return <>{children}</>;
};

export default ProtectedRouter;
