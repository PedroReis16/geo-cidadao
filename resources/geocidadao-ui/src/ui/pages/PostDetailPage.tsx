import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useMap } from "../../data/hooks/useMap";
import PostCard from "../components/PostCard/PostCard";
import LoadingSpinner from "../components/LoadingSpinner";
import "../styles/pages/PostDetailPage.css";
import type { Post } from "../../data/@types/Post";

const PostDetailPage: React.FC = () => {
  const { postId } = useParams<{ postId: string }>();
  const navigate = useNavigate();
  const { posts, setIsMapExpanded } = useMap();
  const [post, setPost] = useState<Post | null>(null);
  const [loading, setLoading] = useState(true);

  // Busca o post nos dados carregados ou faz uma requisição
  useEffect(() => {
    if (!postId) {
      navigate("/feed");
      return;
    }

    // Primeiro tenta encontrar nos posts já carregados
    const foundPost = posts.find((p) => p.id === postId);

    const timer = setTimeout(() => {
      if (foundPost) {
        setPost(foundPost);
        setLoading(false);
      } else {
        // TODO: Buscar post da API
        // Por enquanto, redireciona para o feed se não encontrar
        setLoading(false);
        navigate("/feed");
      }
    }, 0);

    return () => clearTimeout(timer);
  }, [postId, posts, navigate]);

  const handleMapClick = (clickedPost: Post) => {
    if (clickedPost.coordinates) {
      setIsMapExpanded(true);
    }
  };

  const handleBackToFeed = () => {
    navigate("/feed");
  };

  if (loading) {
    return (
      <div className="post-detail-page loading">
        <LoadingSpinner />
      </div>
    );
  }

  if (!post) {
    return (
      <div className="post-detail-page not-found">
        <h2>Post não encontrado</h2>
        <button onClick={handleBackToFeed} className="back-button">
          Voltar ao Feed
        </button>
      </div>
    );
  }

  return (
    <div className="post-detail-page">
      <header className="post-detail-header">
        <button onClick={handleBackToFeed} className="back-button">
          <svg
            width="20"
            height="20"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
          >
            <path d="M19 12H5M12 19l-7-7 7-7" />
          </svg>
          Voltar
        </button>
        <h1>Detalhes da Publicação</h1>
      </header>

      <main className="post-detail-main">
        <div className="post-detail-wrapper">
          <PostCard post={post} onMap={handleMapClick} />

          {/* Seção de comentários futura */}
          <div className="post-comments-section">
            <h3>Comentários ({post.comments})</h3>
            <div className="comments-placeholder">
              <p>Seção de comentários em desenvolvimento...</p>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
};

export default PostDetailPage;
