import React, { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { Settings, MapPin, Calendar } from "lucide-react";
import { useUserProfile } from "../../data/hooks/useUserProfile";
import LoadingSpinner from "../components/LoadingSpinner";
import PostCard from "../components/PostCard/PostCard";
import EditProfileModal from "../components/EditProfileModal";
import Avatar from "../components/PostCard/Avatar";
import "../styles/pages/ProfilePage.css";
import type { UpdateUserProfileData } from "../../data/@types/UserProfile";
import type { Post } from "../../data/@types/Post";
import { formatDate } from "../../utils/dateUtils";

const ProfilePage: React.FC = () => {
  const { userId } = useParams<{ userId?: string }>();
  const navigate = useNavigate();
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const {
    profile,
    posts,
    loading,
    error,
    updating,
    updateError,
    updateProfile,
    isOwnProfile,
  } = useUserProfile({ userId });

  const handleSaveProfile = async (data: UpdateUserProfileData) => {
    try {
      await updateProfile(data);
      setIsEditModalOpen(false);
    } catch (err) {
      console.error("Erro ao salvar perfil:", err);
      // O erro já é tratado no hook
    }
  };

  const handlePostClick = (postId: string) => {
    navigate(`/post/${postId}`);
  };

  const handleMapClick = (post: Post) => {
    if (post.coordinates) {
      navigate("/feed", {
        state: {
          focusPost: post.id,
          coordinates: post.coordinates,
        },
      });
    }
  };

  if (loading) {
    return (
      <div className="profile-loading">
        <LoadingSpinner />
      </div>
    );
  }

  if (error || !profile) {
    return (
      <div className="profile-error">
        <h2>Erro ao carregar perfil</h2>
        <p>{error || "Perfil não encontrado"}</p>
        <button onClick={() => navigate("/feed")} className="btn-back">
          Voltar ao Feed
        </button>
      </div>
    );
  }

  return (
    <div className="profile-page">
      <div className="profile-container">
        {/* Cabeçalho do perfil */}
        <div className="profile-header">
          <div className="profile-cover"></div>

          <div className="profile-info-section">
            <div className="profile-avatar-wrapper">
              <Avatar
                src={profile.profilePictureUrl}
                name={profile.name}
                alt={`Foto de perfil de ${profile.name}`}
                className="profile-avatar"
                size="large"
              />
            </div>

            <div className="profile-details">
              <div className="profile-name-section">
                <h1 className="profile-name">{profile.name}</h1>
                {isOwnProfile && (
                  <button
                    className="btn-edit-profile"
                    onClick={() => setIsEditModalOpen(true)}
                  >
                    <Settings size={18} />
                    <span>Editar perfil</span>
                  </button>
                )}
              </div>

              <p className="profile-username">@{profile.username}</p>

              {profile.bio && <p className="profile-bio">{profile.bio}</p>}

              <div className="profile-meta">
                {profile.createdAt && (
                  <div className="profile-meta-item">
                    <Calendar size={16} />
                    <span>Membro desde {formatDate(profile.createdAt)}</span>
                  </div>
                )}
              </div>

              <div className="profile-stats">
                <div className="stat-item">
                  <span className="stat-value">{posts.length}</span>
                  <span className="stat-label">
                    {posts.length === 1 ? "Publicação" : "Publicações"}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Seção de postagens */}
        <div className="profile-posts-section">
          <h2 className="section-title">
            {isOwnProfile ? "Minhas Publicações" : "Publicações"}
          </h2>

          {posts.length === 0 ? (
            <div className="empty-posts">
              <MapPin size={48} />
              <p>
                {isOwnProfile
                  ? "Você ainda não fez nenhuma publicação"
                  : "Este usuário ainda não fez publicações"}
              </p>
            </div>
          ) : (
            <div className="profile-posts-list">
              {posts.map((post) => (
                <PostCard
                  key={post.id}
                  post={post}
                  onComment={() => handlePostClick(post.id)}
                  onMap={() => handleMapClick(post)}
                />
              ))}
            </div>
          )}
        </div>
      </div>

      {/* Modal de edição */}
      {isOwnProfile && (
        <EditProfileModal
          isOpen={isEditModalOpen}
          onClose={() => setIsEditModalOpen(false)}
          currentName={profile.name}
          currentProfilePicture={profile.profilePictureUrl}
          onSave={handleSaveProfile}
          isLoading={updating}
        />
      )}

      {updateError && (
        <div className="update-error-toast">
          <p>{updateError}</p>
        </div>
      )}
    </div>
  );
};

export default ProfilePage;
