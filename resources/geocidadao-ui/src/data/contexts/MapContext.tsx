import { createContext } from "react";
import type { Dispatch, SetStateAction } from "react";
import type { Coordinates } from "../@types/Coordinates";
import type { Post } from "../@types/Post";

export interface MapContextType {
  // Estado do mapa
  isMapExpanded: boolean;
  setIsMapExpanded: Dispatch<SetStateAction<boolean>>;
  
  // Posição e zoom
  center: Coordinates;
  setCenter: Dispatch<SetStateAction<Coordinates>>;
  zoom: number;
  setZoom: Dispatch<SetStateAction<number>>;
  
  // Item selecionado
  selectedItem: Post | null;
  setSelectedItem: Dispatch<SetStateAction<Post | null>>;
  
  // Posts visíveis no mapa
  posts: Post[];
  setPosts: Dispatch<SetStateAction<Post[]>>;
  
  // Ações de navegação
  navigateToPost: (post: Post) => void;
  navigateToFeed: () => void;
  
  // Posição de novo item (para criação)
  newItemPos: Coordinates | null;
  setNewItemPos: Dispatch<SetStateAction<Coordinates | null>>;
}

export const MapContext = createContext<MapContextType | undefined>(undefined);
