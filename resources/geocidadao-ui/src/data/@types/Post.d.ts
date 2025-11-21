import { Coordinates } from "./Coordinates";
import { MediaItem } from "./MediaItem";
import { User } from "./User";

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
