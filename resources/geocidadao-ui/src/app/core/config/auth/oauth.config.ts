import { AuthConfig } from 'angular-oauth2-oidc';
import { environment } from '../../../../environments/environment';

export const authCodeFlowConfig: AuthConfig = {
  issuer: environment.oauth2.issuer,
  redirectUri: window.location.origin + '/callback',
  clientId: environment.oauth2.clientId,
  responseType: 'code',
  scope: environment.oauth2.scope,
  useSilentRefresh: false,
  requireHttps: false,
  showDebugInformation: environment.oauth2.showDebugInformation,
  strictDiscoveryDocumentValidation: environment.oauth2.strictDiscoveryDocumentValidation,
  skipIssuerCheck: environment.oauth2.skipIssuerCheck,
};