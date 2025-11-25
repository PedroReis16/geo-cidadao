import React, { useState, useRef, useEffect } from "react";
import ReactDOM from "react-dom";
import { Image, Video, X, MapPin, Check, Edit2 } from "lucide-react";
import "../styles/components/PostCreator.css";
import { useMap } from "../../data/hooks/useMap";
import type { Coordinates } from "../../data/@types/Coordinates";
import { createPost } from "../../data/services/postService";
import { reverseGeocode, type AddressDetails } from "../../data/services/geocodingService";

const ALLOWED_IMAGE_TYPES = ["image/jpeg", "image/png"];
const ALLOWED_VIDEO_TYPES = ["video/mp4"];
const ALLOWED_TYPES = [...ALLOWED_IMAGE_TYPES, ...ALLOWED_VIDEO_TYPES];
const MAX_FILES = 10;

interface FileWithPreview {
  file: File;
  preview: string;
  type: "image" | "video";
  name: string;
}

export default function PostCreator() {
  const [postText, setPostText] = useState("");
  const [selectedFiles, setSelectedFiles] = useState<FileWithPreview[]>([]);
  const [isDragging, setIsDragging] = useState(false);
  const [dragCounter, setDragCounter] = useState(0);
  const [isSelectingLocation, setIsSelectingLocation] = useState(false);
  const [selectedLocation, setSelectedLocation] = useState<Coordinates | null>(null);
  const [locationAddress, setLocationAddress] = useState<AddressDetails | null>(null);
  const [isLoadingAddress, setIsLoadingAddress] = useState(false);
  const [isPublishing, setIsPublishing] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);
  
  // Usar o contexto do mapa global
  const { setIsMapExpanded, newItemPos, setNewItemPos } = useMap();

  // Sincroniza a posição selecionada do mapa com o estado local
  useEffect(() => {
    if (isSelectingLocation && newItemPos) {
      setSelectedLocation(newItemPos);
    }
  }, [newItemPos, isSelectingLocation]);

  // Busca o endereço quando uma localização é confirmada
  const fetchAddress = async (coords: Coordinates) => {
    setIsLoadingAddress(true);
    try {
      const address = await reverseGeocode(coords.lat, coords.lng);
      setLocationAddress(address);
    } catch (error) {
      console.error("Erro ao buscar endereço:", error);
      setLocationAddress({
        displayName: `${coords.lat.toFixed(6)}, ${coords.lng.toFixed(6)}`,
      });
    } finally {
      setIsLoadingAddress(false);
    }
  };

  const handleLocationClick = () => {
    // Ativar modo de seleção
    setIsSelectingLocation(true);
    setIsMapExpanded(true);
    setNewItemPos(selectedLocation);
  };

  const handleConfirmLocation = async () => {
    if (newItemPos) {
      setSelectedLocation(newItemPos);
      await fetchAddress(newItemPos);
    }
    setIsSelectingLocation(false);
    setIsMapExpanded(false);
  };

  const handleCancelLocation = () => {
    setIsSelectingLocation(false);
    setIsMapExpanded(false);
    setNewItemPos(selectedLocation);
  };

  const handleRemoveLocation = () => {
    setSelectedLocation(null);
    setLocationAddress(null);
    setNewItemPos(null);
  };

  const handleEditLocation = () => {
    // Abre o mapa novamente para edição
    setIsSelectingLocation(true);
    setIsMapExpanded(true);
    setNewItemPos(selectedLocation);
  };

  const handleDragEnter = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    e.stopPropagation();
    setDragCounter((prev) => prev + 1);
    if (e.dataTransfer.items && e.dataTransfer.items.length > 0) {
      setIsDragging(true);
    }
  };

  const handleDragLeave = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    e.stopPropagation();
    setDragCounter((prev) => {
      const newCounter = prev - 1;
      if (newCounter === 0) {
        setIsDragging(false);
      }
      return newCounter;
    });
  };

  const handleDragOver = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    e.stopPropagation();
  };

  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(false);
    setDragCounter(0);

    const files = e.dataTransfer.files ? Array.from(e.dataTransfer.files) : [];
    handleFiles(files);
  };

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files ? Array.from(e.target.files) : [];
    handleFiles(files);
  };

  const handleFiles = (files: File[]) => {
    const remainingSlots = MAX_FILES - selectedFiles.length;

    const validFiles = files.filter((file) =>
      ALLOWED_TYPES.includes(file.type)
    );

    const filesToAdd = validFiles.slice(0, remainingSlots);

    const newFiles = filesToAdd.map((file) => ({
      file,
      preview: URL.createObjectURL(file),
      type: file.type.startsWith("image/")
        ? ("image" as const)
        : ("video" as const),
      name: file.name,
    }));

    setSelectedFiles((prev) => [...prev, ...newFiles]);
  };

  const removeFile = (index: number) => {
    setSelectedFiles((prev) => {
      const newFiles = [...prev];
      URL.revokeObjectURL(newFiles[index].preview);
      newFiles.splice(index, 1);
      return newFiles;
    });
  };

  const handleVideoClick = () => {
    if (fileInputRef.current) {
      fileInputRef.current.accept = "video/mp4";
      fileInputRef.current.click();
    }
  };

  const handlePhotoClick = () => {
    if (fileInputRef.current) {
      fileInputRef.current.accept = "image/jpeg,image/png";
      fileInputRef.current.click();
    }
  };

  const handlePublish = async () => {
    if (!postText.trim() && selectedFiles.length === 0) return;

    setIsPublishing(true);
    try {
      await createPost({
        content: postText,
        latitude: selectedLocation?.lat,
        longitude: selectedLocation?.lng,
        mediaFiles: selectedFiles.map((f) => f.file),
      });

      setPostText("");
      setSelectedFiles([]);
      setSelectedLocation(null);
      setLocationAddress(null);
      setNewItemPos(null);

    } catch (error) {
      console.error("Erro ao criar post:", error);
      
    } finally {
      setIsPublishing(false);
    }
  };

  return (
    <div className="pc-container">
      <div
        className="pc-card"
        onDragEnter={handleDragEnter}
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
      >
        {isDragging && (
          <div className="pc-drag-overlay">
            <div className="pc-drag-content">
              <div className="pc-drag-icon">
                <Image className="pc-drag-icon-inner" />
              </div>
              <p>Solte os arquivos aqui</p>
              <span>Imagens e vídeos serão adicionados ao post</span>
            </div>
          </div>
        )}

        <div className="pc-header">
          <div className="pc-header-inner">
            <div className="pc-avatar">U</div>
            <textarea
              placeholder="Comece uma publicação"
              value={postText}
              onChange={(e) => setPostText(e.target.value)}
              rows={1}
              onInput={(e) => {
                const target = e.target as HTMLTextAreaElement;
                target.style.height = "auto";
                target.style.height = target.scrollHeight + "px";
              }}
              className="pc-textarea"
            />
          </div>
        </div>

        <div className="pc-media-buttons">
          <button
            onClick={handleVideoClick}
            className="pc-btn"
            disabled={selectedFiles.length >= MAX_FILES}
          >
            <Video className="pc-icon-green" />
            <span>Vídeo</span>
          </button>

          <button
            onClick={handlePhotoClick}
            className="pc-btn"
            disabled={selectedFiles.length >= MAX_FILES}
          >
            <Image className="pc-icon-blue" />
            <span>Foto</span>
          </button>

          <button
            onClick={handleLocationClick}
            className="pc-btn"
            disabled={isSelectingLocation}
          >
            <MapPin className="pc-icon-red" />
            <span>Localização</span>
          </button>
        </div>

        {/* Card de localização selecionada */}
        {selectedLocation && !isSelectingLocation && (
          <div className="pc-location-card">
            <div className="pc-location-card-icon">
              <MapPin size={20} />
            </div>
            <div className="pc-location-card-content">
              {isLoadingAddress ? (
                <span className="pc-location-card-loading">
                  Carregando endereço...
                </span>
              ) : (
                <>
                  <span className="pc-location-card-address">
                    {locationAddress?.displayName || "Localização selecionada"}
                  </span>
                  <span className="pc-location-card-coords">
                    {selectedLocation.lat.toFixed(6)}, {selectedLocation.lng.toFixed(6)}
                  </span>
                </>
              )}
            </div>
            <div className="pc-location-card-actions">
              <button
                onClick={handleEditLocation}
                className="pc-location-card-btn"
                title="Editar localização"
              >
                <Edit2 size={16} />
              </button>
              <button
                onClick={handleRemoveLocation}
                className="pc-location-card-btn pc-location-card-btn-remove"
                title="Remover localização"
              >
                <X size={16} />
              </button>
            </div>
          </div>
        )}

        {selectedFiles.length > 0 && (
          <div className="pc-files">
            <div className="pc-files-grid">
              {selectedFiles.map((file, index) => (
                <div key={index} className="pc-file-item">
                  {file.type === "image" ? (
                    <img
                      src={file.preview}
                      alt={file.name}
                      className="pc-file-preview"
                    />
                  ) : (
                    <video
                      src={file.preview}
                      className="pc-file-preview"
                      controls
                    />
                  )}
                  <button
                    onClick={() => removeFile(index)}
                    className="pc-remove-btn"
                  >
                    <X className="pc-remove-icon" />
                  </button>
                  <div className="pc-file-footer">
                    <p>{file.name}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        <div className="pc-footer">
          <button
            onClick={handlePublish}
            disabled={
              (!postText.trim() && selectedFiles.length === 0) || isPublishing
            }
            className="pc-publish-btn"
          >
            {isPublishing ? "Publicando..." : "Publicar"}
          </button>
        </div>
      </div>

      <input
        ref={fileInputRef}
        type="file"
        multiple
        accept="image/jpeg,image/png,video/mp4"
        onChange={handleFileSelect}
        className="pc-hidden-input"
      />

      {/* Banner de modo de seleção - renderizado como portal */}
      {isSelectingLocation &&
        ReactDOM.createPortal(
          <div className="pc-selection-banner">
            <div className="pc-selection-banner-content">
              <MapPin className="pc-selection-banner-icon" size={20} />
              <span>Clique no mapa para selecionar a localização do post</span>
            </div>
            <div className="pc-selection-banner-actions">
              <button
                onClick={handleCancelLocation}
                className="pc-selection-btn pc-selection-btn-cancel"
              >
                Cancelar
              </button>
              <button
                onClick={handleConfirmLocation}
                className="pc-selection-btn pc-selection-btn-confirm"
                disabled={!newItemPos}
              >
                <Check size={16} />
                Confirmar
              </button>
            </div>
          </div>,
          document.body
        )}
    </div>
  );
}
