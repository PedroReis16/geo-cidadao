import { useEffect, useState } from "react";
import type { Post } from "../../data/@types/Post";
import "../../ui/styles/components/FeedList.css";
import PostCard from "./PostCard/PostCard";

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
  {
    id: "4",
    user: {
      name: "Paula Costa",
      avatar: "https://example.com/avatar4.jpg",
      username: "paula.costa",
    },
    text: "Dia perfeito para um passeio de bike!",
    media: [],
    likes: 65,
    comments: 5,
    isLiked: true,
    timestamp: "2025-10-06T09:00:00Z",
    coordinates: { lat: 40.7128, lng: -74.006 },
  },
  {
    id: "5",
    user: {
      name: "Lucas Santos",
      avatar: "https://example.com/avatar5.jpg",
      username: "lucas.santos",
    },
    text: "Acabei de terminar de ler o livro que todos estão falando!",
    media: [{ type: "image", url: "https://example.com/image4.jpg" }],
    likes: 210,
    comments: 40,
    isLiked: true,
    timestamp: "2025-10-05T18:30:00Z",
    coordinates: { lat: 51.5074, lng: -0.1278 },
  },
  {
    id: "6",
    user: {
      name: "Ana Souza",
      avatar: "https://example.com/avatar6.jpg",
      username: "ana.souza",
    },
    text: "Adorei esse novo café perto de casa, super recomendo!",
    media: [],
    likes: 80,
    comments: 10,
    isLiked: false,
    timestamp: "2025-10-05T16:00:00Z",
    coordinates: { lat: -23.1824, lng: -45.9027 },
  },
  {
    id: "7",
    user: {
      name: "Eduardo Pereira",
      avatar: "https://example.com/avatar7.jpg",
      username: "eduardo.pereira",
    },
    text: "Fui correr no parque e consegui meu melhor tempo!",
    media: [],
    likes: 58,
    comments: 6,
    isLiked: true,
    timestamp: "2025-10-04T14:00:00Z",
    coordinates: { lat: 48.8566, lng: 2.3522 },
  },
  {
    id: "8",
    user: {
      name: "Juliana Rodrigues",
      avatar: "https://example.com/avatar8.jpg",
      username: "juliana.rodrigues",
    },
    text: "Quem mais vai para o festival de música esse fim de semana?",
    media: [{ type: "video", url: "https://example.com/video2.mp4" }],
    likes: 150,
    comments: 25,
    isLiked: true,
    timestamp: "2025-10-04T13:30:00Z",
    coordinates: { lat: -33.8688, lng: 151.2093 },
  },
  {
    id: "9",
    user: {
      name: "Rafael Silva",
      avatar: "https://example.com/avatar9.jpg",
      username: "rafael.silva",
    },
    text: "Café da manhã perfeito para começar o dia!",
    media: [{ type: "image", url: "https://example.com/image5.jpg" }],
    likes: 95,
    comments: 12,
    isLiked: false,
    timestamp: "2025-10-03T08:00:00Z",
    coordinates: { lat: 40.7306, lng: -73.9352 },
  },
  {
    id: "10",
    user: {
      name: "Beatriz Martins",
      avatar: "https://example.com/avatar10.jpg",
      username: "beatriz.martins",
    },
    text: "Estudando para a prova de amanhã, wish me luck!",
    media: [],
    likes: 120,
    comments: 7,
    isLiked: true,
    timestamp: "2025-10-03T06:00:00Z",
    coordinates: { lat: 34.0522, lng: -118.2437 },
  },
];

interface FeedListProps {
  isMapExpanded: boolean;
  feedRef: React.RefObject<HTMLDivElement>;
}

const FeedList: React.FC<FeedListProps> = ({ isMapExpanded, feedRef }) => {
  const [items, setItems] = useState<Post[]>(POSTS);
  const [isMobile, setIsMobile] = useState<boolean>(false);

  useEffect(() => {
    const checkMobile = () => setIsMobile(window.innerWidth < 768);
    checkMobile();
    window.addEventListener("resize", checkMobile);
    return () => window.removeEventListener("resize", checkMobile);
  }, []);

  return (
    <div
      ref={feedRef}
      className={`feed-list ${
        isMapExpanded && isMobile ? "fade-out" : "fade-in"
      }`}
    >
      {items.map((item) => (
        <PostCard
          key={item.id}
          post={item}
        //   onLike={handlePostLike}
        //   onComment={loadPostComments}
        />
      ))}
    </div>
  );
};

export default FeedList;
