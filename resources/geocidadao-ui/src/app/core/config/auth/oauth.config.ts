import { environment } from '@environments/environment';
import { AuthConfig } from 'angular-oauth2-oidc';

export const authCodeFlowConfig: AuthConfig = {
  issuer: environment.oauth2.issuer,
  redirectUri: environment.oauth2.redirectUri,
  clientId: environment.oauth2.clientId,
  responseType: 'code',
  scope: environment.oauth2.scope,
  requireHttps: false,
  showDebugInformation: environment.oauth2.showDebugInformation,
  strictDiscoveryDocumentValidation: environment.oauth2.strictDiscoveryDocumentValidation,
  skipIssuerCheck: environment.oauth2.skipIssuerCheck,
};