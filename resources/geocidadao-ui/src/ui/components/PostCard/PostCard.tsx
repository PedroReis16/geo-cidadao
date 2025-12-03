import React, { useState } from "react";
import { Heart, MessageCircle, MapPin } from "lucide-react";
import "../../styles/components/PostCard/PostCard.css";
import PostHeader from "./PostHeader";
import MediaCarousel from "./MediaCarousel";
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

  const hasMedia = post.media && post.media.length > 0;
  
  console.log('📰 PostCard:', { 
    id: post.id, 
    hasMedia, 
    mediaCount: post.media?.length,
    mediaTypes: post.media?.map(m => m.type),
    mediaUrls: post.media?.map(m => m.url.substring(m.url.length - 40))
  });

  const handleLike = () => onLike?.(post.id);
  const handleComment = () => onComment?.(post.id);
  const handleMap = () => onMap?.(post);

  return (
    <>
      <article className="post-card">
        <PostHeader user={post.author} timestamp={post.timestamp} />

        <div
          className={`post-text ${
            hasMedia ? "post-text--compact" : "post-text--expanded"
          }`}
        >
          {post.content}
        </div>

        {hasMedia && (
          <MediaCarousel
            media={post.media!}
            currentIndex={currentMediaIndex}
            onIndexChange={setCurrentMediaIndex}
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
            <span>{post.likesCount}</span>
          </button>

          <button onClick={handleComment} className="post-action-btn">
            <MessageCircle size={20} className="post-action-icon" />
            <span>{post.commentsCount}</span>
          </button>

          {post.coordinates && (
            <button onClick={handleMap} className="post-action-btn">
              <MapPin size={20} className="post-action-icon" />
            </button>
          )}
        </div>
      </article>
    </>
  );
};

export default PostCard;
