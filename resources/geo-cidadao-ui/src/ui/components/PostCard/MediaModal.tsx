import React, { useState } from "react";
import { ChevronLeft, ChevronRight, X } from "lucide-react";
import "../../styles/components/PostCard/MediaModal.css";
import type { MediaItem } from "../../../data/@types/MediaItem";

interface MediaModalProps {
  mediaItems: MediaItem[];
  currentIndex: number;
  onClose: () => void;
  onIndexChange: (index: number) => void;
}

const MediaModal: React.FC<MediaModalProps> = ({
  mediaItems,
  currentIndex,
  onClose,
  onIndexChange,
}) => {
  const [touchStart, setTouchStart] = useState(0);
  const [touchEnd, setTouchEnd] = useState(0);

  // Verificação de segurança
  if (!mediaItems || mediaItems.length === 0) {
    return null;
  }

  const nextMedia = () => {
    onIndexChange((currentIndex + 1) % mediaItems.length);
  };

  const prevMedia = () => {
    onIndexChange((currentIndex - 1 + mediaItems.length) % mediaItems.length);
  };

  const handleTouchStart = (e: React.TouchEvent) => {
    setTouchStart(e.targetTouches[0].clientX);
  };

  const handleTouchMove = (e: React.TouchEvent) => {
    setTouchEnd(e.targetTouches[0].clientX);
  };

  const handleTouchEnd = () => {
    if (touchStart - touchEnd > 50) nextMedia();
    if (touchStart - touchEnd < -50) prevMedia();
  };

  // Previne scroll do body quando modal está aberto

  return (
    <div
      className="media-modal"
      onTouchStart={handleTouchStart}
      onTouchMove={handleTouchMove}
      onTouchEnd={handleTouchEnd}
    >
      <button onClick={onClose} className="media-modal__close">
        <X size={24} />
      </button>

      <div className="media-modal__content">
        {mediaItems.map((item, index) => (
          <div
            key={index}
            className={`media-modal__item ${
              index === currentIndex ? "media-modal__item--active" : ""
            }`}
          >
            {item.type === "image" ? (
              <img
                src={item.url}
                alt="Expanded media"
                className="media-modal__image"
              />
            ) : (
              <video src={item.url} controls className="media-modal__video" />
            )}
          </div>
        ))}
      </div>

      {mediaItems.length > 1 && (
        <>
          <button
            onClick={prevMedia}
            className="media-modal__nav media-modal__nav--prev"
          >
            <ChevronLeft size={32} />
          </button>

          <button
            onClick={nextMedia}
            className="media-modal__nav media-modal__nav--next"
          >
            <ChevronRight size={32} />
          </button>

          <div className="media-modal__indicators">
            {mediaItems.map((_, index) => (
              <div
                key={index}
                className={`media-modal__indicator ${
                  index === currentIndex ? "media-modal__indicator--active" : ""
                }`}
              />
            ))}
          </div>

          <div className="media-modal__counter">
            {currentIndex + 1} / {mediaItems.length}
          </div>
        </>
      )}
    </div>
  );
};

export default MediaModal;
