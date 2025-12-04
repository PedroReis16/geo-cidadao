import React from "react";
import { useNavigate } from "react-router-dom";
import "../../styles/components/PostCard/PostHeader.css";
import type { Author } from "../../../data/@types/Author";
import { formatTimestamp } from "../../../utils/dateUtils";
import Avatar from "./Avatar";

interface PostHeaderProps {
  user?: Author;
  timestamp: string;
}

const PostHeader: React.FC<PostHeaderProps> = ({ user, timestamp }) => {
  const navigate = useNavigate();

  // Valores padrão caso user seja undefined
  const userName = user?.name || "Usuário Desconhecido";
  const userUsername = user?.username || "desconhecido";
  const userProfilePicture = user?.profilePictureUrl;
  const userId = user?.id;

  const handleProfileClick = () => {
    if (userId) {
      navigate(`/profile/${userId}`);
    }
  };

  return (
    <div className="post-header">
      <div 
        className="post-header__avatar-wrapper"
        onClick={handleProfileClick}
        role="button"
        tabIndex={0}
        style={{ cursor: userId ? 'pointer' : 'default' }}
      >
        <Avatar 
          src={userProfilePicture} 
          name={userName}
          alt={`Foto de perfil de ${userName}`}
          className="post-header__avatar" 
        />
      </div>
      <div className="post-header__info">
        <div 
          className="post-header__name"
          onClick={handleProfileClick}
          role="button"
          tabIndex={0}
          style={{ cursor: userId ? 'pointer' : 'default' }}
        >
          {userName}
        </div>
        <div className="post-header__meta">
          @{userUsername} · {formatTimestamp(timestamp)}
        </div>
      </div>
    </div>
  );
};

export default PostHeader;
