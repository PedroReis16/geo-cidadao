using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs;

namespace GeoCidadao.GerenciamentoPostsAPI.Contracts
{
    public interface ILocationsService
    {
        Task<GeoJsonFeatureCollectionDTO> GetPostsAsGeoJsonAsync(LocationQueryDTO locationQuery);
    }
}