import React from "react";
import { X } from "lucide-react";
import "../../styles/components/PostCard/MediaModal.css";
import type { MediaItem } from "../../../data/@types/MediaItem";


interface MediaModalProps {
  media: MediaItem;
  onClose: () => void;
}

const MediaModal: React.FC<MediaModalProps> = ({ media, onClose }) => {
  return (
    <div className="media-modal" onClick={onClose}>
      <button onClick={onClose} className="media-modal__close">
        <X size={24} />
      </button>

      <div className="media-modal__content">
        {media.type === "image" ? (
          <img
            src={media.url}
            alt="Expanded media"
            className="media-modal__image"
            onClick={(e) => e.stopPropagation()}
          />
        ) : (
          <video
            src={media.url}
            controls
            className="media-modal__video"
            onClick={(e) => e.stopPropagation()}
          />
        )}
      </div>
    </div>
  );
};

export default MediaModal;
