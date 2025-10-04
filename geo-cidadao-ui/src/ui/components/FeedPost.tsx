import { Bookmark, Heart, MapPin, MessageCircle, Share2 } from "lucide-react";
import type React from "react";
import type { FeedItem } from "../../data/@types/FeedItem";
import "../styles/components/FeedPost.css";

type FeedPostProps = {
  content: FeedItem;
  highlightedFeedItem: string | number;
  setCenter: (center: { lat: number; lng: number }) => void;
  setZoom: (zoom: number) => void;
  setSelectedItem: (item: FeedItem) => void;
  setIsMapExpanded: (expanded: boolean) => void;
};

const FeedPost: React.FC<FeedPostProps> = ({
  content,
  highlightedFeedItem,
  setCenter,
  setZoom,
  setSelectedItem,
  setIsMapExpanded,
}) => {
  return (
    <>
      <div
        key={content.id}
        id={`feed-item-${content.id}`}
        className={`feed-item ${
          highlightedFeedItem === content.id ? "highlight" : ""
        }`}
      >
        <div className="feed-item-header">
          <div className="feed-item-author">
            <div className="avatar">{content.author[0]}</div>
            <div>
              <h3>{content.author}</h3>
              <p>{content.title}</p>
            </div>
          </div>
          <button
            className="see-map-btn"
            onClick={() => {
              setCenter({ lat: content.lat, lng: content.lng });
              setZoom(16);
              setSelectedItem(content);
              setIsMapExpanded(true);
            }}
          >
            <MapPin size={14} /> Ver no mapa
          </button>
        </div>

        <div className="feed-item-content">
          <p>{content.description}</p>
          <small>
            {content.lat.toFixed(4)}, {content.lng.toFixed(4)}
          </small>
        </div>

        <div className="feed-item-actions">
          <button>
            <Heart size={20} /> {content.likes}
          </button>
          <button>
            <MessageCircle size={20} /> {content.comments}
          </button>
          <button>
            <Share2 size={20} />
          </button>
          <button>
            <Bookmark size={20} />
          </button>
        </div>
      </div>
    </>
  );
};

export default FeedPost;
