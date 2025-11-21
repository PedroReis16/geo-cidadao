import type HttpClient from './HttpClient';

// Tipos de exemplo - ajuste conforme seus modelos
interface Post {
  id: string;
  title: string;
  content: string;
  authorId: string;
  createdAt: string;
  // adicione outros campos conforme necessário
}

interface CreatePostDto {
  title: string;
  content: string;
  // adicione outros campos conforme necessário
}

/**
 * Serviço para gerenciar Posts
 */
class PostService {
  private httpClient: HttpClient;

  constructor(httpClient: HttpClient) {
    this.httpClient = httpClient;
  }

  /**
   * Busca todos os posts
   */
  async getPosts(): Promise<Post[]> {
    return this.httpClient.get<Post[]>('/api/posts');
  }

  /**
   * Busca um post por ID
   */
  async getPostById(id: string): Promise<Post> {
    return this.httpClient.get<Post>(`/api/posts/${id}`);
  }

  /**
   * Cria um novo post
   */
  async createPost(data: CreatePostDto): Promise<Post> {
    return this.httpClient.post<Post>('/api/posts', data);
  }

  /**
   * Atualiza um post existente
   */
  async updatePost(id: string, data: Partial<CreatePostDto>): Promise<Post> {
    return this.httpClient.put<Post>(`/api/posts/${id}`, data);
  }

  /**
   * Deleta um post
   */
  async deletePost(id: string): Promise<void> {
    return this.httpClient.delete<void>(`/api/posts/${id}`);
  }
}

export default PostService;
