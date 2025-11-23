import React, { useState } from "react";
import { Heart, MessageCircle, MapPin } from "lucide-react";
import DetailsHeader from "./DetailsHeader";
import "../../styles/components/PostDetails/PostDetailsCard.css";
import type { Post } from "../../../data/@types/Post";
import CommentInput from "../Comments/CommentInput";
import CommentsList from "../Comments/CommentList";
import MediaCarousel from "../PostCard/MediaCarousel";

const PostDetailsCard: React.FC = () => {
  const [currentMediaIndex, setCurrentMediaIndex] = useState(0);
  const [isLiked, setIsLiked] = useState(false);
  const [likesCount, setLikesCount] = useState(120);

  const post: Post = {
    id: "1",
    author: {
      name: "João Silva",
      username: "@joao.silva",
      avatar: "https://via.placeholder.com/40",
    },
    content: "Estou explorando o novo parque da cidade!",
    timestamp: "2025-10-08T08:00:00Z",
    media: [
      {
        url: "https://images.unsplash.com/photo-1567942712330-dd0f81a4d736?w=800&h=600&fit=crop",
        type: "image",
      },
      {
        url: "https://images.unsplash.com/photo-1519331379826-f10be5486c6f?w=800&h=600&fit=crop",
        type: "image",
      },
      {
        url: "https://images.unsplash.com/photo-1441974231531-c6227db76b6e?w=800&h=600&fit=crop",
        type: "image",
      },
    ],
    coordinates: {
      lat: -23.55052,
      lng: -46.633308,
    },
    likes: 120,
  };

  const handleLike = () => {
    setIsLiked(!isLiked);
    setLikesCount(isLiked ? likesCount - 1 : likesCount + 1);
  };

  const getTimeAgo = (timestamp: string) => {
    const date = new Date(timestamp);
    const now = new Date();
    const diff = Math.floor((now.getTime() - date.getTime()) / 1000 / 60);
    if (diff < 60) return `${diff} min`;
    const hours = Math.floor(diff / 60);
    if (hours < 24) return `${hours}h`;
    return `${Math.floor(hours / 24)}d`;
  };

  return (
    <div className="post-details-card">
      {/* MediaCarrosel */}
      <MediaCarousel
        media={post.media!}
        currentIndex={currentMediaIndex}
        onIndexChange={(index) => setCurrentMediaIndex(index)}
        variant="details"
      />

      {/* Details Section */}
      <div className="post-details">
        {/* Header */}
        <DetailsHeader author={post.author} />

        {/* Content & Comments */}
        <div className="post-body">
          {/* Post Content */}
          <div className="post-content">
            <div className="post-content__avatar">
              {post.author.name.charAt(0)}
            </div>
            <div className="post-content__text">
              <div className="post-content__meta">
                <span className="post-content__username">
                  {post.author.username}
                </span>
                <span className="post-content__timestamp">
                  {getTimeAgo(post.timestamp)}
                </span>
              </div>
              <p className="post-content__message">{post.content}</p>
            </div>
          </div>

          {/* Comments */}
          <CommentsList postId={post.id} />
        </div>

        {/* Actions */}
        <div className="post-footer">
          <div className="post-actions">
            <button onClick={handleLike} className="post-actions__button">
              <Heart
                className={
                  "icon-md icon-heart" + (isLiked ? " icon-heart--liked" : "")
                }
              />
            </button>
            <button className="post-actions__button">
              <MessageCircle className="icon-md" />
            </button>

            <button className="post-actions__button">
              <MapPin className="icon-md" />
            </button>
          </div>

          <div className="post-likes">
            <span className="post-likes__count">
              {likesCount.toLocaleString()} curtidas
            </span>
          </div>

          {/* {post.location && ( */}
          <div className="post-location">
            <MapPin className="icon-sm" />
            <span>Nome do local</span>
          </div>
          {/* )} */}
          <div className="post-timestamp">
            <span>{getTimeAgo(post.timestamp)}</span>
          </div>

          {/* Comment Input */}
          <CommentInput />
        </div>
      </div>
    </div>
  );
};
export default PostDetailsCard;
