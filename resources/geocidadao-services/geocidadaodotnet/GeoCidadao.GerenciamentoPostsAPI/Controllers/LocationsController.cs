using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoPostsAPI.Controllers
{
    [ApiController]
    [Route("posts/locations")]
    [Authorize]
    public class LocationsController(ILocationsService service) : ControllerBase
    {
        private readonly ILocationsService _service = service;

        /// <summary>
        /// Obter posts em formato GeoJSON para visualização em mapa
        /// </summary>
        /// <param name="latitude">Latitude do ponto central</param>
        /// <param name="longitude">Longitude do ponto central</param>
        /// <param name="radiusKm">Raio de busca em quilômetros</param>
        /// <param name="city">Filtrar por cidade</param>
        /// <param name="state">Filtrar por estado</param>
        /// <param name="country">Filtrar por país</param>
        /// <param name="itemsCount">Número máximo de posts a serem retornados</param>
        /// <param name="pageNumber">Número da página (iniciando em 1)</param>
        /// <returns>GeoJSON FeatureCollection com os posts encontrados</returns>
        [HttpGet("map")]
        [Authorize(Policy = "Posts.Read")]
        public async Task<IActionResult> GetPostsMap(
            [FromQuery] double? latitude,
            [FromQuery] double? longitude,
            [FromQuery] double? radiusKm,
            [FromQuery] string? city,
            [FromQuery] string? state,
            [FromQuery] string? country,
            [FromQuery] int? itemsCount,
            [FromQuery] int? pageNumber)
        {
            var locationQuery = new LocationQueryDTO
            {
                Latitude = latitude,
                Longitude = longitude,
                RadiusKm = radiusKm,
                City = city,
                State = state,
                Country = country,
                ItemsCount = itemsCount,
                PageNumber = pageNumber
            };

            GeoJsonFeatureCollectionDTO geoJson = await _service.GetPostsAsGeoJsonAsync(locationQuery);

            if (geoJson.Features.Count == 0)
                return NoContent();

            return Ok(geoJson);
        }
    }
}