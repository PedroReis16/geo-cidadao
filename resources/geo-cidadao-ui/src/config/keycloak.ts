import Keycloak from 'keycloak-js';

// Configuração do Keycloak usando variáveis de ambiente
const keycloakConfig = {
  url: import.meta.env.VITE_KEYCLOAK_URL || 'http://localhost:8082/',
  realm: import.meta.env.VITE_KEYCLOAK_REALM || 'geocidadao',
  clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID || 'geo-cidadao-ui',
};

const keycloak = new Keycloak(keycloakConfig);

export default keycloak;
