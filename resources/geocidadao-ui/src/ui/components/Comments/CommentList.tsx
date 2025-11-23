import React from "react";
import type { Comment } from "../../../data/@types/Comment";
import PostComment from "./PostComment";

interface CommentsListProps {
  postId: string;
}

const CommentsList: React.FC<CommentsListProps> = ({ postId }) => {
  const comments: Comment[] = [
    {
      id: "1",
      postId: postId,
      author: {
        name: "Maria Oliveira",
        username: "@maria.oliveira",
        avatar: "https://via.placeholder.com/40",
      },
      content: "Que lugar incrível! Adoraria visitar.",
      createdAt: "2025-10-08T09:00:00Z",
      likes: 15,
      timestamp: "2 horas atrás",
    },
    {
      id: "2",
      postId: postId,
      author: {
        name: "Pedro Henrique",
        username: "@pedro.henrique",
        avatar: "https://via.placeholder.com/40",
      },
      content: "Que lugar incrível! Adoraria visitar.",
      createdAt: "2025-10-08T09:00:00Z",
      likes: 15,
      timestamp: "2 horas atrás",
    },
    {
      id: "3",
      postId: postId,
      author: {
        name: "Maria Oliveira",
        username: "@maria.oliveira",
        avatar: "https://via.placeholder.com/40",
      },
      content: "Que lugar incrível! Adoraria visitar.",
      createdAt: "2025-10-08T09:00:00Z",
      likes: 15,
      timestamp: "2 horas atrás",
    },
  ];

  return (
    <div className="post-comments">
      {comments.map((comment) => (
        <PostComment key={comment.id} comment={comment} />
      ))}
    </div>
  );
};

export default CommentsList;
