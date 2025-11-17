using GeoCidadao.Database;
using GeoCidadao.Database.EFDao;
using GeoCidadao.Database.Entities.GerenciamentoPostsAPI;
using GeoCidadao.GerenciamentoPostsAPI.Database.CacheContracts;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.Models.Enums;
using GeoCidadao.Models.Exceptions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace GeoCidadao.GerenciamentoPostsAPI.Database.EFDao
{
    internal class PostLocationDao(GeoDbContext context, IPostLocationDaoCache? cache = null) : BaseDao<PostLocation>(context, cache), IPostLocationDao
    {
        protected override IPostLocationDaoCache? GetCache() => _cache as IPostLocationDaoCache;

        protected override Task ValidateEntityForInsert(params PostLocation[] obj)
        {
            foreach (PostLocation postLocation in obj)
            {
                if (postLocation.PostId == Guid.Empty)
                    throw new EntityValidationException(nameof(PostLocation), "A localização do post deve estar associada a um post válido.", ErrorCodes.POST_NOT_FOUND);
                if (postLocation.Position == null)
                    throw new EntityValidationException(nameof(PostLocation), "A localização do post deve conter coordenadas válidas.", ErrorCodes.INVALID_POSITION);
            }
            return Task.CompletedTask;
        }

        protected override Task ValidateEntityForUpdate(params PostLocation[] obj)
        {
            return ValidateEntityForInsert(obj);
        }

        public async Task<List<PostLocation>> GetPostsWithinRadiusAsync(Point center, double radiusKm, int? itemsCount = null, int? pageNumber = null)
        {
            // Convert km to meters for PostGIS distance function
            double radiusMeters = radiusKm * 1000;

            IQueryable<PostLocation> query = _context.Set<PostLocation>()
                .Where(pl => pl.Position.Distance(center) <= radiusMeters)
                .OrderBy(pl => pl.Position.Distance(center));

            if (itemsCount.HasValue && pageNumber.HasValue && itemsCount > 0 && pageNumber > 0)
            {
                int skip = (pageNumber.Value - 1) * itemsCount.Value;
                query = query.Skip(skip).Take(itemsCount.Value);
            }
            else if (itemsCount.HasValue && itemsCount > 0)
            {
                query = query.Take(itemsCount.Value);
            }

            return await query.ToListAsync();
        }

        public Task<List<PostLocation>> GetPostsByLocationAsync(string? city = null, string? state = null, string? country = null, int? itemsCount = null, int? pageNumber = null)
        {
            // Note: This method would require additional fields in PostLocation entity to store city, state, country
            // For now, returning empty list as the current schema doesn't support text-based location filtering
            // This would need to be populated from the PositionDTO data when creating posts
            return Task.FromResult(new List<PostLocation>());
        }

        public async Task<PostLocation?> GetPostLocationByPostIdAsync(Guid postId)
        {
            return await _context.Set<PostLocation>()
                .FirstOrDefaultAsync(pl => pl.PostId == postId);
        }
    }
}