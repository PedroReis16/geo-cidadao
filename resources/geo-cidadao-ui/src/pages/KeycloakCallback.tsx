import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useKeycloak } from '@react-keycloak/web';
import AuthLoading from '../ui/components/AuthLoading';

/**
 * Página de callback do Keycloak
 * Esta página processa o retorno do login e redireciona o usuário
 * de forma sutil respeitando o tema da aplicação
 */
const KeycloakCallback = () => {
  const { keycloak, initialized } = useKeycloak();
  const navigate = useNavigate();

  useEffect(() => {
    if (initialized && keycloak.authenticated) {
      // Tenta recuperar a URL original do sessionStorage
      const returnUrl = sessionStorage.getItem('keycloak_return_url') || '/feed';
      
      // Limpa o sessionStorage
      sessionStorage.removeItem('keycloak_return_url');
      
      // Redireciona imediatamente para a página original sem delay
      navigate(returnUrl, { replace: true });
    }
  }, [initialized, keycloak.authenticated, navigate]);

  return <AuthLoading message="Autenticando..." />;
};

export default KeycloakCallback;

