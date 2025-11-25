const API_BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:8080";

export const API_ENDPOINTS = {
  GERENCIAMENTO_POSTS: `http://localhost:8091/gerenciamento-posts`,
  GERENCIAMENTO_USUARIOS: `http://localhost:8092/gerenciamento-usuarios`,
  FEED_SERVICE: `http://localhost:8093/feed-service`,
  FEED_MAP: `http://localhost:8094/feed-map`,
  ENGAGEMENT_SERVICE: `http://localhost:8092/engagement-service`,
};

export default API_ENDPOINTS;
