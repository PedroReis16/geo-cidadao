export interface MediaItem {
  type: "image" | "video";
  url: string;
}

// Tipo para representar mídia retornada pela API (apenas string da URL)
export type MediaUrl = string;
