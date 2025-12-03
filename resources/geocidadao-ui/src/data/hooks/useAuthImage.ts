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

        console.log('📡 useAuthImage - Response:', {
          status: response.status,
          ok: response.ok,
          contentType: response.headers.get('content-type'),
          url: imageUrl
        });
        
        if (!response.ok) {
          throw new Error(`Erro ao carregar imagem: ${response.status}`);
        }

        // Lê a resposta como texto para obter a URL real da imagem
        const realImageUrl = await response.text();
        console.log('🔗 useAuthImage - URL real da imagem:', realImageUrl);

        // Como a URL do S3 é assinada e temporária, podemos usá-la diretamente
        // Isso evita problemas de CORS com o LocalStack
        console.log('✅ Usando URL assinada diretamente');
        
        if (isMounted) {
          setImageSrc(realImageUrl);
          setLoading(false);
          console.log('🎯 useAuthImage - Estado atualizado com URL assinada');
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

    // Cleanup: cancela a requisição se o componente desmontar
    return () => {
      console.log('🧹 useAuthImage - Cleanup, desmontando:', imageUrl);
      isMounted = false;
      controller.abort();
    };
  }, [imageUrl]);

  return { imageSrc, loading, error };
};
