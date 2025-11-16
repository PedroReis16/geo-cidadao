using GeoCidadao.AnalyticsServiceAPI.Model.DTOs;
using GeoCidadao.Models.Enums;

namespace GeoCidadao.AnalyticsServiceAPI.Contracts
{
    public interface IAnalyticsService
    {
        Task<RegionSummaryDTO> GetRegionSummaryAsync(string? city, string? state);
        Task<List<ProblemEventDTO>> GetTopProblemsAsync(string? city, string? state, PostCategory? category, DateTime? startDate, DateTime? endDate, int limit = 10);
        Task<List<HotspotDTO>> GetHotspotsAsync(string? state, int limit = 20);
    }
}
