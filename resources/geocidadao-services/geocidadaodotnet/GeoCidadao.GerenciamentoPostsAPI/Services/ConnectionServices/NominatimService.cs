using System.Text.Json;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.ConnectionServices;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Nominatim;

namespace GeoCidadao.GerenciamentoPostsAPI.Services.ConnectionServices
{
    public class NominatimService(HttpClient httpClient) : INominatimService
    {
        private const double _minLatitude = -23.972;
        private const double _maxLatitude = -23.594;
        private const double _minLongitude = -46.693;
        private const double _maxLongitude = -46.007;

        private readonly HttpClient _httpClient = httpClient;

        public async Task<AddressDTO> GetCoordinatesDetailsAsync(double latitude, double longitude)
        {
            ValidateCoordinates(latitude, longitude);

            HttpClient client = new HttpClient()
            {
                BaseAddress = _httpClient.BaseAddress!,
            };

            HttpResponseMessage response = await client.GetAsync($"reverse?lat={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&lon={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&format=json&addressdetails=1");

            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();

            NominatimReverserResponseDTO addressResponse = JsonSerializer.Deserialize<NominatimReverserResponseDTO>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new InvalidOperationException("Houve um erro ao desserializar a resposta do Nominatim.");

            return addressResponse!.Address;
        }

        private void ValidateCoordinates(double latitude, double longitude)
        {
            if (_minLatitude > latitude || _maxLatitude < latitude ||
               _minLongitude > longitude || _maxLongitude < longitude)
                throw new ArgumentOutOfRangeException($"As coordenadas fornecidas (Latitude: {latitude}, Longitude: {longitude}) estão fora da área de cobertura do serviço.");
        }

    }
}