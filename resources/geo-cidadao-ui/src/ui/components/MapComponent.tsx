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
  onItemPreviewClick?: (item: FeedItem) => void; // Callback para abrir detalhes
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
  onItemPreviewClick,
}) => {
  const mapContainerRef = useRef<HTMLDivElement | null>(null);
  const mapInstanceRef = useRef<L.Map | null>(null);
  const markersRef = useRef<Map<number, L.Marker>>(new Map());
  const newMarkerRef = useRef<L.Marker | null>(null);
  const resizeTimeoutRef = useRef<NodeJS.Timeout | null>(null);
  const popupRef = useRef<L.Popup | null>(null);
  const [isMapReady, setIsMapReady] = useState(false);
  const [visibleItems, setVisibleItems] = useState<FeedItem[]>([]);

  const MIN_ZOOM_TO_SHOW_ITEMS = 13; // Zoom mínimo para mostrar marcadores
  const MAX_DISTANCE_KM = 10; // Distância máxima em km do centro para mostrar items

  // Calcula distância entre dois pontos (fórmula de Haversine)
  const calculateDistance = (lat1: number, lng1: number, lat2: number, lng2: number): number => {
    const R = 6371; // Raio da Terra em km
    const dLat = ((lat2 - lat1) * Math.PI) / 180;
    const dLng = ((lng2 - lng1) * Math.PI) / 180;
    const a =
      Math.sin(dLat / 2) * Math.sin(dLat / 2) +
      Math.cos((lat1 * Math.PI) / 180) *
        Math.cos((lat2 * Math.PI) / 180) *
        Math.sin(dLng / 2) *
        Math.sin(dLng / 2);
    const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    return R * c;
  };

  // Filtra items visíveis baseado em zoom e distância
  useEffect(() => {
    if (zoom < MIN_ZOOM_TO_SHOW_ITEMS) {
      setVisibleItems([]);
      return;
    }

    // Ajusta distância máxima baseado no zoom
    const zoomFactor = Math.max(0.5, (zoom - MIN_ZOOM_TO_SHOW_ITEMS) / 5);
    const maxDistance = MAX_DISTANCE_KM * (2 - zoomFactor);

    const filtered = items.filter((item) => {
      const distance = calculateDistance(center.lat, center.lng, item.lat, item.lng);
      return distance <= maxDistance;
    });

    setVisibleItems(filtered);
  }, [items, center, zoom]);

  // Cria o HTML do popup personalizado
  const createPopupContent = (item: FeedItem): HTMLDivElement => {
    const container = document.createElement("div");
    container.className = "custom-popup";
    
    container.innerHTML = `
      <div class="popup-content">
        ${item.image ? `<img src="${item.image}" alt="${item.title}" class="popup-image" />` : ''}
        <div class="popup-body">
          <h3 class="popup-title">${item.title}</h3>
          <p class="popup-author">Por ${item.author}</p>
          <p class="popup-description">${item.description}</p>
          <div class="popup-stats">
            <span class="popup-stat">
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"></path>
              </svg>
              ${item.likes}
            </span>
            <span class="popup-stat">
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"></path>
              </svg>
              ${item.comments}
            </span>
          </div>
          <button class="popup-button" data-item-id="${item.id}">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6"></path>
              <polyline points="15 3 21 3 21 9"></polyline>
              <line x1="10" y1="14" x2="21" y2="3"></line>
            </svg>
            Ver detalhes
          </button>
        </div>
      </div>
    `;

    // Adiciona listener ao botão
    const button = container.querySelector('.popup-button');
    if (button) {
      button.addEventListener('click', (e) => {
        e.stopPropagation();
        if (onItemPreviewClick) {
          onItemPreviewClick(item);
        }
      });
    }

    return container;
  };

  // Inicializa o mapa
  useEffect(() => {
    if (!mapContainerRef.current || mapInstanceRef.current) return;

    const map = L.map(mapContainerRef.current, {
      center: [center.lat, center.lng],
      zoom: zoom,
      zoomControl: false,
      attributionControl: false,
    });

    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
      maxZoom: 19,
      attribution: '© OpenStreetMap contributors',
    }).addTo(map);

    map.on("moveend", () => {
      const mapCenter = map.getCenter();
      setCenter({ lat: mapCenter.lat, lng: mapCenter.lng });
    });

    map.on("zoomend", () => {
      setZoom(map.getZoom());
    });

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

    const resizeObserver = new ResizeObserver(handleResize);
    resizeObserver.observe(mapContainerRef.current);

    window.addEventListener("resize", handleResize);

    setTimeout(() => {
      if (mapInstanceRef.current) {
        mapInstanceRef.current.invalidateSize();
      }
    }, 650);

    return () => {
      resizeObserver.disconnect();
      window.removeEventListener("resize", handleResize);
      if (resizeTimeoutRef.current) {
        clearTimeout(resizeTimeoutRef.current);
      }
    };
  }, [isMapExpanded]);

  // Gerencia marcadores dos items visíveis
  useEffect(() => {
    if (!mapInstanceRef.current || !isMapReady) return;

    const map = mapInstanceRef.current;

    // Remove todos os marcadores que não estão mais visíveis
    markersRef.current.forEach((marker, id) => {
      if (!visibleItems.find((item) => item.id === id)) {
        marker.remove();
        markersRef.current.delete(id);
      }
    });

    // Adiciona ou atualiza marcadores visíveis
    visibleItems.forEach((item) => {
      let marker = markersRef.current.get(item.id);

      if (!marker) {
        const isSelected = selectedItem?.id === item.id;
        const icon = L.divIcon({
          className: "custom-marker-icon",
          html: `
            <svg width="${isMapExpanded && zoom >= 15 ? '36' : '24'}" height="${isMapExpanded && zoom >= 15 ? '36' : '24'}" viewBox="0 0 24 24" fill="${isSelected ? '#dc2626' : '#2563eb'}" stroke="${isSelected ? '#dc2626' : '#2563eb'}" stroke-width="2">
              <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path>
              <circle cx="12" cy="10" r="3" fill="white"></circle>
            </svg>
          `,
          iconSize: [isMapExpanded && zoom >= 15 ? 36 : 24, isMapExpanded && zoom >= 15 ? 36 : 24],
          iconAnchor: [isMapExpanded && zoom >= 15 ? 18 : 12, isMapExpanded && zoom >= 15 ? 36 : 24],
        });

        marker = L.marker([item.lat, item.lng], { icon })
          .addTo(map)
          .on("click", () => {
            if (isMapExpanded) {
              setSelectedItem(item);
              
              // Cria e abre popup personalizado
              const popupContent = createPopupContent(item);
              const popup = L.popup({
                maxWidth: 300,
                className: 'custom-leaflet-popup'
              })
                .setLatLng([item.lat, item.lng])
                .setContent(popupContent)
                .openOn(map);
              
              popupRef.current = popup;
            }
          });

        markersRef.current.set(item.id, marker);
      } else {
        const currentPos = marker.getLatLng();
        if (currentPos.lat !== item.lat || currentPos.lng !== item.lng) {
          marker.setLatLng([item.lat, item.lng]);
        }
      }

      // Atualiza ícone do marcador selecionado
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
  }, [visibleItems, isMapReady, isMapExpanded, selectedItem, zoom]);

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

      {/* Indicador de zoom mínimo */}
      {isMapExpanded && zoom < MIN_ZOOM_TO_SHOW_ITEMS && (
        <div className="zoom-info">
          <ZoomIn size={20} />
          <span>Aproxime mais para ver os pontos</span>
        </div>
      )}

      {/* Contador de items visíveis */}
      {isMapExpanded && zoom >= MIN_ZOOM_TO_SHOW_ITEMS && (
        <div className="items-counter">
          <MapPin size={16} />
          <span>{visibleItems.length} {visibleItems.length === 1 ? 'ponto' : 'pontos'} visíveis</span>
        </div>
      )}

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