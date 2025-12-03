export interface UserProfile {
  id: string;
  name: string;
  username: string;
  profilePictureUrl?: string;
  bio?: string;
  email?: string;
  createdAt?: string;
}

export interface UpdateUserProfileData {
  name?: string;
  profilePicture?: File;
}
