import React from "react";
import { Outlet } from "react-router-dom";
import { useMap } from "../../data/hooks/useMap";
import MapComponent from "./MapComponent";
import "../styles/components/MapLayout.css";

/**
 * Layout global que mantém o MapComponent visível em todas as páginas
 * O mapa funciona como uma camada de navegação paralela
 */
const MapLayout: React.FC = () => {
  const {
    posts,
    center,
    zoom,
    setZoom,
    setCenter,
    isMapExpanded,
    setIsMapExpanded,
    selectedItem,
    setSelectedItem,
    navigateToPost,
    newItemPos,
  } = useMap();

  // Filtra posts que possuem coordenadas
  const postsWithLocation = posts.filter((post) => post.coordinates);

  return (
    <div className="map-layout">
      {/* Conteúdo da página atual (Feed, PostDetail, etc) */}
      <div className="map-layout-content">
        <Outlet />
      </div>

      {/* Mapa global - só aparece se houver posts com localização */}
      {postsWithLocation.length > 0 && (
        <div className={`map-layout-wrapper ${isMapExpanded ? "expanded" : "collapsed"}`}>
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
            newItemPos={newItemPos}
            onItemPreviewClick={navigateToPost}
          />
        </div>
      )}
    </div>
  );
};

export default MapLayout;
