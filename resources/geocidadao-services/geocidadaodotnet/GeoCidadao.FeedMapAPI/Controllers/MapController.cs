using GeoCidadao.FeedMapAPI.Contracts;
using GeoCidadao.FeedMapAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.FeedMapAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class MapController(IMapService mapService, ILogger<MapController> logger) : ControllerBase
    {
        private readonly IMapService _mapService = mapService;
        private readonly ILogger<MapController> _logger = logger;

        /// <summary>
        /// Retorna postagens dentro de um quadrante geográfico baseado no nível de zoom
        /// </summary>
        /// <param name="topLeftLat">Latitude do canto superior esquerdo do bounding box</param>
        /// <param name="topLeftLon">Longitude do canto superior esquerdo do bounding box</param>
        /// <param name="bottomRightLat">Latitude do canto inferior direito do bounding box</param>
        /// <param name="bottomRightLon">Longitude do canto inferior direito do bounding box</param>
        /// <param name="zoom">Nível de zoom do mapa (0-20, onde 20 é o mais aproximado)</param>
        /// <param name="limit">Número máximo de postagens a retornar (padrão: 100)</param>
        /// <returns>Lista de postagens otimizadas para exibição no mapa</returns>
        [HttpGet]
        public async Task<IActionResult> GetMapFeed(
            [FromQuery] double topLeftLat,
            [FromQuery] double topLeftLon,
            [FromQuery] double bottomRightLat,
            [FromQuery] double bottomRightLon,
            [FromQuery] int zoom,
            [FromQuery] int limit = 100)
        {
            try
            {
                // Validações básicas
                if (topLeftLat < bottomRightLat)
                {
                    return BadRequest(new { error = "topLeftLat deve ser maior que bottomRightLat" });
                }

                if (topLeftLon > bottomRightLon)
                {
                    return BadRequest(new { error = "topLeftLon deve ser menor que bottomRightLon" });
                }

                if (zoom < 0 || zoom > 20)
                {
                    return BadRequest(new { error = "zoom deve estar entre 0 e 20" });
                }

                if (limit < 1 || limit > 500)
                {
                    return BadRequest(new { error = "limit deve estar entre 1 e 500" });
                }

                List<MapPostDTO> posts = await _mapService.GetPostsInBoundsAsync(
                    topLeftLat,
                    topLeftLon,
                    bottomRightLat,
                    bottomRightLon,
                    zoom,
                    limit);

                return Ok(new
                {
                    count = posts.Count,
                    zoom = zoom,
                    bounds = new
                    {
                        topLeft = new { lat = topLeftLat, lon = topLeftLon },
                        bottomRight = new { lat = bottomRightLat, lon = bottomRightLon }
                    },
                    posts = posts
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar postagens no mapa");
                return StatusCode(500, new { error = "Erro interno ao processar requisição" });
            }
        }
    }
}