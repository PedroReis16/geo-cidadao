import React, { useState, useRef } from "react";
import { X, Camera, User } from "lucide-react";
import "../../styles/components/EditProfileModal.css";
import type { UpdateUserProfileData } from "../../../data/@types/UserProfile";
import LoadingSpinner from "../LoadingSpinner";

interface EditProfileModalProps {
  isOpen: boolean;
  onClose: () => void;
  currentName: string;
  currentProfilePicture?: string;
  onSave: (data: UpdateUserProfileData) => Promise<void>;
  isLoading?: boolean;
}

const EditProfileModal: React.FC<EditProfileModalProps> = ({
  isOpen,
  onClose,
  currentName,
  currentProfilePicture,
  onSave,
  isLoading = false,
}) => {
  const [name, setName] = useState(currentName);
  const [previewImage, setPreviewImage] = useState<string | undefined>(
    currentProfilePicture
  );
  const [selectedFile, setSelectedFile] = useState<File | undefined>();
  const fileInputRef = useRef<HTMLInputElement>(null);

  if (!isOpen) return null;

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setSelectedFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setPreviewImage(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const updateData: UpdateUserProfileData = {};

    if (name !== currentName) {
      updateData.name = name;
    }

    if (selectedFile) {
      updateData.profilePicture = selectedFile;
    }

    // Só envia se houver alterações
    if (Object.keys(updateData).length > 0) {
      await onSave(updateData);
    }

    onClose();
  };

  const handleOverlayClick = (e: React.MouseEvent) => {
    if (e.target === e.currentTarget) {
      onClose();
    }
  };

  return (
    <div className="modal-overlay" onClick={handleOverlayClick}>
      <div className="modal-content">
        <div className="modal-header">
          <h2 className="modal-title">Editar Perfil</h2>
          <button
            className="modal-close-btn"
            onClick={onClose}
            disabled={isLoading}
            aria-label="Fechar"
          >
            <X size={24} />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="edit-profile-form">
          <div className="profile-picture-section">
            <div className="profile-picture-container">
              {previewImage ? (
                <img
                  src={previewImage}
                  alt="Preview do perfil"
                  className="profile-picture-preview"
                />
              ) : (
                <div className="profile-picture-placeholder">
                  <User size={64} />
                </div>
              )}
              <button
                type="button"
                className="change-picture-btn"
                onClick={() => fileInputRef.current?.click()}
                disabled={isLoading}
              >
                <Camera size={20} />
                <span>Alterar foto</span>
              </button>
            </div>
            <input
              ref={fileInputRef}
              type="file"
              accept="image/*"
              onChange={handleImageChange}
              className="file-input"
              disabled={isLoading}
            />
          </div>

          <div className="form-group">
            <label htmlFor="name" className="form-label">
              Nome
            </label>
            <input
              id="name"
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              className="form-input"
              placeholder="Digite seu nome"
              required
              disabled={isLoading}
              maxLength={100}
            />
          </div>

          <div className="modal-actions">
            <button
              type="button"
              onClick={onClose}
              className="btn btn-cancel"
              disabled={isLoading}
            >
              Cancelar
            </button>
            <button
              type="submit"
              className="btn btn-save"
              disabled={isLoading}
            >
              {isLoading ? <LoadingSpinner size="small" /> : "Salvar"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default EditProfileModal;
