using GeoCidadao.AnalyticsServiceAPI.Contracts;
using GeoCidadao.AnalyticsServiceAPI.Database.Contracts;
using GeoCidadao.AnalyticsServiceAPI.Model.DTOs;
using GeoCidadao.Models.Enums;

namespace GeoCidadao.AnalyticsServiceAPI.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IProblemEventDao _problemEventDao;
        private readonly ILogger<AnalyticsService> _logger;

        public AnalyticsService(
            IProblemEventDao problemEventDao,
            ILogger<AnalyticsService> logger)
        {
            _problemEventDao = problemEventDao;
            _logger = logger;
        }

        public async Task<RegionSummaryDTO> GetRegionSummaryAsync(string? city, string? state)
        {
            _logger.LogInformation($"Getting region summary for city={city}, state={state}");

            var problemsByCategory = await _problemEventDao.GetProblemCountByCategoryAsync(city, state, null, null);
            var mostRelevant = await _problemEventDao.GetMostRelevantByRegionAsync(city, state, 10);

            var summary = new RegionSummaryDTO
            {
                City = city,
                State = state,
                TotalProblems = problemsByCategory.Values.Sum(),
                ProblemsByCategory = problemsByCategory,
                MostRelevantProblems = mostRelevant.Select(MapToProblemEventDTO).ToList()
            };

            return summary;
        }

        public async Task<List<ProblemEventDTO>> GetTopProblemsAsync(
            string? city, 
            string? state, 
            PostCategory? category, 
            DateTime? startDate, 
            DateTime? endDate, 
            int limit = 10)
        {
            _logger.LogInformation($"Getting top problems for city={city}, state={state}, category={category}, limit={limit}");

            var problems = await _problemEventDao.GetTopProblemsByRegionAsync(city, state, category, startDate, endDate, limit);
            
            return problems.Select(MapToProblemEventDTO).ToList();
        }

        public async Task<List<HotspotDTO>> GetHotspotsAsync(string? state, int limit = 20)
        {
            _logger.LogInformation($"Getting hotspots for state={state}, limit={limit}");

            var hotspots = await _problemEventDao.GetHotspotsAsync(state, limit);
            
            return hotspots.Select(h => new HotspotDTO
            {
                City = h.Key,
                ProblemCount = h.Value
            }).ToList();
        }

        private static ProblemEventDTO MapToProblemEventDTO(Models.Entities.AnalyticsServiceAPI.ProblemEvent pe)
        {
            return new ProblemEventDTO
            {
                Id = pe.Id,
                PostId = pe.PostId,
                Title = pe.Title,
                Description = pe.Description,
                Category = pe.Category,
                City = pe.City,
                State = pe.State,
                Latitude = pe.Latitude,
                Longitude = pe.Longitude,
                EventTimestamp = pe.EventTimestamp,
                LikesCount = pe.LikesCount,
                CommentsCount = pe.CommentsCount,
                RelevanceScore = pe.RelevanceScore
            };
        }
    }
}
