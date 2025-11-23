using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Nominatim;

namespace GeoCidadao.GerenciamentoPostsAPI.Contracts.ConnectionServices
{
    public interface INominatimService
    {
        Task<AddressDTO> GetCoordinatesDetailsAsync(double latitude, double longitude);
    }
}