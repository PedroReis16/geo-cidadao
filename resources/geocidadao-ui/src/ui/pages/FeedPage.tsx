import React, { useRef, useEffect } from "react";
import "../styles/pages/FeedPage.css";
import FeedList from "../components/FeedList";
import { useMap } from "../../data/hooks/useMap";
import type { Post } from "../../data/@types/Post";

// Mock data temporário - será substituído por chamadas à API
const POSTS: Post[] = [
  {
    id: "1",
    user: {
      name: "João Silva",
      avatar: "https://example.com/avatar1.jpg",
      username: "joao.silva",
    },
    text: "Estou explorando o novo parque da cidade!",
    media: [
      { type: "image", url: "https://example.com/image1.jpg" },
      { type: "video", url: "https://example.com/video1.mp4" },
    ],
    likes: 120,
    comments: 15,
    isLiked: true,
    timestamp: "2025-10-08T08:00:00Z",
    coordinates: { lat: -23.5505, lng: -46.6333 },
  },
  {
    id: "2",
    user: {
      name: "Maria Oliveira",
      avatar: "https://example.com/avatar2.jpg",
      username: "maria.oliveira",
    },
    text: "Amei o show ontem, foi incrível!",
    media: [{ type: "image", url: "https://example.com/image2.jpg" }],
    likes: 250,
    comments: 32,
    isLiked: false,
    timestamp: "2025-10-07T19:45:00Z",
    coordinates: { lat: -22.9707, lng: -43.1896 },
  },
  {
    id: "3",
    user: {
      name: "Carlos Almeida",
      avatar: "https://example.com/avatar3.jpg",
      username: "carlos.almeida",
    },
    text: "Não vejo a hora de viajar para o Japão!",
    media: [{ type: "image", url: "https://example.com/image3.jpg" }],
    likes: 89,
    comments: 8,
    isLiked: false,
    timestamp: "2025-10-06T12:30:00Z",
    coordinates: { lat: 35.6762, lng: 139.6503 },
  },
];

const FeedPage: React.FC = () => {
  const feedRef = useRef<HTMLDivElement>(null!);
  const { setPosts, setIsMapExpanded, isMapExpanded } = useMap();

  // Carrega os posts no contexto do mapa quando o componente monta
  useEffect(() => {
    setPosts(POSTS);
  }, [setPosts]);

  const handleMapItemClick = (post: Post) => {
    if (post.coordinates) {
      setIsMapExpanded(true);
    }
  };

  return (
    <div>
      <FeedList
        isMapExpanded={isMapExpanded}
        feedRef={feedRef}
        items={POSTS}
        onMapItemClick={handleMapItemClick}
      />
    </div>
  );
};

export default FeedPage;
