import { useState, useEffect, useCallback } from "react";
import {
  getUserProfile,
  getCurrentUserProfile,
  getUserPosts,
  updateUserProfile,
} from "../services/userService";
import type { UserProfile, UpdateUserProfileData } from "../@types/UserProfile";
import type { Post } from "../@types/Post";

interface UseUserProfileOptions {
  userId?: string; // Se não fornecido, busca o usuário autenticado
}

interface UseUserProfileReturn {
  profile: UserProfile | null;
  posts: Post[];
  loading: boolean;
  error: string | null;
  updating: boolean;
  updateError: string | null;
  refreshProfile: () => Promise<void>;
  refreshPosts: () => Promise<void>;
  updateProfile: (data: UpdateUserProfileData) => Promise<void>;
  isOwnProfile: boolean;
}

export const useUserProfile = (
  options: UseUserProfileOptions = {}
): UseUserProfileReturn => {
  const [profile, setProfile] = useState<UserProfile | null>(null);
  const [posts, setPosts] = useState<Post[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [updating, setUpdating] = useState(false);
  const [updateError, setUpdateError] = useState<string | null>(null);
  const [currentUserId, setCurrentUserId] = useState<string | null>(null);

  const { userId } = options;

  // Determina se é o próprio perfil
  const isOwnProfile = !userId || userId === currentUserId;

  // Busca o perfil do usuário
  const fetchProfile = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      let profileData: UserProfile;

      if (!userId) {
        // Busca o perfil do usuário autenticado
        profileData = await getCurrentUserProfile();
        setCurrentUserId(profileData.id);
      } else {
        // Busca o perfil de um usuário específico
        profileData = await getUserProfile(userId);
      }

      setProfile(profileData);
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Erro ao carregar perfil"
      );
      console.error("Erro ao buscar perfil:", err);
    } finally {
      setLoading(false);
    }
  }, [userId]);

  // Busca as postagens do usuário
  const fetchPosts = useCallback(async () => {
    if (!profile?.id) return;

    try {
      const postsData = await getUserPosts(profile.id);
      setPosts(postsData);
    } catch (err) {
      console.error("Erro ao buscar posts do usuário:", err);
      // Não define erro aqui para não sobrescrever erro do perfil
    }
  }, [profile?.id]);

  // Atualiza o perfil
  const handleUpdateProfile = useCallback(
    async (data: UpdateUserProfileData) => {
      try {
        setUpdating(true);
        setUpdateError(null);

        const updatedProfile = await updateUserProfile(data);
        setProfile(updatedProfile);
      } catch (err) {
        const errorMessage =
          err instanceof Error ? err.message : "Erro ao atualizar perfil";
        setUpdateError(errorMessage);
        throw new Error(errorMessage);
      } finally {
        setUpdating(false);
      }
    },
    []
  );

  // Carrega o perfil ao montar o componente
  useEffect(() => {
    fetchProfile();
  }, [fetchProfile]);

  // Carrega os posts quando o perfil for carregado
  useEffect(() => {
    if (profile) {
      fetchPosts();
    }
  }, [profile, fetchPosts]);

  return {
    profile,
    posts,
    loading,
    error,
    updating,
    updateError,
    refreshProfile: fetchProfile,
    refreshPosts: fetchPosts,
    updateProfile: handleUpdateProfile,
    isOwnProfile,
  };
};
