using GeoCidadao.FeedMapAPI.Models.DTOs;

namespace GeoCidadao.FeedMapAPI.Contracts
{
    public interface IMapService
    {
        /// <summary>
        /// Busca postagens dentro de um quadrante geográfico baseado no nível de zoom
        /// </summary>
        /// <param name="topLeftLat">Latitude do canto superior esquerdo</param>
        /// <param name="topLeftLon">Longitude do canto superior esquerdo</param>
        /// <param name="bottomRightLat">Latitude do canto inferior direito</param>
        /// <param name="bottomRightLon">Longitude do canto inferior direito</param>
        /// <param name="zoomLevel">Nível de zoom do mapa (0-20, onde 20 é mais aproximado)</param>
        /// <param name="limit">Número máximo de postagens a retornar</param>
        Task<List<MapPostDTO>> GetPostsInBoundsAsync(
            double topLeftLat, 
            double topLeftLon, 
            double bottomRightLat, 
            double bottomRightLon, 
            int zoomLevel,
            int limit = 100);
    }
}