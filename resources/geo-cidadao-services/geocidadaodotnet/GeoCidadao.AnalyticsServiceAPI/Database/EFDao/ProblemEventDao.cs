using GeoCidadao.AnalyticsServiceAPI.Database.Contracts;
using GeoCidadao.Database;
using GeoCidadao.Models.Entities.AnalyticsServiceAPI;
using GeoCidadao.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace GeoCidadao.AnalyticsServiceAPI.Database.EFDao
{
    public class ProblemEventDao : IProblemEventDao
    {
        private readonly GeoDbContext _context;

        public ProblemEventDao(GeoDbContext context)
        {
            _context = context;
        }

        public async Task<ProblemEvent?> FindAsync(Guid id)
        {
            return await _context.ProblemEvents.FindAsync(id);
        }

        public async Task<ProblemEvent?> GetByPostIdAsync(Guid postId)
        {
            return await _context.ProblemEvents
                .Where(pe => pe.PostId == postId)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(ProblemEvent problemEvent)
        {
            await _context.ProblemEvents.AddAsync(problemEvent);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProblemEvent problemEvent)
        {
            _context.ProblemEvents.Update(problemEvent);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProblemEvent>> GetTopProblemsByRegionAsync(
            string? city, 
            string? state, 
            PostCategory? category, 
            DateTime? startDate, 
            DateTime? endDate, 
            int limit = 10)
        {
            var query = _context.ProblemEvents.AsQueryable();

            if (!string.IsNullOrEmpty(city))
                query = query.Where(pe => pe.City == city);

            if (!string.IsNullOrEmpty(state))
                query = query.Where(pe => pe.State == state);

            if (category.HasValue)
                query = query.Where(pe => pe.Category == category.Value);

            if (startDate.HasValue)
                query = query.Where(pe => pe.EventTimestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(pe => pe.EventTimestamp <= endDate.Value);

            return await query
                .OrderByDescending(pe => pe.RelevanceScore)
                .ThenByDescending(pe => pe.EventTimestamp)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<ProblemEvent>> GetMostRelevantByRegionAsync(string? city, string? state, int limit = 10)
        {
            var query = _context.ProblemEvents.AsQueryable();

            if (!string.IsNullOrEmpty(city))
                query = query.Where(pe => pe.City == city);

            if (!string.IsNullOrEmpty(state))
                query = query.Where(pe => pe.State == state);

            return await query
                .OrderByDescending(pe => pe.RelevanceScore)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Dictionary<PostCategory, int>> GetProblemCountByCategoryAsync(
            string? city, 
            string? state, 
            DateTime? startDate, 
            DateTime? endDate)
        {
            var query = _context.ProblemEvents.AsQueryable();

            if (!string.IsNullOrEmpty(city))
                query = query.Where(pe => pe.City == city);

            if (!string.IsNullOrEmpty(state))
                query = query.Where(pe => pe.State == state);

            if (startDate.HasValue)
                query = query.Where(pe => pe.EventTimestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(pe => pe.EventTimestamp <= endDate.Value);

            return await query
                .GroupBy(pe => pe.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetHotspotsAsync(string? state, int limit = 20)
        {
            var query = _context.ProblemEvents.AsQueryable();

            if (!string.IsNullOrEmpty(state))
                query = query.Where(pe => pe.State == state);

            return await query
                .Where(pe => !string.IsNullOrEmpty(pe.City))
                .GroupBy(pe => pe.City!)
                .Select(g => new { City = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(limit)
                .ToDictionaryAsync(x => x.City, x => x.Count);
        }
    }
}
