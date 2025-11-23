import { Coordinates } from "./Coordinates";
import { MediaItem } from "./MediaItem";

export interface Post {
  id: string;
  author: Author;
  content: string;
  media?: MediaItem[];
  likes: number;
  comments: number;
  timestamp: string;
  coordinates?: Coordinates;
}
