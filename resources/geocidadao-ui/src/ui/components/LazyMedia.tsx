import React from "react";
import { useAuthImage } from "../../data/hooks/useAuthImage";

interface LazyMediaProps {
  type: "image" | "video";
  url: string;
  isActive: boolean;
  shouldPreload?: boolean; // Para pré-carregar mídias adjacentes
  alt?: string;
  className?: string;
}

/**
 * Componente que carrega mídia de forma lazy apenas quando está ativa no carrossel
 */
const LazyMedia: React.FC<LazyMediaProps> = React.memo(({
  type,
  url,
  isActive,
  shouldPreload = false,
  alt = "Post media",
  className = "",
}) => {
  const shouldLoad = isActive || shouldPreload;
  console.log('🎬 LazyMedia render:', { 
    urlEnd: url.substring(url.length - 30), 
    isActive, 
    shouldPreload, 
    shouldLoad, 
    type 
  });
  
  const { imageSrc, loading, error } = useAuthImage(shouldLoad ? url : null);
  
  console.log('📊 LazyMedia estado após useAuthImage:', { 
    imageSrc: imageSrc ? imageSrc.substring(0, 50) + '...' : null, 
    loading, 
    error 
  });

  // Se não deve carregar ainda, mostra placeholder
  if (!shouldLoad) {
    console.log('⏸️ LazyMedia: Não carregando ainda');
    return (
      <div className={className}>
        <div className="lazy-media-placeholder">
          <div className="lazy-media-icon">📷</div>
        </div>
      </div>
    );
  }

  // Se está carregando
  if (loading) {
    console.log('⏳ LazyMedia: Carregando...');
    return (
      <div className={className}>
        <div className="lazy-media-loading">
          <div className="spinner-small"></div>
        </div>
      </div>
    );
  }

  // Se houve erro
  if (error || !imageSrc) {
    console.log('❌ LazyMedia: Erro ou sem imagem');
    return (
      <div className={className}>
        <div className="lazy-media-error">
          <p>⚠️ Erro ao carregar mídia</p>
        </div>
      </div>
    );
  }

  // Renderiza a mídia carregada
  console.log('✅ LazyMedia: Renderizando mídia!', type, 'com src:', imageSrc);
  if (type === "image") {
    return <img key={imageSrc} src={imageSrc} alt={alt} className={className} />;
  } else {
    return <video key={imageSrc} src={imageSrc} controls className={className} />;
  }
});

LazyMedia.displayName = 'LazyMedia';

export default LazyMedia;
