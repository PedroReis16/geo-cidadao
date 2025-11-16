using GeoCidadao.AnalyticsServiceAPI.Model.DTOs;
using GeoCidadao.Models.Enums;

namespace GeoCidadao.AnalyticsServiceAPI.Contracts
{
    public interface IAnalyticsService
    {
        Task<RegionSummaryDTO?> GetRegionSummaryAsync(string regionIdentifier);
        Task<List<TopProblemDTO>> GetTopProblemsAsync(string? region = null, string? period = null, PostCategory? category = null, int limit = 10);
        Task<List<HotspotDTO>> GetHotspotsAsync(int limit = 20);
    }
}
