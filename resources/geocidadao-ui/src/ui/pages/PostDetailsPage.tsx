import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useMap } from "../../data/hooks/useMap";
import LoadingSpinner from "../components/LoadingSpinner";
import "../styles/pages/PostDetailPage.css";
import type { Post } from "../../data/@types/Post";
import PostDetailsCard from "../components/PostDetailsCard/PostDetailsCard";

const PostDetailsPage: React.FC = () => {
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
      <main className="post-detail-main">
        <PostDetailsCard />
      </main>
    </div>
  );
};

export default PostDetailsPage;
