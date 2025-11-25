import React, { useRef, useCallback } from "react";
import "../styles/pages/FeedPage.css";
import FeedList from "../components/FeedList";
import { useMap } from "../../data/hooks/useMap";
import type { Post } from "../../data/@types/Post";

const FeedPage: React.FC = () => {
  const feedRef = useRef<HTMLDivElement>(null!);
  const { setPosts, setIsMapExpanded, isMapExpanded } = useMap();

  const handleMapItemClick = useCallback((post: Post) => {
    if (post.coordinates) {
      setIsMapExpanded(true);
      // Atualiza o contexto do mapa com o post clicado
      setPosts([post]);
    }
  }, [setIsMapExpanded, setPosts]);

  return (
    <div>
      <FeedList
        isMapExpanded={isMapExpanded}
        feedRef={feedRef}
        onMapItemClick={handleMapItemClick}
      />
    </div>
  );
};

export default FeedPage;
