export interface FeedItem {
  id: number;
  lat: number;
  lng: number;
  title: string;
  description: string;
  author: string;
  likes: number;
  comments: number;
  image: string | null;
}