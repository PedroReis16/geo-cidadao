import type { Coordinates } from "./Coordinates";

export interface Post {
  id: string;
  user: User;
  text: string;
  media?: MediaItem[];
  likes: number;
  comments: number;
  isLiked: boolean;
  timestamp: string;
  coordinates: Coordinates;
}
