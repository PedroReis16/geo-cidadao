import React from 'react';
import "../../styles/components/Comments/CommentInput.css";

const CommentInput: React.FC = () => {
  return (
    <div className="post-input">
      <input
        type="text"
        placeholder="Adicione um comentário..."
        className="post-input__field"
      />
      <button className="post-input__submit">Publicar</button>
    </div>
  );
};

export default CommentInput;
