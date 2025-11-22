import React, { useState, useEffect, useCallback } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { MapContext } from "./MapContext";
import type { Coordinates } from "../@types/Coordinates";
import type { Post } from "../@types/Post";

interface MapProviderProps {
  children: React.ReactNode;
}

export const MapProvider: React.FC<MapProviderProps> = ({ children }) => {
  const navigate = useNavigate();
  const location = useLocation();

  // Estado do mapa
  const [isMapExpanded, setIsMapExpanded] = useState(false);
  const [center, setCenter] = useState<Coordinates>({
    lat: -23.5505,
    lng: -46.6333,
  });
  const [zoom, setZoom] = useState(12);
  const [selectedItem, setSelectedItem] = useState<Post | null>(null);
  const [posts, setPosts] = useState<Post[]>([]);
  const [newItemPos, setNewItemPos] = useState<Coordinates | null>(null);

  // Navegar para um post específico
  const navigateToPost = useCallback(
    (post: Post) => {
      setSelectedItem(post);
      if (post.coordinates) {
        setCenter(post.coordinates);
        setZoom(16); // Zoom maior para focar no post
      }
      // Navega para a página do post
      navigate(`/post/${post.id}`);
      // Recolhe o mapa para mostrar os detalhes
      setIsMapExpanded(false);
    },
    [navigate]
  );

  // Navegar para o feed
  const navigateToFeed = useCallback(() => {
    setSelectedItem(null);
    setZoom(12); // Zoom padrão do feed
    navigate("/feed");
  }, [navigate]);

  // Detecta mudanças de rota e ajusta o comportamento do mapa
  useEffect(() => {
    const path = location.pathname;

    // Se estiver na página de um post
    if (path.startsWith("/post/")) {
      const postId = path.split("/post/")[1];
      
      // Busca o post nos posts carregados
      const post = posts.find((p) => p.id === postId);
      if (post && post.coordinates) {
        // Usa setTimeout para evitar cascading renders
        setTimeout(() => {
          setSelectedItem(post);
          setCenter(post.coordinates);
          setZoom(16);
        }, 0);
      }
    } 
    // Se estiver no feed
    else if (path === "/feed") {
      // Mantém os posts carregados, mas remove seleção
      if (selectedItem && !isMapExpanded) {
        setTimeout(() => {
          setSelectedItem(null);
          setZoom(12);
        }, 0);
      }
    }
  }, [location.pathname, posts, selectedItem, isMapExpanded]);

  // Quando expandir o mapa, centraliza no item selecionado
  useEffect(() => {
    if (isMapExpanded && selectedItem?.coordinates) {
      setTimeout(() => {
        setCenter(selectedItem.coordinates);
      }, 0);
    }
  }, [isMapExpanded, selectedItem]);

  return (
    <MapContext.Provider
      value={{
        isMapExpanded,
        setIsMapExpanded,
        center,
        setCenter,
        zoom,
        setZoom,
        selectedItem,
        setSelectedItem,
        posts,
        setPosts,
        navigateToPost,
        navigateToFeed,
        newItemPos,
        setNewItemPos,
      }}
    >
      {children}
    </MapContext.Provider>
  );
};
