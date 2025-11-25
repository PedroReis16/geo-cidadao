import API_ENDPOINTS from "../../config/api";
import keycloak from "../../config/keycloak";

export interface CreatePostData {
  content: string;
  latitude?: number;
  longitude?: number;
  mediaFiles?: File[];
}

export const createPost = async (data: CreatePostData): Promise<any> => {
  const formData = new FormData();

  formData.append("Content", data.content);

  if (data.latitude !== undefined && data.longitude !== undefined) {
    formData.append("Latitude", data.latitude.toString());
    formData.append("Longitude", data.longitude.toString());
  }

  if (data.mediaFiles && data.mediaFiles.length > 0) {
    data.mediaFiles.forEach((file) => {
      formData.append("MediaFiles", file);
    });
  }

  const response = await fetch(`${API_ENDPOINTS.GERENCIAMENTO_POSTS}/Posts`, {
    method: "POST",
    headers: {
      Authorization: `Bearer ${keycloak.token}`,
    },
    body: formData,
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Erro ao criar post: ${response.status} - ${errorText}`);
  }

  return await response.json();
};
