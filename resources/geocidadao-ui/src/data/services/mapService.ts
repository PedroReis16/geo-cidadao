import type { Post } from "../@types/Post";
import type { Coordinates } from "../@types/Coordinates";
import API_ENDPOINTS from "../../config/api";
import keycloak from "../../config/keycloak";

export interface MapBounds {
  north: number;
  south: number;
  east: number;
  west: number;
}

export interface MapPostsQuery {
  bounds: MapBounds;
  zoom: number;
  limit?: number;
}

export interface MapPostsResponse {
  posts: Post[];
  totalCount: number;
  bounds: MapBounds;
}

class MapService {
  private cache: Map<string, { data: MapPostsResponse; timestamp: number }>;
  private readonly CACHE_TTL = 5 * 60 * 1000; // 5 minutos

  constructor() {
    this.cache = new Map();
  }

  /**
   * Gera uma chave de cache baseada nos bounds e zoom
   */
  private getCacheKey(query: MapPostsQuery): string {
    const { bounds, zoom } = query;
    // Arredonda coordenadas para reduzir variações mínimas
    const n = bounds.north.toFixed(4);
    const s = bounds.south.toFixed(4);
    const e = bounds.east.toFixed(4);
    const w = bounds.west.toFixed(4);
    return `${n},${s},${e},${w},${zoom}`;
  }

  /**
   * Verifica se o cache é válido
   */
  private isCacheValid(timestamp: number): boolean {
    return Date.now() - timestamp < this.CACHE_TTL;
  }

  /**
   * Limpa cache expirado
   */
  private cleanExpiredCache(): void {
    const now = Date.now();
    for (const [key, value] of this.cache.entries()) {
      if (now - value.timestamp >= this.CACHE_TTL) {
        this.cache.delete(key);
      }
    }
  }

  /**
   * Busca posts dentro dos bounds especificados
   */
  async getPostsInBounds(query: MapPostsQuery): Promise<MapPostsResponse> {
    const cacheKey = this.getCacheKey(query);
    
    // Verifica cache
    const cached = this.cache.get(cacheKey);
    if (cached && this.isCacheValid(cached.timestamp)) {
      console.log("[MapService] Retornando do cache:", cacheKey);
      return cached.data;
    }

    try {
      // Limpa cache periodicamente
      this.cleanExpiredCache();

      // Garante que o token está válido
      await keycloak.updateToken(30);

      const { bounds, zoom, limit } = query;
      
      // Calcula limite dinâmico baseado no zoom
      const dynamicLimit = this.calculateLimit(zoom, limit);

      const params = new URLSearchParams({
        north: bounds.north.toString(),
        south: bounds.south.toString(),
        east: bounds.east.toString(),
        west: bounds.west.toString(),
        zoom: zoom.toString(),
        limit: dynamicLimit.toString(),
      });

      console.log("[MapService] Buscando posts:", {
        bounds,
        zoom,
        limit: dynamicLimit,
      });

      const url = `${API_ENDPOINTS.FEED_MAP}/map/posts?${params.toString()}`;

      const response = await fetch(url, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${keycloak.token}`,
        },
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data: MapPostsResponse = await response.json();

      // Armazena no cache
      this.cache.set(cacheKey, {
        data,
        timestamp: Date.now(),
      });

      return data;
    } catch (error) {
      console.error("[MapService] Erro ao buscar posts:", error);
      
      // Em caso de erro, retorna vazio ao invés de falhar
      return {
        posts: [],
        totalCount: 0,
        bounds: query.bounds,
      };
    }
  }

  /**
   * Calcula limite de posts baseado no zoom
   * Zoom baixo = menos posts
   * Zoom alto = mais posts
   */
  private calculateLimit(zoom: number, customLimit?: number): number {
    if (customLimit) return customLimit;

    // Escala exponencial baseada no zoom
    if (zoom < 10) return 50;
    if (zoom < 12) return 100;
    if (zoom < 14) return 200;
    if (zoom < 16) return 500;
    return 1000;
  }

  /**
   * Calcula bounds baseado no centro e zoom
   * Útil para buscar posts iniciais
   */
  calculateBoundsFromCenter(
    center: Coordinates,
    zoom: number
  ): MapBounds {
    // Aproximação simples de bounds baseado no zoom
    // Quanto maior o zoom, menor a área
    const delta = 180 / Math.pow(2, zoom);
    
    return {
      north: Math.min(90, center.lat + delta),
      south: Math.max(-90, center.lat - delta),
      east: Math.min(180, center.lng + delta),
      west: Math.max(-180, center.lng - delta),
    };
  }

  /**
   * Limpa todo o cache
   */
  clearCache(): void {
    this.cache.clear();
    console.log("[MapService] Cache limpo");
  }

  /**
   * Obtém estatísticas do cache
   */
  getCacheStats() {
    const now = Date.now();
    const validEntries = Array.from(this.cache.values()).filter(
      (entry) => now - entry.timestamp < this.CACHE_TTL
    ).length;

    return {
      total: this.cache.size,
      valid: validEntries,
      expired: this.cache.size - validEntries,
    };
  }
}

// Singleton
export const mapService = new MapService();
