using GeoCidadao.AnalyticsServiceAPI.Model.Entities;

namespace GeoCidadao.AnalyticsServiceAPI.Database.Contracts
{
    public interface IRegionMetricsDao
    {
        Task<RegionMetrics?> FindByRegionIdentifierAsync(string regionIdentifier);
        Task AddAsync(RegionMetrics regionMetrics);
        Task UpdateAsync(RegionMetrics regionMetrics);
        Task<List<RegionMetrics>> GetTopRegionsByPostCountAsync(int limit = 10);
        Task<List<RegionMetrics>> GetAllRegionsAsync();
    }
}
