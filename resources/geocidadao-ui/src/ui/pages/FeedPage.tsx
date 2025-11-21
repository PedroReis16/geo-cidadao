import React, { useState, useRef, useMemo } from "react";
import "../styles/pages/FeedPage.css";
import type { Coordinates } from "../../data/@types/Coordinates";
import FeedList from "../components/FeedList";
import MapComponent from "../components/MapComponent";
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
  const [items] = useState<Post[]>(POSTS);
  const [zoom, setZoom] = useState<number>(12);
  const [center, setCenter] = useState<Coordinates>({
    lat: -23.5505,
    lng: -46.6333,
  });
  const [isMapExpanded, setIsMapExpanded] = useState(false);
  const [selectedItem, setSelectedItem] = useState<Post | null>(null);

  const feedRef = useRef<HTMLDivElement>(null!);

  // Filtra posts que possuem coordenadas
  const postsWithLocation = useMemo(
    () => items.filter((post) => post.coordinates),
    [items]
  );

  const handleMapItemClick = (post: Post) => {
    setSelectedItem(post);
    if (post.coordinates) {
      setCenter(post.coordinates);
      setIsMapExpanded(true);
    }
  };

  return (
    <div className="feed-page">
      <main className="feed-main">
        <div className="feed-wrapper">
          <FeedList
            isMapExpanded={isMapExpanded}
            feedRef={feedRef}
            items={items}
            onMapItemClick={handleMapItemClick}
          />
        </div>
      </main>

      {/* Minimapa fixo - só aparece se houver posts com localização */}
      {postsWithLocation.length > 0 && (
        <div className={`map-wrapper ${isMapExpanded ? "expanded" : "collapsed"}`}>
          <MapComponent
            items={postsWithLocation}
            center={center}
            zoom={zoom}
            setZoom={setZoom}
            setCenter={setCenter}
            isMapExpanded={isMapExpanded}
            setIsMapExpanded={setIsMapExpanded}
            selectedItem={selectedItem}
            setSelectedItem={setSelectedItem}
          />
        </div>
      )}
    </div>
  );
};

export default FeedPage;
