import React from "react";
import { MapPin, Heart, MessageCircle } from "lucide-react";
import type { FeedItem } from "../../data/@types/FeedItem";
import type { Coordinates } from "../../data/@types/Coordinates";

interface FeedPostProps {
  content: FeedItem;
  highlightedFeedItem: number | null;
  setCenter: React.Dispatch<React.SetStateAction<Coordinates>>;
  setZoom: React.Dispatch<React.SetStateAction<number>>;
  setSelectedItem: React.Dispatch<React.SetStateAction<FeedItem | null>>;
  setIsMapExpanded: React.Dispatch<React.SetStateAction<boolean>>;
}

const FeedPost: React.FC<FeedPostProps> = ({
  content,
  highlightedFeedItem,
  setCenter,
  setZoom,
  setSelectedItem,
  setIsMapExpanded,
}) => {
  const isHighlighted = highlightedFeedItem === content.id;

  const handleLocationClick = () => {
    setCenter({ lat: content.lat, lng: content.lng });
    setZoom(16);
    setSelectedItem(content);
    setIsMapExpanded(true);
  };

  return (
    <div
      id={`feed-item-${content.id}`}
      className={`feed-item ${isHighlighted ? "highlighted" : ""}`}
    >
      {content.image && (
        <img 
          src={content.image} 
          alt={content.title} 
          className="feed-item-image"
        />
      )}
      
      <div className="feed-item-body">
        <div className="feed-item-header">
          <h3 className="feed-item-title">{content.title}</h3>
          <button 
            className="feed-item-location" 
            onClick={handleLocationClick}
            title="Ver no mapa"
          >
            <MapPin size={16} />
          </button>
        </div>

        <p className="feed-item-author">Por {content.author}</p>
        <p className="feed-item-description">{content.description}</p>

        <div className="feed-item-actions">
          <button className="feed-action-btn">
            <Heart size={18} />
            <span>{content.likes}</span>
          </button>
          <button className="feed-action-btn">
            <MessageCircle size={18} />
            <span>{content.comments}</span>
          </button>
        </div>
      </div>
    </div>
  );
};

export default FeedPost;