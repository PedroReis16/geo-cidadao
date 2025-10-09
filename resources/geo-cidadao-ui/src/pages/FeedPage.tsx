import React, { useState, useEffect, useRef } from "react";
import "../ui/styles/pages/FeedPage.css";
import type { Coordinates } from "../data/@types/Coordinates";
import type { Post } from "../data/@types/Post";
import PostCard from "../ui/components/PostCard/PostCard";
import FeedList from "../ui/components/FeedList";

const FeedPage: React.FC = () => {
  const [zoom, setZoom] = useState<number>(12);
  const [center, setCenter] = useState<Coordinates>({
    lat: -23.5505,
    lng: -46.6333,
  });
  const [isMapExpanded, setIsMapExpanded] = useState<boolean>(false);
  const [selectedItem, setSelectedItem] = useState<Post | null>(null);
  const [highlightedFeedItem, setHighlightedFeedItem] = useState<string | null>(
    null
  );

  // const [showAddModal, setShowAddModal] = useState<boolean>(false);
  const [newItemPos, setNewItemPos] = useState<Coordinates | null>(null);
  // const [newItemData, setNewItemData] = useState({ title: "", description: "" });

  const feedRef = useRef<HTMLDivElement | null>(null);

  // Função para lidar com clique no preview do item no mapa
  const handleItemPreviewClick = (post: Post) => {
    // Minimiza o mapa
    setIsMapExpanded(false);

    // Define o item como selecionado
    setSelectedItem(post);
    setHighlightedFeedItem(post.id);

    // Aguarda um pouco para o mapa minimizar, depois rola até o item
    setTimeout(() => {
      const feedElement = document.getElementById(`feed-item-${post.id}`);
      if (feedElement && feedRef.current) {
        feedElement.scrollIntoView({
          behavior: "smooth",
          block: "center",
        });
      }

      // Remove o highlight após 3 segundos
      setTimeout(() => {
        setHighlightedFeedItem(null);
      }, 3000);
    }, 700); // Tempo da animação de fechar o mapa
  };

  const handlePostLike = (postId: string) => {};

  const loadPostComments = (postId: string) => {};

  return (
    <div className={`feed-container ${isMapExpanded ? "map-open" : ""}`}>
      <main className="feed-main">
        <FeedList isMapExpanded={isMapExpanded} feedRef={feedRef} />

        {/* Mapa */}
        {/* <div
          className={`map-wrapper ${isMapExpanded ? "expanded" : "collapsed"}`}
        >
          <MapComponent
            items={items}
            center={center}
            zoom={zoom}
            setZoom={setZoom}
            setCenter={setCenter}
            isMapExpanded={isMapExpanded}
            setIsMapExpanded={setIsMapExpanded}
            selectedItem={selectedItem}
            setSelectedItem={setSelectedItem}
            onMapClick={(coords) => {
              setNewItemPos(coords);
              // setShowAddModal(true);
            }}
            newItemPos={newItemPos}
            // onItemPreviewClick={handleItemPreviewClick}
          />
        </div> */}
      </main>
    </div>
  );
};

export default FeedPage;
