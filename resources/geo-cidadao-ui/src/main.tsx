import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { ReactKeycloakProvider } from '@react-keycloak/web';
import keycloak from "./config/keycloak";
import LoadingSpinner from "./ui/components/LoadingSpinner";
import ErrorBoundary from "./ui/components/ErrorBoundary";
import "./ui/styles/reset.css";
import "./ui/styles/index.css";
import App from "./pages/App.tsx";

// Configurações de inicialização do Keycloak
const keycloakProviderInitConfig = {
  onLoad: 'check-sso' as const, // Verifica SSO sem forçar login imediatamente
  checkLoginIframe: false, // Desabilita verificação por iframe
  pkceMethod: 'S256' as const, // Habilita PKCE para maior segurança
};

// Evento que é chamado após autenticação bem-sucedida
const handleOnEvent = (event: string, error: unknown) => {
  if (event === 'onAuthSuccess') {
    console.log('Autenticação bem-sucedida!');
  }
  if (event === 'onAuthError') {
    console.error('Erro na autenticação:', error);
  }
  if (event === 'onTokenExpired') {
    console.log('Token expirado, renovando...');
    keycloak.updateToken(30);
  }
};

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ErrorBoundary>
      <ReactKeycloakProvider 
        authClient={keycloak}
        initOptions={keycloakProviderInitConfig}
        LoadingComponent={<LoadingSpinner />}
        onEvent={handleOnEvent}
      >
        <App />
      </ReactKeycloakProvider>
    </ErrorBoundary>
  </StrictMode>
);
