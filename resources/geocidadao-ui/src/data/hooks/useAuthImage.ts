import { useState, useEffect } from "react";
import keycloak from "../../config/keycloak";

/**
 * Hook para carregar imagens que requerem autenticação
 * Converte a URL em um blob local para exibição
 */
export const useAuthImage = (imageUrl: string | null | undefined) => {
  const [imageSrc, setImageSrc] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<boolean>(false);

  useEffect(() => {
    console.log('🖼️ useAuthImage - URL mudou:', imageUrl);
    
    // Reset do estado quando a URL muda
    setImageSrc(null);
    setLoading(false);
    setError(false);
    
    if (!imageUrl) {
      console.log('⏹️ useAuthImage - Sem URL, não carregando');
      return;
    }

    let isMounted = true;
    const controller = new AbortController();
    let currentObjectUrl: string | null = null;

    const loadImage = async () => {
      try {
        console.log('⏳ useAuthImage - Iniciando carregamento:', imageUrl);
        setLoading(true);
        setError(false);

        // Garante que o token está válido
        await keycloak.updateToken(30);

        console.log('🔑 useAuthImage - Token atualizado, fazendo fetch...');
        const response = await fetch(imageUrl, {
          method: "GET",
          headers: {
            Authorization: `Bearer ${keycloak.token}`,
          },
          signal: controller.signal,
        });

        console.log('📡 useAuthImage - Response status:', response.status);
        
        if (!response.ok) {
          throw new Error(`Erro ao carregar imagem: ${response.status}`);
        }

        const blob = await response.blob();
        currentObjectUrl = URL.createObjectURL(blob);
        console.log('✅ Imagem carregada com sucesso! Blob URL:', currentObjectUrl, 'size:', blob.size, 'type:', blob.type);

        if (isMounted) {
          setImageSrc(currentObjectUrl);
          setLoading(false);
          console.log('🎯 useAuthImage - Estado atualizado com blob URL');
        } else {
          console.log('⚠️ useAuthImage - Componente desmontado, revogando blob');
          URL.revokeObjectURL(currentObjectUrl);
        }
      } catch (err) {
        if (isMounted && err instanceof Error && err.name !== "AbortError") {
          console.error("❌ Erro ao carregar imagem:", imageUrl, err);
          setError(true);
          setLoading(false);
        } else if (err instanceof Error && err.name === "AbortError") {
          console.log('🚫 useAuthImage - Requisição cancelada');
        }
      }
    };

    loadImage();

    // Cleanup: revoga o object URL e cancela a requisição se o componente desmontar
    return () => {
      console.log('🧹 useAuthImage - Cleanup, desmontando:', imageUrl);
      isMounted = false;
      controller.abort();
      if (currentObjectUrl) {
        URL.revokeObjectURL(currentObjectUrl);
      }
    };
  }, [imageUrl]);

  return { imageSrc, loading, error };
};
