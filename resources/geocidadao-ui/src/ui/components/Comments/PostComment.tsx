import React from "react";
import "../../styles/components/Comments/PostComment.css";
import type { Comment } from "../../../data/@types/Comment";

interface CommentProps {
  comment: Comment;
}

const PostComment: React.FC<CommentProps> = ({ comment }) => {
  return (
    <div key={comment.id} className="comment">
      <div className="comment__avatar">
        {comment.author.name.charAt(0).toUpperCase()}
      </div>
      <div className="comment__body">
        <div className="comment__meta">
          <span className="comment__author">{comment.author.name}</span>
          <span className="comment__timestamp">{comment.timestamp}</span>
        </div>
        <p className="comment__text">{comment.content}</p>
        <div className="comment__actions">
          <button className="comment__action">{comment.likes} curtidas</button>
          <button className="comment__action">Responder</button>
        </div>
      </div>
    </div>
  );
};

export default PostComment;
