import { createRoot } from 'react-dom/client'
import './ui/styles/index.css'
import App from './App.tsx'
import { StrictMode } from 'react'
import AppKeycloakProvider from './data/contexts/KeycloakProvider.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <AppKeycloakProvider>
      <App />
    </AppKeycloakProvider>
  </StrictMode>
)
