using Microsoft.AspNetCore.Mvc;
using GeoCidadao.AnalyticsServiceAPI.Contracts;
using GeoCidadao.AnalyticsServiceAPI.Model.DTOs;
using GeoCidadao.Models.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace GeoCidadao.AnalyticsServiceAPI.Controllers
{
    /// <summary>
    /// Analytics endpoints for managers to view problem analysis
    /// </summary>
    [ApiController]
    [Route("analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(
            IAnalyticsService analyticsService,
            ILogger<AnalyticsController> logger)
        {
            _analyticsService = analyticsService;
            _logger = logger;
        }

        /// <summary>
        /// Get summary of posts for a specific region
        /// </summary>
        /// <param name="regionId">Region identifier (format: City-State)</param>
        /// <returns>Region summary with metrics</returns>
        [HttpGet("regions/{regionId}/summary")]
        [SwaggerOperation(Summary = "Get region summary", Description = "Returns aggregated metrics for a specific region")]
        [SwaggerResponse(200, "Region summary retrieved successfully", typeof(RegionSummaryDTO))]
        [SwaggerResponse(404, "Region not found")]
        public async Task<ActionResult<RegionSummaryDTO>> GetRegionSummary(string regionId)
        {
            try
            {
                var summary = await _analyticsService.GetRegionSummaryAsync(regionId);
                
                if (summary == null)
                    return NotFound(new { message = $"No data found for region {regionId}" });

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting region summary for {regionId}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get top problems (most relevant posts) by region, period, or category
        /// </summary>
        /// <param name="region">Optional region filter (format: City-State)</param>
        /// <param name="period">Optional period filter (not implemented yet)</param>
        /// <param name="category">Optional category filter</param>
        /// <param name="limit">Maximum number of results (default: 10)</param>
        /// <returns>List of top problems</returns>
        [HttpGet("top-problems")]
        [SwaggerOperation(Summary = "Get top problems", Description = "Returns the most relevant posts filtered by region, period, or category")]
        [SwaggerResponse(200, "Top problems retrieved successfully", typeof(List<TopProblemDTO>))]
        public async Task<ActionResult<List<TopProblemDTO>>> GetTopProblems(
            [FromQuery] string? region = null,
            [FromQuery] string? period = null,
            [FromQuery] PostCategory? category = null,
            [FromQuery] int limit = 10)
        {
            try
            {
                var topProblems = await _analyticsService.GetTopProblemsAsync(region, period, category, limit);
                return Ok(topProblems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top problems");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get hotspots (regions with most posts)
        /// </summary>
        /// <param name="limit">Maximum number of results (default: 20)</param>
        /// <returns>List of hotspot regions</returns>
        [HttpGet("hotspots")]
        [SwaggerOperation(Summary = "Get hotspots", Description = "Returns regions with the highest concentration of posts for heat mapping")]
        [SwaggerResponse(200, "Hotspots retrieved successfully", typeof(List<HotspotDTO>))]
        public async Task<ActionResult<List<HotspotDTO>>> GetHotspots([FromQuery] int limit = 20)
        {
            try
            {
                var hotspots = await _analyticsService.GetHotspotsAsync(limit);
                return Ok(hotspots);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting hotspots");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
