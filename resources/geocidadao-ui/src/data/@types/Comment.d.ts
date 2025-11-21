export interface Comment {
  id: string;
  postId: string;
  userId: string;
  content: string;
  createdAt: string | Date;
  updatedAt?: string | Date;
}
