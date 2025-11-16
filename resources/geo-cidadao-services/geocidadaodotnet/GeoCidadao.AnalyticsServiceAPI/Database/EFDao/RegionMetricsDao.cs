using GeoCidadao.AnalyticsServiceAPI.Database.Contracts;
using GeoCidadao.AnalyticsServiceAPI.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoCidadao.AnalyticsServiceAPI.Database.EFDao
{
    public class RegionMetricsDao : IRegionMetricsDao
    {
        private readonly AnalyticsDbContext _context;

        public RegionMetricsDao(AnalyticsDbContext context)
        {
            _context = context;
        }

        public async Task<RegionMetrics?> FindByRegionIdentifierAsync(string regionIdentifier)
        {
            return await _context.RegionMetrics
                .FirstOrDefaultAsync(r => r.RegionIdentifier == regionIdentifier);
        }

        public async Task AddAsync(RegionMetrics regionMetrics)
        {
            await _context.RegionMetrics.AddAsync(regionMetrics);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RegionMetrics regionMetrics)
        {
            _context.RegionMetrics.Update(regionMetrics);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RegionMetrics>> GetTopRegionsByPostCountAsync(int limit = 10)
        {
            return await _context.RegionMetrics
                .OrderByDescending(r => r.TotalPosts)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<RegionMetrics>> GetAllRegionsAsync()
        {
            return await _context.RegionMetrics.ToListAsync();
        }
    }
}
