import type { Author } from "./Author";
import type { Coordinates } from "./Coordinates";
import type { MediaItem, MediaUrl } from "./MediaItem";
import type { Location } from "./Location";

export interface Post {
  id: string;
  author: Author;
  content: string;
  media?: MediaItem[];
  location?: Location;
  createdAt: string;
  likesCount: number;
  commentsCount: number;
  timestamp: string;
  coordinates?: Coordinates;
  isLiked?: boolean;
}

export interface FeedPost {
  id: string;
  media: MediaUrl[]; // Array de URLs de mídia
  author: Author;
  content: string;
  location: Location | null;
  createdAt: string;
  likesCount: number;
  commentsCount: number;
  timestamp: string;
}
