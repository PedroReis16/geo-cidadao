import API_ENDPOINTS from "../../config/api";
import keycloak from "../../config/keycloak";
import type { UserProfile, UpdateUserProfileData } from "../@types/UserProfile";
import type { Post } from "../@types/Post";

/**
 * Busca o perfil do usuário autenticado
 */
export const getCurrentUserProfile = async (): Promise<UserProfile> => {
  const response = await fetch(
    `${API_ENDPOINTS.GERENCIAMENTO_USUARIOS}/usuarios/me`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${keycloak.token}`,
        "Content-Type": "application/json",
      },
    }
  );

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(
      `Erro ao buscar perfil do usuário: ${response.status} - ${errorText}`
    );
  }

  return await response.json();
};

/**
 * Busca o perfil de um usuário específico por ID
 */
export const getUserProfile = async (userId: string): Promise<UserProfile> => {
  const response = await fetch(
    `${API_ENDPOINTS.GERENCIAMENTO_USUARIOS}/usuarios/${userId}`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${keycloak.token}`,
        "Content-Type": "application/json",
      },
    }
  );

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(
      `Erro ao buscar perfil do usuário: ${response.status} - ${errorText}`
    );
  }

  return await response.json();
};

/**
 * Busca as postagens de um usuário específico
 */
export const getUserPosts = async (userId: string): Promise<Post[]> => {
  const response = await fetch(
    `${API_ENDPOINTS.GERENCIAMENTO_USUARIOS}/usuarios/${userId}/posts`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${keycloak.token}`,
        "Content-Type": "application/json",
      },
    }
  );

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(
      `Erro ao buscar posts do usuário: ${response.status} - ${errorText}`
    );
  }

  return await response.json();
};

/**
 * Atualiza o perfil do usuário
 */
export const updateUserProfile = async (
  data: UpdateUserProfileData
): Promise<UserProfile> => {
  const formData = new FormData();

  if (data.name) {
    formData.append("Name", data.name);
  }

  if (data.profilePicture) {
    formData.append("ProfilePicture", data.profilePicture);
  }

  const response = await fetch(
    `${API_ENDPOINTS.GERENCIAMENTO_USUARIOS}/usuarios/me`,
    {
      method: "PUT",
      headers: {
        Authorization: `Bearer ${keycloak.token}`,
      },
      body: formData,
    }
  );

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(
      `Erro ao atualizar perfil: ${response.status} - ${errorText}`
    );
  }

  return await response.json();
};
