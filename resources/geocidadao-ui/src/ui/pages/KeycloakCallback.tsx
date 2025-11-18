import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useKeycloak } from '@react-keycloak/web';

const KeycloakCallback: React.FC = () => {
  const { keycloak, initialized } = useKeycloak();
  const navigate = useNavigate();

  useEffect(() => {
    if (!initialized) return;

    if (keycloak.authenticated) {
      const returnUrl =
        sessionStorage.getItem('keycloak_return_url') ?? '/';
      sessionStorage.removeItem('keycloak_return_url');

      navigate(returnUrl, { replace: true });
    } else {
      keycloak.login({
        redirectUri: `${window.location.origin}/auth/callback`,
      });
    }
  }, [initialized, keycloak, navigate]);

  return <div>Finalizando autenticação...</div>;
};

export default KeycloakCallback;
