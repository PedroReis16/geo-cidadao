import API_ENDPOINTS from "../../config/api";
import keycloak from "../../config/keycloak";
import type { FeedPost } from "../@types/Post";

export interface FeedResponse {
  posts: FeedPost[];
  hasMore: boolean;
  nextPage: number;
}

/**
 * Busca posts do feed com paginação
 */
export const getFeed = async (
  page: number = 1,
  pageSize: number = 20
): Promise<FeedPost[]> => {
  try {
    // Garante que o token está válido
    await keycloak.updateToken(30);

    const url = `${API_ENDPOINTS.FEED_SERVICE}/Feed?page=${page}&pageSize=${pageSize}`;

    const response = await fetch(url, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${keycloak.token}`,
      },
    });

    if (!response.ok) {
      if (response.status === 401) {
        keycloak.login();
        throw new Error("Não autorizado. Redirecionando para login...");
      }
      throw new Error(`Erro ao buscar feed: ${response.status}`);
    }

    const posts: FeedPost[] = await response.json();
    return posts;
  } catch (error) {
    console.error("Erro ao buscar feed:", error);
    throw error;
  }
};

/**
 * Verifica se o usuário curtiu um post
 */
export const checkIfLiked = async (postId: string): Promise<boolean> => {
  try {
    await keycloak.updateToken(30);

    const response = await fetch(
      `${API_ENDPOINTS.ENGAGEMENT_SERVICE}/Engagement/${postId}/liked`,
      {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${keycloak.token}`,
        },
      }
    );

    if (!response.ok) return false;

    const data = await response.json();
    return data.isLiked || false;
  } catch (error) {
    console.error("Erro ao verificar curtida:", error);
    return false;
  }
};
