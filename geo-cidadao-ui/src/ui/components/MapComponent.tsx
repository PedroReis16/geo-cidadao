import React, { useRef, useState } from "react";
import { MapPin, Maximize2, ZoomIn, ZoomOut, X } from "lucide-react";
import "../styles/components/MapComponent.css";


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

interface Position {
  x: number;
  y: number;
}

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
  const mapRef = useRef<HTMLDivElement | null>(null);
  const [isDragging, setIsDragging] = useState(false);
  const [dragStart, setDragStart] = useState<Position>({ x: 0, y: 0 });

  const MIN_ZOOM = 8;
  const MAX_ZOOM = 18;
  const SHOW_ITEMS_MIN_ZOOM = 13;

  const latLngToPixel = (lat: number, lng: number): Position => {
    if (!mapRef.current) return { x: 0, y: 0 };
    const mapWidth = mapRef.current.offsetWidth;
    const mapHeight = mapRef.current.offsetHeight;
    const scale = Math.pow(2, zoom);
    const worldWidth = 256 * scale;

    const x = (lng + 180) * (worldWidth / 360);
    const latRad = (lat * Math.PI) / 180;
    const mercN = Math.log(Math.tan(Math.PI / 4 + latRad / 2));
    const y = worldWidth / 2 - (worldWidth * mercN) / (2 * Math.PI);

    const centerX = (center.lng + 180) * (worldWidth / 360);
    const centerLatRad = (center.lat * Math.PI) / 180;
    const centerMercN = Math.log(Math.tan(Math.PI / 4 + centerLatRad / 2));
    const centerY = worldWidth / 2 - (worldWidth * centerMercN) / (2 * Math.PI);

    return {
      x: mapWidth / 2 + (x - centerX),
      y: mapHeight / 2 + (y - centerY),
    };
  };

  const pixelToLatLng = (x: number, y: number): Coordinates => {
    if (!mapRef.current) return { lat: 0, lng: 0 };
    const mapWidth = mapRef.current.offsetWidth;
    const mapHeight = mapRef.current.offsetHeight;
    const scale = Math.pow(2, zoom);
    const worldWidth = 256 * scale;

    const centerX = (center.lng + 180) * (worldWidth / 360);
    const centerLatRad = (center.lat * Math.PI) / 180;
    const centerMercN = Math.log(Math.tan(Math.PI / 4 + centerLatRad / 2));
    const centerY = worldWidth / 2 - (worldWidth * centerMercN) / (2 * Math.PI);

    const worldX = centerX + (x - mapWidth / 2);
    const worldY = centerY + (y - mapHeight / 2);

    const lng = (worldX / worldWidth) * 360 - 180;
    const mercN = (worldWidth / 2 - worldY) * (2 * Math.PI) / worldWidth;
    const latRad = 2 * Math.atan(Math.exp(mercN)) - Math.PI / 2;
    const lat = (latRad * 180) / Math.PI;

    return { lat, lng };
  };

  const handleWheel = (e: React.WheelEvent) => {
    e.preventDefault();
    const delta = e.deltaY > 0 ? -1 : 1;
    setZoom((prev) => Math.max(MIN_ZOOM, Math.min(MAX_ZOOM, prev + delta)));
  };

  const handleMouseDown = (e: React.MouseEvent) => {
    if (e.button === 0) {
      setIsDragging(true);
      setDragStart({ x: e.clientX, y: e.clientY });
    }
  };

  const handleMouseMove = (e: React.MouseEvent) => {
    if (!isDragging || !mapRef.current) return;
    const dx = e.clientX - dragStart.x;
    const dy = e.clientY - dragStart.y;
    const scale = Math.pow(2, zoom);
    const worldWidth = 256 * scale;

    const lngDelta = -(dx / mapRef.current.offsetWidth) * (360 / (worldWidth / 256));
    const latDelta = (dy / mapRef.current.offsetHeight) * (180 / (worldWidth / 256));

    setCenter((prev) => ({
      lat: Math.max(-85, Math.min(85, prev.lat + latDelta)),
      lng: ((prev.lng + lngDelta + 180) % 360) - 180,
    }));
    setDragStart({ x: e.clientX, y: e.clientY });
  };

  const handleMouseUp = () => setIsDragging(false);

  const handleMapClick = (e: React.MouseEvent) => {
    if (isDragging || !isMapExpanded || !mapRef.current || !onMapClick) return;
    const rect = mapRef.current.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;
    const coords = pixelToLatLng(x, y);
    onMapClick(coords);
  };

  const handleZoomIn = () => setZoom((prev) => Math.min(MAX_ZOOM, prev + 1));
  const handleZoomOut = () => setZoom((prev) => Math.max(MIN_ZOOM, prev - 1));

  return (
    <div
      ref={mapRef}
      className={`map-container ${isMapExpanded ? "expanded" : "collapsed"}`}
      onWheel={isMapExpanded ? handleWheel : undefined}
      onMouseDown={isMapExpanded ? handleMouseDown : undefined}
      onMouseMove={isMapExpanded ? handleMouseMove : undefined}
      onMouseUp={isMapExpanded ? handleMouseUp : undefined}
      onMouseLeave={isMapExpanded ? handleMouseUp : undefined}
      onClick={isMapExpanded ? handleMapClick : () => setIsMapExpanded(true)}
    >
      {/* Marcadores */}
      {zoom >= SHOW_ITEMS_MIN_ZOOM &&
        items.map((item) => {
          const pos = latLngToPixel(item.lat, item.lng);
          const isSelected = selectedItem?.id === item.id;
          const isVisible =
            pos.x >= -50 &&
            pos.x <= (mapRef.current?.offsetWidth || 0) + 50 &&
            pos.y >= -50 &&
            pos.y <= (mapRef.current?.offsetHeight || 0) + 50;

          if (!isVisible) return null;
          return (
            <div
              key={item.id}
              className="marker"
              style={{
                left: `${pos.x}px`,
                top: `${pos.y}px`,
                zIndex: isSelected ? 1000 : 10,
              }}
              onClick={(e) => {
                e.stopPropagation();
                if (isMapExpanded) setSelectedItem(item);
              }}
            >
              <MapPin
                size={isMapExpanded && zoom >= 15 ? 36 : 24}
                color={isSelected ? "#dc2626" : "#2563eb"}
                fill={isSelected ? "#dc2626" : "#2563eb"}
              />
            </div>
          );
        })}

      {/* Novo item */}
      {newItemPos && (
        <div
          className="marker new"
          style={{
            left: `${latLngToPixel(newItemPos.lat, newItemPos.lng).x}px`,
            top: `${latLngToPixel(newItemPos.lat, newItemPos.lng).y}px`,
          }}
        >
          <MapPin size={36} color="#16a34a" fill="#16a34a" />
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
            <button onClick={handleZoomIn} disabled={zoom >= MAX_ZOOM}>
              <ZoomIn size={20} />
            </button>
            <button onClick={handleZoomOut} disabled={zoom <= MIN_ZOOM}>
              <ZoomOut size={20} />
            </button>
          </div>
        </>
      )}
    </div>
  );
};

export default MapComponent;
