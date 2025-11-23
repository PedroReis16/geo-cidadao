import { useContext } from "react";
import { MapContext } from "../contexts/MapContext";

/**
 * Hook para acessar o contexto do mapa
 * Deve ser usado apenas dentro de componentes que estão dentro do MapProvider
 */
export const useMap = () => {
  const context = useContext(MapContext);
  
  if (!context) {
    throw new Error("useMap must be used within MapProvider");
  }
  
  return context;
};
