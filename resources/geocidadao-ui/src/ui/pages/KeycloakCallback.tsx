import React, { useEffect, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { useKeycloak } from '@react-keycloak/web';
import LoadingSpinner from '../components/LoadingSpinner';

const KeycloakCallback: React.FC = () => {
  const { keycloak, initialized } = useKeycloak();
  const navigate = useNavigate();

  const message = useMemo(() => {
    if (!initialized) return 'Finalizando autenticação...';
    if (keycloak.authenticated) return 'Autenticação concluída!';
    return 'Redirecionando para login...';
  }, [initialized, keycloak.authenticated]);

  useEffect(() => {
    if (!initialized) return;

    if (keycloak.authenticated) {
      // Pequeno delay para mostrar mensagem de sucesso antes de redirecionar
      const timer = setTimeout(() => {
        const returnUrl =
          sessionStorage.getItem('keycloak_return_url') ?? '/';
        sessionStorage.removeItem('keycloak_return_url');

        navigate(returnUrl, { replace: true });
      }, 500);

      return () => clearTimeout(timer);
    } else {
      keycloak.login({
        redirectUri: `${window.location.origin}/auth/callback`,
      });
    }
  }, [initialized, keycloak, navigate]);

  return <LoadingSpinner message={message} />;
};

export default KeycloakCallback;
