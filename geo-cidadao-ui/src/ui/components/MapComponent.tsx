import React, { useRef, useState, useEffect } from "react";
import { MapPin, Maximize2, ZoomIn, ZoomOut, X } from "lucide-react";
import L from "leaflet";
import "leaflet/dist/leaflet.css";
import "../styles/components/MapComponent.css";
import type { FeedItem } from "../../data/@types/FeedItem";
import type { Coordinates } from "../../data/@types/Coordinates";

interface MapComponentProps {
  items: FeedItem[];
  center: Coordinates;
  zoom: number;
  setZoom: React.Dispatch<React.SetStateAction<number>>;
  setCenter: React.Dispatch<React.SetStateAction<Coordinates>>;
  isMapExpanded: boolean;
  setIsMapExpanded: React.Dispatch<React.SetStateAction<boolean>>;
  selectedItem: FeedItem | null;
  setSelectedItem: React.Dispatch<React.SetStateAction<FeedItem | null>>;
  onMapClick?: (coords: Coordinates) => void;
  newItemPos?: Coordinates | null;
}

const MapComponent: React.FC<MapComponentProps> = ({
  items,
  center,
  zoom,
  setZoom,
  setCenter,
  isMapExpanded,
  setIsMapExpanded,
  selectedItem,
  setSelectedItem,
  onMapClick,
  newItemPos,
}) => {
  const mapContainerRef = useRef<HTMLDivElement | null>(null);
  const mapInstanceRef = useRef<L.Map | null>(null);
  const markersRef = useRef<Map<number, L.Marker>>(new Map());
  const newMarkerRef = useRef<L.Marker | null>(null);
  const resizeTimeoutRef = useRef<NodeJS.Timeout | null>(null);
  const [isMapReady, setIsMapReady] = useState(false);

  // Inicializa o mapa
  useEffect(() => {
    if (!mapContainerRef.current || mapInstanceRef.current) return;

    // Cria instância do mapa
    const map = L.map(mapContainerRef.current, {
      center: [center.lat, center.lng],
      zoom: zoom,
      zoomControl: false,
      attributionControl: false,
    });

    // Adiciona tile layer (OpenStreetMap)
    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
      maxZoom: 19,
      attribution: '© OpenStreetMap contributors',
    }).addTo(map);

    // Listener para atualizar estado quando usuário move/zoomeia
    map.on("moveend", () => {
      const mapCenter = map.getCenter();
      setCenter({ lat: mapCenter.lat, lng: mapCenter.lng });
    });

    map.on("zoomend", () => {
      setZoom(map.getZoom());
    });

    // Listener para cliques no mapa
    map.on("click", (e: L.LeafletMouseEvent) => {
      if (isMapExpanded && onMapClick) {
        onMapClick({ lat: e.latlng.lat, lng: e.latlng.lng });
      }
    });

    mapInstanceRef.current = map;
    setIsMapReady(true);

    return () => {
      map.remove();
      mapInstanceRef.current = null;
      setIsMapReady(false);
    };
  }, []);

  // Atualiza centro e zoom quando props mudam
  useEffect(() => {
    if (!mapInstanceRef.current) return;
    
    const map = mapInstanceRef.current;
    const currentCenter = map.getCenter();
    const currentZoom = map.getZoom();

    if (
      Math.abs(currentCenter.lat - center.lat) > 0.0001 ||
      Math.abs(currentCenter.lng - center.lng) > 0.0001 ||
      currentZoom !== zoom
    ) {
      map.setView([center.lat, center.lng], zoom, { animate: true });
    }
  }, [center, zoom]);

  // Handler de redimensionamento responsivo
  useEffect(() => {
    if (!mapInstanceRef.current || !mapContainerRef.current) return;

    const handleResize = () => {
      if (resizeTimeoutRef.current) {
        clearTimeout(resizeTimeoutRef.current);
      }

      resizeTimeoutRef.current = setTimeout(() => {
        if (mapInstanceRef.current) {
          mapInstanceRef.current.invalidateSize();
        }
      }, 100);
    };

    // Observer para mudanças no container
    const resizeObserver = new ResizeObserver(handleResize);
    resizeObserver.observe(mapContainerRef.current);

    // Listener para resize da janela
    window.addEventListener("resize", handleResize);

    // Força invalidação quando expande/colapsa
    setTimeout(() => {
      if (mapInstanceRef.current) {
        mapInstanceRef.current.invalidateSize();
      }
    }, 650); // Após a animação CSS

    return () => {
      resizeObserver.disconnect();
      window.removeEventListener("resize", handleResize);
      if (resizeTimeoutRef.current) {
        clearTimeout(resizeTimeoutRef.current);
      }
    };
  }, [isMapExpanded]);

  // Gerencia marcadores dos items
  useEffect(() => {
    if (!mapInstanceRef.current || !isMapReady) return;

    const map = mapInstanceRef.current;

    // Remove marcadores que não existem mais
    markersRef.current.forEach((marker, id) => {
      if (!items.find((item) => item.id === id)) {
        marker.remove();
        markersRef.current.delete(id);
      }
    });

    // Adiciona ou atualiza marcadores
    items.forEach((item) => {
      let marker = markersRef.current.get(item.id);

      if (!marker) {
        // Cria ícone personalizado
        const icon = L.divIcon({
          className: "custom-marker-icon",
          html: `
            <svg width="24" height="24" viewBox="0 0 24 24" fill="#2563eb" stroke="#2563eb" stroke-width="2">
              <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path>
              <circle cx="12" cy="10" r="3" fill="white"></circle>
            </svg>
          `,
          iconSize: [24, 24],
          iconAnchor: [12, 24],
        });

        marker = L.marker([item.lat, item.lng], { icon })
          .addTo(map)
          .on("click", () => {
            if (isMapExpanded) {
              setSelectedItem(item);
            }
          });

        markersRef.current.set(item.id, marker);
      } else {
        // Atualiza posição se mudou
        const currentPos = marker.getLatLng();
        if (currentPos.lat !== item.lat || currentPos.lng !== item.lng) {
          marker.setLatLng([item.lat, item.lng]);
        }
      }

      // Atualiza estilo do marcador selecionado
      const isSelected = selectedItem?.id === item.id;
      const iconHtml = `
        <svg width="${isMapExpanded && zoom >= 15 ? '36' : '24'}" height="${isMapExpanded && zoom >= 15 ? '36' : '24'}" viewBox="0 0 24 24" fill="${isSelected ? '#dc2626' : '#2563eb'}" stroke="${isSelected ? '#dc2626' : '#2563eb'}" stroke-width="2">
          <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path>
          <circle cx="12" cy="10" r="3" fill="white"></circle>
        </svg>
      `;

      const newIcon = L.divIcon({
        className: "custom-marker-icon",
        html: iconHtml,
        iconSize: [isMapExpanded && zoom >= 15 ? 36 : 24, isMapExpanded && zoom >= 15 ? 36 : 24],
        iconAnchor: [isMapExpanded && zoom >= 15 ? 18 : 12, isMapExpanded && zoom >= 15 ? 36 : 24],
      });

      marker.setIcon(newIcon);
    });
  }, [items, isMapReady, isMapExpanded, selectedItem, zoom]);

  // Gerencia marcador do novo item
  useEffect(() => {
    if (!mapInstanceRef.current || !isMapReady) return;

    const map = mapInstanceRef.current;

    if (newItemPos) {
      if (!newMarkerRef.current) {
        const newIcon = L.divIcon({
          className: "custom-marker-icon new-marker",
          html: `
            <svg width="36" height="36" viewBox="0 0 24 24" fill="#16a34a" stroke="#16a34a" stroke-width="2">
              <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path>
              <circle cx="12" cy="10" r="3" fill="white"></circle>
            </svg>
          `,
          iconSize: [36, 36],
          iconAnchor: [18, 36],
        });

        newMarkerRef.current = L.marker([newItemPos.lat, newItemPos.lng], { icon }).addTo(map);
      } else {
        newMarkerRef.current.setLatLng([newItemPos.lat, newItemPos.lng]);
      }
    } else if (newMarkerRef.current) {
      newMarkerRef.current.remove();
      newMarkerRef.current = null;
    }
  }, [newItemPos, isMapReady]);

  const handleZoomIn = () => {
    if (mapInstanceRef.current) {
      mapInstanceRef.current.zoomIn();
    }
  };

  const handleZoomOut = () => {
    if (mapInstanceRef.current) {
      mapInstanceRef.current.zoomOut();
    }
  };

  return (
    <div
      className={`map-container ${isMapExpanded ? "expanded" : "collapsed"}`}
      onClick={!isMapExpanded ? () => setIsMapExpanded(true) : undefined}
    >
      <div ref={mapContainerRef} className="leaflet-map" />

      {/* Overlay minimizado */}
      {!isMapExpanded && (
        <div className="map-overlay">
          <div className="map-overlay-content">
            <Maximize2 size={16} />
            <span>Clique para expandir</span>
          </div>
        </div>
      )}

      {/* Controles */}
      {isMapExpanded && (
        <>
          <button className="btn-close" onClick={() => setIsMapExpanded(false)}>
            <X size={20} />
          </button>
          <div className="zoom-controls">
            <button onClick={handleZoomIn}>
              <ZoomIn size={20} />
            </button>
            <button onClick={handleZoomOut}>
              <ZoomOut size={20} />
            </button>
          </div>
        </>
      )}
    </div>
  );
};

export default MapComponent;