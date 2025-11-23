export interface Comment {
  id: string;
  postId: string;
  author: Author;
  content: string;
  createdAt: string | Date;
  likes: number;
  timestamp: string;
}
