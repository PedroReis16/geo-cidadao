import { useKeycloak } from '@react-keycloak/web';

/**
 * Hook customizado para acessar informações de autenticação do Keycloak
 */
export const useAuth = () => {
  const { keycloak, initialized } = useKeycloak();

  return {
    // Estado de autenticação
    isAuthenticated: keycloak.authenticated,
    isInitialized: initialized,
    
    // Informações do usuário
    user: keycloak.tokenParsed,
    username: keycloak.tokenParsed?.preferred_username,
    email: keycloak.tokenParsed?.email,
    name: keycloak.tokenParsed?.name,
    userId: keycloak.tokenParsed?.sub,
    
    // Tokens
    token: keycloak.token,
    refreshToken: keycloak.refreshToken,
    
    // Funções de autenticação
    login: () => keycloak.login(),
    logout: () => keycloak.logout(),
    register: () => keycloak.register(),
    
    // Atualizar token
    updateToken: (minValidity = 30) => keycloak.updateToken(minValidity),
    
    // Verificar roles
    hasRole: (role: string) => keycloak.hasRealmRole(role),
    hasResourceRole: (role: string, resource: string) => 
      keycloak.hasResourceRole(role, resource),
    
    // Instância do keycloak para casos avançados
    keycloak,
  };
};
