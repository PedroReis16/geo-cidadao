import React from "react";
import { Outlet } from "react-router-dom";
import { useMap } from "../../data/hooks/useMap";
import MapComponent from "./MapComponent";
import NavigationBar from "./NavigationBar";
import "../styles/components/MapLayout.css";

/**
 * Layout global que mantém o MapComponent visível em todas as páginas
 * O mapa funciona como um popup fixo no canto inferior direito da tela
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
    setNewItemPos,
  } = useMap();

  // Filtra posts que possuem coordenadas
  const postsWithLocation = posts.filter((post) => post.coordinates);

  // Handler para clique no mapa (quando está em modo de seleção)
  const handleMapClick = (coords: { lat: number; lng: number }) => {
    // Sempre permite clique quando o mapa está expandido
    // Isso permite seleção de localização no PostCreator
    setNewItemPos(coords);
  };

  return (
    <div className="map-layout">
      {/* Barra de navegação */}
      <NavigationBar />

      {/* Conteúdo da página atual (Feed, PostDetail, etc) */}
      <div className="map-layout-content">
        <Outlet />
      </div>

      {/* Mapa global - sempre renderizado como popup fixo */}
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
          onMapClick={handleMapClick}
          enableDynamicLoading={true}
        />
      </div>
    </div>
  );
};

export default MapLayout;
