using GeoCidadao.AnalyticsServiceAPI.Contracts;
using GeoCidadao.AnalyticsServiceAPI.Model.DTOs;
using GeoCidadao.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.AnalyticsServiceAPI.Controllers
{
    /// <summary>
    /// Analytics operations for problem tracking and regional insights
    /// </summary>
    [ApiController]
    [Route("analytics")]
    [Authorize]
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
        /// Get summary of problems for a specific region
        /// </summary>
        /// <param name="city">City name (optional)</param>
        /// <param name="state">State name (optional)</param>
        /// <returns>Region summary with problem statistics</returns>
        [HttpGet("regions/summary")]
        [Authorize(Policy = "Analytics.Read")]
        [ProducesResponseType(typeof(RegionSummaryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<RegionSummaryDTO>> GetRegionSummary(
            [FromQuery] string? city = null,
            [FromQuery] string? state = null)
        {
            try
            {
                var summary = await _analyticsService.GetRegionSummaryAsync(city, state);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting region summary: {ex.Message}");
                return StatusCode(500, "An error occurred while getting region summary");
            }
        }

        /// <summary>
        /// Get top problems by region, category, and time period
        /// </summary>
        /// <param name="city">City name (optional)</param>
        /// <param name="state">State name (optional)</param>
        /// <param name="category">Problem category (optional)</param>
        /// <param name="startDate">Start date for filtering (optional)</param>
        /// <param name="endDate">End date for filtering (optional)</param>
        /// <param name="limit">Maximum number of results (default: 10)</param>
        /// <returns>List of top problems</returns>
        [HttpGet("top-problems")]
        [Authorize(Policy = "Analytics.Read")]
        [ProducesResponseType(typeof(List<ProblemEventDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<ProblemEventDTO>>> GetTopProblems(
            [FromQuery] string? city = null,
            [FromQuery] string? state = null,
            [FromQuery] PostCategory? category = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int limit = 10)
        {
            try
            {
                var problems = await _analyticsService.GetTopProblemsAsync(city, state, category, startDate, endDate, limit);
                return Ok(problems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting top problems: {ex.Message}");
                return StatusCode(500, "An error occurred while getting top problems");
            }
        }

        /// <summary>
        /// Get hotspots (cities with most problems)
        /// </summary>
        /// <param name="state">State name (optional)</param>
        /// <param name="limit">Maximum number of results (default: 20)</param>
        /// <returns>List of hotspots with problem counts</returns>
        [HttpGet("hotspots")]
        [Authorize(Policy = "Analytics.Read")]
        [ProducesResponseType(typeof(List<HotspotDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<HotspotDTO>>> GetHotspots(
            [FromQuery] string? state = null,
            [FromQuery] int limit = 20)
        {
            try
            {
                var hotspots = await _analyticsService.GetHotspotsAsync(state, limit);
                return Ok(hotspots);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting hotspots: {ex.Message}");
                return StatusCode(500, "An error occurred while getting hotspots");
            }
        }
    }
}
