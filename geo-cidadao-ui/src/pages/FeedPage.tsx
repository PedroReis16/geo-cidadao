import React, { useState, useEffect, useRef } from "react";
import MapComponent from "../ui/components/MapComponent";
import "../ui/styles/pages/FeedPage.css";
import type { FeedItem } from "../data/@types/FeedItem";
import type { Coordinates } from "../data/@types/Coordinates";
import FeedPost from "../ui/components/FeedPost";

const INITIAL_ITEMS: FeedItem[] = [
  {
    id: 1,
    lat: -23.5505,
    lng: -46.6333,
    title: "Item 1",
    description: "Centro de São Paulo",
    author: "João Silva",
    likes: 45,
    comments: 12,
    image: null,
  },
  {
    id: 2,
    lat: -23.5489,
    lng: -46.6388,
    title: "Item 2",
    description: "Praça da República",
    author: "Maria Santos",
    likes: 23,
    comments: 8,
    image: null,
  },
  {
    id: 3,
    lat: -23.5613,
    lng: -46.6556,
    title: "Item 3",
    description: "Avenida Paulista",
    author: "Pedro Costa",
    likes: 67,
    comments: 15,
    image: null,
  },
  {
    id: 4,
    lat: -23.5505,
    lng: -46.64,
    title: "Item 4",
    description: "Mercado Municipal",
    author: "Ana Lima",
    likes: 34,
    comments: 9,
    image: null,
  },
  {
    id: 5,
    lat: -23.558,
    lng: -46.662,
    title: "Item 5",
    description: "Parque Ibirapuera",
    author: "Carlos Souza",
    likes: 89,
    comments: 21,
    image: null,
  },
];

const FeedPage: React.FC = () => {
  const [items, setItems] = useState<FeedItem[]>(INITIAL_ITEMS);
  const [zoom, setZoom] = useState<number>(12);
  const [center, setCenter] = useState<Coordinates>({ lat: -23.5505, lng: -46.6333 });
  const [isMapExpanded, setIsMapExpanded] = useState<boolean>(false);
  const [selectedItem, setSelectedItem] = useState<FeedItem | null>(null);
  const [highlightedFeedItem, setHighlightedFeedItem] = useState<number | null>(null);
  const [isMobile, setIsMobile] = useState<boolean>(false);

  // const [showAddModal, setShowAddModal] = useState<boolean>(false);
  const [newItemPos, setNewItemPos] = useState<Coordinates | null>(null);
  // const [newItemData, setNewItemData] = useState({ title: "", description: "" });

  const feedRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    const checkMobile = () => setIsMobile(window.innerWidth < 768);
    checkMobile();
    window.addEventListener("resize", checkMobile);
    return () => window.removeEventListener("resize", checkMobile);
  }, []);

  return (
    <div className={`feed-container ${isMapExpanded ? "map-open" : ""}`}>
      
      <main className="feed-main">
        <div
          ref={feedRef}
          className={`feed-list ${isMapExpanded && isMobile ? "fade-out" : "fade-in"}`}
        >
          {items.map((item) => (
            <FeedPost
              key={item.id}
              content={item}
              highlightedFeedItem={highlightedFeedItem}
              setCenter={setCenter}
              setZoom={setZoom}
              setSelectedItem={setSelectedItem}
              setIsMapExpanded={setIsMapExpanded}
            />
          ))}
        </div>

        {/* Mapa */}
        <div className={`map-wrapper ${isMapExpanded ? "expanded" : "collapsed"}`}>
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
              setShowAddModal(true);
            }}
            newItemPos={newItemPos}
          />
        </div>
      </main>

    </div>
  );
};

export default FeedPage;
