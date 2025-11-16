using GeoCidadao.AnalyticsServiceAPI.Database.Contracts;
using GeoCidadao.AnalyticsServiceAPI.Model.Entities;
using GeoCidadao.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace GeoCidadao.AnalyticsServiceAPI.Database.EFDao
{
    public class PostAnalyticsDao : IPostAnalyticsDao
    {
        private readonly AnalyticsDbContext _context;

        public PostAnalyticsDao(AnalyticsDbContext context)
        {
            _context = context;
        }

        public async Task<PostAnalytics?> FindByPostIdAsync(Guid postId)
        {
            return await _context.PostAnalytics
                .FirstOrDefaultAsync(p => p.PostId == postId);
        }

        public async Task AddAsync(PostAnalytics postAnalytics)
        {
            await _context.PostAnalytics.AddAsync(postAnalytics);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PostAnalytics>> GetTopPostsByRelevanceAsync(
            string? city = null, 
            string? state = null, 
            PostCategory? category = null, 
            int limit = 10)
        {
            var query = _context.PostAnalytics.AsQueryable();

            if (!string.IsNullOrEmpty(city))
                query = query.Where(p => p.City == city);

            if (!string.IsNullOrEmpty(state))
                query = query.Where(p => p.State == state);

            if (category.HasValue)
                query = query.Where(p => p.Category == category.Value);

            return await query
                .OrderByDescending(p => p.RelevanceScore)
                .ThenByDescending(p => p.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<PostAnalytics>> GetPostsByRegionAsync(
            string? city, 
            string? state, 
            DateTime? startDate = null, 
            DateTime? endDate = null)
        {
            var query = _context.PostAnalytics.AsQueryable();

            if (!string.IsNullOrEmpty(city))
                query = query.Where(p => p.City == city);

            if (!string.IsNullOrEmpty(state))
                query = query.Where(p => p.State == state);

            if (startDate.HasValue)
                query = query.Where(p => p.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.CreatedAt <= endDate.Value);

            return await query.ToListAsync();
        }

        public async Task<int> CountPostsInRegionAsync(string? city, string? state)
        {
            var query = _context.PostAnalytics.AsQueryable();

            if (!string.IsNullOrEmpty(city))
                query = query.Where(p => p.City == city);

            if (!string.IsNullOrEmpty(state))
                query = query.Where(p => p.State == state);

            return await query.CountAsync();
        }
    }
}
