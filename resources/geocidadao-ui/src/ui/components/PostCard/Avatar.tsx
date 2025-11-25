import React from "react";
import "../../styles/components/PostCard/Avatar.css";
import AuthImage from "../AuthImage";

interface AvatarProps {
  src?: string;
  name: string;
  alt: string;
  className?: string;
}

/**
 * Componente Avatar que exibe a foto de perfil ou iniciais do usuário
 * Caso não haja foto, exibe as iniciais em um círculo colorido
 */
const Avatar: React.FC<AvatarProps> = ({ src, name, alt, className = "" }) => {

  /**
   * Extrai as iniciais do nome do usuário
   * Exemplos: "João Silva" -> "JS", "Maria" -> "M"
   */
  const getInitials = (fullName: string): string => {
    if (!fullName) return "?";
    
    const names = fullName.trim().split(" ");
    if (names.length === 1) {
      return names[0].charAt(0).toUpperCase();
    }
    
    // Pega primeira letra do primeiro e último nome
    const firstInitial = names[0].charAt(0);
    const lastInitial = names[names.length - 1].charAt(0);
    return (firstInitial + lastInitial).toUpperCase();
  };

  /**
   * Gera uma cor baseada no hash do nome para consistência
   */
  const getColorFromName = (name: string): string => {
    const colors = [
      "#FF6B6B", // vermelho
      "#4ECDC4", // turquesa
      "#45B7D1", // azul claro
      "#FFA07A", // salmão
      "#98D8C8", // menta
      "#F7DC6F", // amarelo
      "#BB8FCE", // roxo claro
      "#85C1E2", // azul céu
      "#F8B739", // laranja
      "#52B788", // verde
    ];

    let hash = 0;
    for (let i = 0; i < name.length; i++) {
      hash = name.charCodeAt(i) + ((hash << 5) - hash);
    }
    
    return colors[Math.abs(hash) % colors.length];
  };

  const initials = getInitials(name);
  const backgroundColor = getColorFromName(name);

  // Fallback para quando não há imagem ou ocorre erro
  const initialsElement = (
    <div
      className={`avatar avatar--initials ${className}`}
      style={{ backgroundColor }}
      aria-label={alt}
      title={name}
    >
      <span className="avatar__initials">{initials}</span>
    </div>
  );

  // Se não há src, mostra as iniciais
  if (!src) {
    return initialsElement;
  }

  // Usa AuthImage para carregar com autenticação
  return (
    <AuthImage
      src={src}
      alt={alt}
      className={`avatar avatar--image ${className}`}
      fallback={initialsElement}
      loadingComponent={initialsElement}
    />
  );
};

export default Avatar;
