export const environment = {
  production: false,
  defaultLanguage: 'pt-BR',
  supportedLanguages: [
    { value: 'pt-BR', label: 'Português' },
    { value: 'en-US', label: 'English' },
    { value: 'es-ES', label: 'Español' },
  ],
  apiUrl: '',
  oauth2: {
    issuer: '',
    clientId: '',
    redirectUri: 'http://localhost:4200/callback',
    scope: 'openid',
    showDebugInformation: true,
    strictDiscoveryDocumentValidation: false,
    skipIssuerCheck: true,
    refreshTokenTimeThreshold: 1 * 60000, // 1 minute
  },
};
