import { useEffect, useState } from "react";
import "../styles/components/FeedList.css";
import PostCard from "./PostCard/PostCard";
import type { Post } from "../../data/@types/Post";
import PostCreator from "./PostCreator";

interface FeedListProps {
  isMapExpanded: boolean;
  feedRef: React.RefObject<HTMLDivElement>;
  items?: Post[];
  onMapItemClick?: (post: Post) => void;
}

const FeedList: React.FC<FeedListProps> = ({
  isMapExpanded,
  feedRef,
  items,
  onMapItemClick,
}) => {
  const [isMobile, setIsMobile] = useState<boolean>(false);

  useEffect(() => {
    const checkMobile = () => setIsMobile(window.innerWidth < 768);
    checkMobile();
    window.addEventListener("resize", checkMobile);
    return () => window.removeEventListener("resize", checkMobile);
  }, []);

  return (
    <div
      ref={feedRef}
      className={`feed-list ${
        isMapExpanded && isMobile ? "fade-out" : "fade-in"
      } `}
    >
      <PostCreator />
      
      {items && items.length > 0 ? (
        items.map((item: Post) => (
          <PostCard
            key={item.id}
            post={item}
            onMap={onMapItemClick}
            //   onLike={handlePostLike}
            //   onComment={loadPostComments}
          />
        ))
      ) : (
        <div className="no-posts">Nenhum post disponível.</div>
      )}
    </div>
  );
};

export default FeedList;
