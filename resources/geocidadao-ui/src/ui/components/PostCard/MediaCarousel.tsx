import React, { useState } from "react";
import { ChevronLeft, ChevronRight } from "lucide-react";
import "../../styles/components/PostCard/MediaCarousel.css";
import type { MediaItem } from "../../../data/@types/MediaItem";
import LazyMedia from "../LazyMedia";

interface MediaCarouselProps {
  media: MediaItem[];
  currentIndex: number;
  onIndexChange: (index: number) => void;
  variant?: "default" | "details"; // Nova prop
}

const MediaCarousel: React.FC<MediaCarouselProps> = ({
  media,
  currentIndex,
  onIndexChange,
  variant = "default",
}) => {
  const [touchStart, setTouchStart] = useState(0);
  const [touchEnd, setTouchEnd] = useState(0);
  
  console.log('🎠 MediaCarousel:', { 
    mediaCount: media.length, 
    currentIndex,
    media: media.map(m => ({
      type: m.type,
      urlEnd: m.url.substring(m.url.length - 30)
    }))
  });

  const nextMedia = () => {
    onIndexChange((currentIndex + 1) % media.length);
  };

  const prevMedia = () => {
    onIndexChange((currentIndex - 1 + media.length) % media.length);
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

  return (
    <div
      className={`media-carousel media-carousel--${variant}`}
      onTouchStart={handleTouchStart}
      onTouchMove={handleTouchMove}
      onTouchEnd={handleTouchEnd}
    >
      {media.map((item, index) => {
        // A mídia atual deve ser sempre carregada
        const isCurrentMedia = index === currentIndex;
        
        // Pré-carrega as mídias adjacentes (anterior e próxima)
        const isAdjacentMedia = 
          index === currentIndex - 1 || 
          index === currentIndex + 1 ||
          (currentIndex === 0 && index === media.length - 1) ||
          (currentIndex === media.length - 1 && index === 0);

        return (
          <div
            key={index}
            className={`media-carousel__item ${
              isCurrentMedia ? "media-carousel__item--active" : ""
            }`}
          >
            <LazyMedia
              type={item.type}
              url={item.url}
              isActive={isCurrentMedia}
              shouldPreload={isAdjacentMedia}
              alt={`Post media ${index + 1}`}
              className={item.type === "image" ? "media-carousel__image" : "media-carousel__video"}
            />
          </div>
        );
      })}

      {media.length > 1 && (
        <>
          <button
            onClick={(e) => {
              e.stopPropagation();
              prevMedia();
            }}
            className="media-carousel__btn media-carousel__btn--prev"
          >
            <ChevronLeft size={20} />
          </button>

          <button
            onClick={(e) => {
              e.stopPropagation();
              nextMedia();
            }}
            className="media-carousel__btn media-carousel__btn--next"
          >
            <ChevronRight size={20} />
          </button>

          <div className="media-carousel__indicators">
            {media.map((_, index) => (
              <div
                key={index}
                className={`media-carousel__indicator ${
                  index === currentIndex
                    ? "media-carousel__indicator--active"
                    : ""
                }`}
              />
            ))}
          </div>
        </>
      )}
    </div>
  );
};

export default MediaCarousel;
