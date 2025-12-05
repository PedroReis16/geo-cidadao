import { inject, Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { authCodeFlowConfig } from '@core/config';

@Injectable({
  providedIn: 'root',
})
export class OauthService {
  oauthService = inject(OAuthService);
  private configurePromise: Promise<void> | null = null;

  constructor() {
    this.configure();
  }

  private configure(): Promise<void> {
    if (!this.configurePromise) {
      this.oauthService.configure(authCodeFlowConfig);
      
      this.configurePromise = this.oauthService
        .loadDiscoveryDocument()
        .then(() => {
          return this.oauthService.tryLogin();
        })
        .then(() => {
          this.oauthService.setupAutomaticSilentRefresh();
        })
        .catch((error) => {
          console.error('Erro durante a configuração do OAuthService:', error);
        });
    }
    return this.configurePromise;
  }

  public async login() {
    try {
      await this.configure();
      this.oauthService.initLoginFlow(); //Redireciona para a página de autenticação do keycloak
    } catch (error) {
      console.error('Erro durante o login:', error);
    }
  }

  public async logout() {
    await this.configure();
    this.oauthService.logOut(); //Redireciona para a página de logout do keycloak
  }

  public get identityClaims() {
    return this.oauthService.getIdentityClaims();
  }

  public get accessToken() {
    return this.oauthService.getAccessToken();
  }

  public async isAuthenticated() {
    await this.configure();
    return this.oauthService.hasValidAccessToken();
  }
}
