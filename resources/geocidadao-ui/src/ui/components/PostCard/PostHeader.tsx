import React from "react";
import "../../styles/components/PostCard/PostHeader.css";
import type { User } from "../../../data/@types/User";

interface PostHeaderProps {
  user: User;
  timestamp: string;
}

const PostHeader: React.FC<PostHeaderProps> = ({ user, timestamp }) => {
  return (
    <div className="post-header">
      <img src={user.avatar} alt={user.name} className="post-header__avatar" />
      <div className="post-header__info">
        <div className="post-header__name">{user.name}</div>
        <div className="post-header__meta">
          @{user.username} · {timestamp}
        </div>
      </div>
    </div>
  );
};

export default PostHeader;
