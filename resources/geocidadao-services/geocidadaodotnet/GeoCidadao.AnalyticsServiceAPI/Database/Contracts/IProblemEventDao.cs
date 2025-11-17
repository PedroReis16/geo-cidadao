using GeoCidadao.Models.Entities.AnalyticsServiceAPI;
using GeoCidadao.Models.Enums;

namespace GeoCidadao.AnalyticsServiceAPI.Database.Contracts
{
    public interface IProblemEventDao
    {
        Task<ProblemEvent?> FindAsync(Guid id);
        Task<ProblemEvent?> GetByPostIdAsync(Guid postId);
        Task AddAsync(ProblemEvent problemEvent);
        Task UpdateAsync(ProblemEvent problemEvent);
        Task<List<ProblemEvent>> GetTopProblemsByRegionAsync(string? city, string? state, PostCategory? category, DateTime? startDate, DateTime? endDate, int limit = 10);
        Task<List<ProblemEvent>> GetMostRelevantByRegionAsync(string? city, string? state, int limit = 10);
        Task<Dictionary<PostCategory, int>> GetProblemCountByCategoryAsync(string? city, string? state, DateTime? startDate, DateTime? endDate);
        Task<Dictionary<string, int>> GetHotspotsAsync(string? state, int limit = 20);
    }
}
