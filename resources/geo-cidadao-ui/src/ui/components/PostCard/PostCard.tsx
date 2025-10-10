import React, { useState } from "react";
import { Heart, MessageCircle, MapPin } from "lucide-react";
import PostHeader from "./PostHeader";
import MediaCarousel from "./MediaCarousel";
import MediaModal from "./MediaModal";
import "../../../ui/styles/components/PostCard/PostCard.css";
import type { Post } from "../../../data/@types/Post";

interface PostCardProps {
  post: Post;
  onLike?: (postId: string) => void;
  onComment?: (postId: string) => void;
  onMap?: (post: Post) => void;
}

const PostCard: React.FC<PostCardProps> = ({
  post,
  onLike,
  onComment,
  onMap,
}) => {
  const [currentMediaIndex, setCurrentMediaIndex] = useState(0);
  const [isMediaExpanded, setIsMediaExpanded] = useState(false);

  const hasMedia = post.media && post.media.length > 0;

  const handleLike = () => onLike?.(post.id);
  const handleComment = () => onComment?.(post.id);
  const handleMap = () => onMap?.(post);

  return (
    <>
      <article className="post-card">
        <PostHeader user={post.user} timestamp={post.timestamp} />

        <div
          className={`post-text ${
            hasMedia ? "post-text--compact" : "post-text--expanded"
          }`}
        >
          {post.text}
        </div>

        {hasMedia && (
          <MediaCarousel
            media={post.media!}
            currentIndex={currentMediaIndex}
            onIndexChange={setCurrentMediaIndex}
            onMediaClick={() => setIsMediaExpanded(true)}
          />
        )}

        <div className="post-actions">
          <button
            onClick={handleLike}
            className={`post-action-btn ${
              post.isLiked ? "post-action-btn--liked" : ""
            }`}
          >
            <Heart
              size={20}
              fill={post.isLiked ? "var(--color-alert)" : "none"}
              className="post-action-icon"
            />
            <span>{post.likes}</span>
          </button>

          <button onClick={handleComment} className="post-action-btn">
            <MessageCircle size={20} className="post-action-icon" />
            <span>{post.comments}</span>
          </button>

          <button onClick={handleMap} className="post-action-btn">
            <MapPin size={20} className="post-action-icon" />
          </button>
        </div>
      </article>

      {isMediaExpanded && hasMedia && post.media && post.media.length > 0 && (
        <MediaModal
          mediaItems={post.media}
          currentIndex={currentMediaIndex}
          onClose={() => setIsMediaExpanded(false)}
          onIndexChange={setCurrentMediaIndex}
        />
      )}
    </>
  );
};

export default PostCard;
