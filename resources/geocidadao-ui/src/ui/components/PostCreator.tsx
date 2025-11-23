import React, { useState, useRef } from "react";
import { Image, Video, X } from "lucide-react";
import "../styles/components/PostCreator.css";

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
  const fileInputRef = useRef<HTMLInputElement>(null);

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

  const removeFile = (index) => {
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

  const handlePublish = () => {
    setPostText("");
    setSelectedFiles([]);
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
                e.target.style.height = "auto";
                e.target.style.height = e.target.scrollHeight + "px";
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
        </div>

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
            disabled={!postText.trim() && selectedFiles.length === 0}
            className="pc-publish-btn"
          >
            Publicar
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
    </div>
  );
}
