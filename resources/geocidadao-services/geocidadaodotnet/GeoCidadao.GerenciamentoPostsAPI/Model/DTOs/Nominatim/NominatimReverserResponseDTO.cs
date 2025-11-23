using System.Text.Json.Serialization;

namespace GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Nominatim
{
    public class NominatimReverserResponseDTO
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("lat")]
        public string Latitude { get; set; } = string.Empty;

        [JsonPropertyName("lon")]
        public string Longitude { get; set; } = string.Empty;

        [JsonPropertyName("address")]
        public AddressDTO Address { get; set; } = null!;
    }
}