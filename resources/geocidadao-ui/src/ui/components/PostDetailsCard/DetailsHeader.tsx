import React from "react";
import "../../styles/components/PostDetails/DetailsHeader.css";

interface DetailsHeaderProps {
    author: Author;
}

const DetailsHeader: React.FC<DetailsHeaderProps> = ({ author }) => {
  return (
    <div className="post-header">
      <div className="post-header__avatar">{author.name.charAt(0)}</div>
      <div className="post-header__info">
        <div className="post-header__name">{author.name}</div>
        <div className="post-header__username">{author.username}</div>
      </div>
      <button className="post-header__follow">Seguindo</button>
    </div>
  );
};

export default DetailsHeader;
