using System.Text.Json.Serialization;

namespace GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Nominatim
{
    public class AddressDTO
    {
        public string Road { get; set; } = string.Empty;
        public string Suburb { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Town { get; set; } = string.Empty;
        public string Village { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Postcode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("country_code")]
        public string CountryCode { get; set; } = string.Empty;
    }
}