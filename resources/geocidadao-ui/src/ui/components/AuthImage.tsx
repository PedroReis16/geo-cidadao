import React from "react";
import { useAuthImage } from "../../data/hooks/useAuthImage";

interface AuthImageProps {
  src: string | null | undefined;
  alt: string;
  className?: string;
  fallback?: React.ReactNode;
  loadingComponent?: React.ReactNode;
}

/**
 * Componente para exibir imagens que requerem autenticação
 * Carrega a imagem via fetch com token e a exibe como blob
 */
const AuthImage: React.FC<AuthImageProps> = ({
  src,
  alt,
  className = "",
  fallback = null,
  loadingComponent = null,
}) => {
  const { imageSrc, loading, error } = useAuthImage(src);
  
  console.log('🖼️ AuthImage:', { src, imageSrc, loading, error });

  // Se não há URL, mostra fallback
  if (!src) {
    return <>{fallback}</>;
  }

  // Se está carregando
  if (loading) {
    return (
      <>
        {loadingComponent || (
          <div className={`auth-image-loading ${className}`}>
            <div className="spinner-small"></div>
          </div>
        )}
      </>
    );
  }

  // Se houve erro
  if (error || !imageSrc) {
    return <>{fallback}</>;
  }

  // Imagem carregada com sucesso
  console.log('✅ AuthImage: Renderizando imagem com src:', imageSrc);
  return <img key={imageSrc} src={imageSrc} alt={alt} className={className} />;
};

export default AuthImage;
