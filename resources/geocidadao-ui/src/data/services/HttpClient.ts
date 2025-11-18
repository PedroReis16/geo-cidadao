import type { KeycloakInstance } from 'keycloak-js';

interface HttpClientConfig {
  keycloak: KeycloakInstance;
  baseURL?: string;
}

/**
 * Cliente HTTP que adiciona automaticamente o token de autenticação
 * e renova o token quando necessário
 */
class HttpClient {
  private keycloak: KeycloakInstance;
  private baseURL: string;

  constructor(config: HttpClientConfig) {
    this.keycloak = config.keycloak;
    this.baseURL = config.baseURL || import.meta.env.VITE_API_BASE_URL || '';
  }

  /**
   * Garante que o token está válido antes de fazer a requisição
   */
  private async ensureTokenValidity(): Promise<string | undefined> {
    try {
      // Tenta renovar o token se ele vai expirar em menos de 30 segundos
      await this.keycloak.updateToken(30);
      return this.keycloak.token;
    } catch (error) {
      console.error('Erro ao renovar token:', error);
      // Se falhar, tenta fazer login novamente
      this.keycloak.login();
      throw new Error('Token inválido. Redirecionando para login...');
    }
  }

  /**
   * Realiza uma requisição HTTP
   */
  private async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const token = await this.ensureTokenValidity();
    
    const url = `${this.baseURL}${endpoint}`;
    
    const headers: Record<string, string> = {
      'Content-Type': 'application/json',
    };

    // Adiciona headers customizados
    if (options.headers) {
      Object.entries(options.headers).forEach(([key, value]) => {
        headers[key] = value as string;
      });
    }

    // Adiciona o token de autenticação
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(url, {
      ...options,
      headers,
    });

    // Se não autorizado, tenta fazer login
    if (response.status === 401) {
      this.keycloak.login();
      throw new Error('Não autorizado. Redirecionando para login...');
    }

    // Se erro no servidor
    if (!response.ok) {
      const error = await response.text();
      throw new Error(error || `Erro HTTP: ${response.status}`);
    }

    // Se resposta vazia (204 No Content)
    if (response.status === 204) {
      return {} as T;
    }

    return response.json();
  }

  /**
   * GET request
   */
  async get<T>(endpoint: string, options?: RequestInit): Promise<T> {
    return this.request<T>(endpoint, { ...options, method: 'GET' });
  }

  /**
   * POST request
   */
  async post<T>(
    endpoint: string,
    data?: unknown,
    options?: RequestInit
  ): Promise<T> {
    return this.request<T>(endpoint, {
      ...options,
      method: 'POST',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  /**
   * PUT request
   */
  async put<T>(
    endpoint: string,
    data?: unknown,
    options?: RequestInit
  ): Promise<T> {
    return this.request<T>(endpoint, {
      ...options,
      method: 'PUT',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  /**
   * PATCH request
   */
  async patch<T>(
    endpoint: string,
    data?: unknown,
    options?: RequestInit
  ): Promise<T> {
    return this.request<T>(endpoint, {
      ...options,
      method: 'PATCH',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  /**
   * DELETE request
   */
  async delete<T>(endpoint: string, options?: RequestInit): Promise<T> {
    return this.request<T>(endpoint, { ...options, method: 'DELETE' });
  }
}

export default HttpClient;