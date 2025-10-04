import React, { useState, useEffect, useRef } from "react";
import {
  MapPin,
  Plus,
  Heart,
  MessageCircle,
  Share2,
  Bookmark,
  Map,
} from "lucide-react";
import MapComponent from "../ui/components/MapComponent";
import "../ui/styles/Pages/FeedPage.css";

interface FeedItem {
  id: number;
  lat: number;
  lng: number;
  title: string;
  description: string;
  author: string;
  likes: number;
  comments: number;
  image: string | null;
}

interface Coordinates {
  lat: number;
  lng: number;
}

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

  const [showAddModal, setShowAddModal] = useState<boolean>(false);
  const [newItemPos, setNewItemPos] = useState<Coordinates | null>(null);
  const [newItemData, setNewItemData] = useState({ title: "", description: "" });

  const feedRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    const checkMobile = () => setIsMobile(window.innerWidth < 768);
    checkMobile();
    window.addEventListener("resize", checkMobile);
    return () => window.removeEventListener("resize", checkMobile);
  }, []);

  const handleAddItem = () => {
    if (!newItemData.title.trim() || !newItemPos) return;
    const newItem: FeedItem = {
      id: Date.now(),
      lat: newItemPos.lat,
      lng: newItemPos.lng,
      title: newItemData.title,
      description: newItemData.description,
      author: "Você",
      likes: 0,
      comments: 0,
      image: null,
    };
    setItems((prev) => [newItem, ...prev]);
    setShowAddModal(false);
    setNewItemData({ title: "", description: "" });
    setNewItemPos(null);
  };

  return (
    <div className={`feed-container ${isMapExpanded ? "map-open" : ""}`}>
      <header className="feed-header">
        <div className="feed-header-left">
          <MapPin size={28} className="icon-blue" />
          <h1>Social Map</h1>
        </div>
        <button
          className="toggle-map-btn"
          onClick={() => setIsMapExpanded(!isMapExpanded)}
        >
          <Map size={20} />
          {isMapExpanded ? "Ver Feed" : "Ver Mapa"}
        </button>
      </header>

      <main className="feed-main">
        <div
          ref={feedRef}
          className={`feed-list ${isMapExpanded && isMobile ? "fade-out" : "fade-in"}`}
        >
          {items.map((item) => (
            <div
              key={item.id}
              id={`feed-item-${item.id}`}
              className={`feed-item ${highlightedFeedItem === item.id ? "highlight" : ""}`}
            >
              <div className="feed-item-header">
                <div className="feed-item-author">
                  <div className="avatar">{item.author[0]}</div>
                  <div>
                    <h3>{item.author}</h3>
                    <p>{item.title}</p>
                  </div>
                </div>
                <button
                  className="see-map-btn"
                  onClick={() => {
                    setCenter({ lat: item.lat, lng: item.lng });
                    setZoom(16);
                    setSelectedItem(item);
                    setIsMapExpanded(true);
                  }}
                >
                  <MapPin size={14} /> Ver no mapa
                </button>
              </div>

              <div className="feed-item-content">
                <p>{item.description}</p>
                <small>
                  {item.lat.toFixed(4)}, {item.lng.toFixed(4)}
                </small>
              </div>

              <div className="feed-item-actions">
                <button>
                  <Heart size={20} /> {item.likes}
                </button>
                <button>
                  <MessageCircle size={20} /> {item.comments}
                </button>
                <button>
                  <Share2 size={20} />
                </button>
                <button>
                  <Bookmark size={20} />
                </button>
              </div>
            </div>
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

      {/* Modal adicionar */}
      {showAddModal && (
        <div className="modal-overlay fade-in">
          <div className="modal">
            <h2>
              <Plus size={20} /> Novo Item
            </h2>
            <label>Título *</label>
            <input
              type="text"
              value={newItemData.title}
              onChange={(e) => setNewItemData({ ...newItemData, title: e.target.value })}
              placeholder="Nome do item"
            />
            <label>Descrição</label>
            <textarea
              value={newItemData.description}
              onChange={(e) => setNewItemData({ ...newItemData, description: e.target.value })}
              placeholder="Descreva este local"
              rows={3}
            />
            <div className="modal-actions">
              <button
                onClick={() => {
                  setShowAddModal(false);
                  setNewItemData({ title: "", description: "" });
                  setNewItemPos(null);
                }}
              >
                Cancelar
              </button>
              <button onClick={handleAddItem} disabled={!newItemData.title.trim()}>
                Adicionar
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default FeedPage;
