import React, { useState } from "react";
import { ChevronLeft, ChevronRight } from "lucide-react";
import "../../styles/components/PostCard/MediaCarousel.css";
import type { MediaItem } from "../../../data/@types/MediaItem";

interface MediaCarouselProps {
  media: MediaItem[];
  currentIndex: number;
  onIndexChange: (index: number) => void;
  onMediaClick?: () => void;
  variant?: "default" | "details"; // Nova prop
}

const MediaCarousel: React.FC<MediaCarouselProps> = ({
  media,
  currentIndex,
  onIndexChange,
  onMediaClick,
  variant = "default",
}) => {
  const [touchStart, setTouchStart] = useState(0);
  const [touchEnd, setTouchEnd] = useState(0);

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
      {media.map((item, index) => (
        <div
          key={index}
          className={`media-carousel__item ${
            index === currentIndex ? "media-carousel__item--active" : ""
          }`}
          onClick={onMediaClick ? () => onMediaClick() : undefined}
        >
          {item.type === "image" ? (
            <img
              src={item.url}
              alt="Post media"
              className="media-carousel__image"
            />
          ) : (
            <video src={item.url} controls className="media-carousel__video" />
          )}
        </div>
      ))}

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
