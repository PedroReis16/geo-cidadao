const NOMINATIM_URL = import.meta.env.VITE_API_URL || "http://localhost:8081";

export interface AddressDetails {
  road?: string;
  neighbourhood?: string;
  suburb?: string;
  city?: string;
  state?: string;
  country?: string;
  displayName: string;
}

export const reverseGeocode = async (
  lat: number,
  lng: number
): Promise<AddressDetails> => {
  try {
    const response = await fetch(
      `${NOMINATIM_URL}/nominatim/reverse?format=json&lat=${lat}&lon=${lng}&zoom=18&addressdetails=1`
    );

    if (!response.ok) {
      throw new Error("Erro ao buscar endereço");
    }

    const data = await response.json();

    const address = data.address || {};

    // Monta o nome de exibição mais legível
    const parts: string[] = [];
    
    if (address.road) {
      parts.push(address.road);
      if (address.house_number) {
        parts[parts.length - 1] += `, ${address.house_number}`;
      }
    }
    
    if (address.neighbourhood || address.suburb) {
      parts.push(address.neighbourhood || address.suburb);
    }
    
    if (address.city || address.town || address.village) {
      parts.push(address.city || address.town || address.village);
    }
    
    const displayName = parts.length > 0 ? parts.join(" - ") : data.display_name;

    return {
      road: address.road,
      neighbourhood: address.neighbourhood || address.suburb,
      suburb: address.suburb,
      city: address.city || address.town || address.village,
      state: address.state,
      country: address.country,
      displayName,
    };
  } catch (error) {
    console.error("Erro ao buscar endereço:", error);
    // Retorna coordenadas formatadas como fallback
    return {
      displayName: `${lat.toFixed(6)}, ${lng.toFixed(6)}`,
    };
  }
};
