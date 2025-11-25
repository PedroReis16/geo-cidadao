import React from "react";
import "../../styles/components/PostCard/PostHeader.css";
import type { Author } from "../../../data/@types/Author";
import { formatTimestamp } from "../../../utils/dateUtils";
import Avatar from "./Avatar";

interface PostHeaderProps {
  user?: Author;
  timestamp: string;
}

const PostHeader: React.FC<PostHeaderProps> = ({ user, timestamp }) => {
  // Valores padrão caso user seja undefined
  const userName = user?.name || "Usuário Desconhecido";
  const userUsername = user?.username || "desconhecido";
  const userProfilePicture = user?.profilePictureUrl;

  return (
    <div className="post-header">
      <Avatar 
        src={userProfilePicture} 
        name={userName}
        alt={`Foto de perfil de ${userName}`}
        className="post-header__avatar" 
      />
      <div className="post-header__info">
        <div className="post-header__name">{userName}</div>
        <div className="post-header__meta">
          @{userUsername} · {formatTimestamp(timestamp)}
        </div>
      </div>
    </div>
  );
};

export default PostHeader;
