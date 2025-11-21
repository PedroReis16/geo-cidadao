import Keycloak from "keycloak-js";

const keycloak = new Keycloak({
  url: import.meta.env.VITE_KEYCLOAK_URL || "http://localhost:8082/",
  realm: import.meta.env.VITE_KEYCLOAK_REALM || "geocidadao",
  clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID || "geocidadao-ui",
});

export default keycloak;