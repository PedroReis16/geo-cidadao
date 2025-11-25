const API_BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:8081";

export const API_ENDPOINTS = {
  GERENCIAMENTO_POSTS: `${API_BASE_URL}/gerenciamento-posts`,
  GERENCIAMENTO_USUARIOS: `${API_BASE_URL}/gerenciamento-usuarios`,
  FEED_SERVICE: `${API_BASE_URL}/feed-service`,
  FEED_MAP: `${API_BASE_URL}/feed-map`,
  ENGAGEMENT_SERVICE: `${API_BASE_URL}/engagement-service`,
};

export default API_ENDPOINTS;
