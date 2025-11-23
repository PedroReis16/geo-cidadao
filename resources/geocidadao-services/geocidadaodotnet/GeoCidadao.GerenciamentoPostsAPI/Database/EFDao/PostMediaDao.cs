using GeoCidadao.Database;
using GeoCidadao.Database.EFDao;
using GeoCidadao.GerenciamentoPostsAPI.Database.CacheContracts;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Models.Enums;
using GeoCidadao.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GeoCidadao.GerenciamentoPostsAPI.Database.EFDao
{
    internal class PostMediaDao(GeoDbContext context, IPostMediaDaoCache? cache = null) : BaseDao<PostMedia>(context, cache), IPostMediaDao
    {
        protected override IPostMediaDaoCache? GetCache() => _cache as IPostMediaDaoCache;

        protected override Task ValidateEntityForInsert(params PostMedia[] obj)
        {
            foreach (PostMedia media in obj)
            {
                if (media.Post == null || media.Post.Id == Guid.Empty)
                    throw new EntityValidationException(nameof(PostMedia), "O post associado à mídia é obrigatório.", ErrorCodes.POST_NOT_FOUND);
                if (string.IsNullOrWhiteSpace(media.MediaType))
                    throw new EntityValidationException(nameof(PostMedia), "O tipo de mídia é obrigatório.", ErrorCodes.POST_MEDIA_TYPE_REQUIRED);
                if (media.FileSize <= 0)
                    throw new EntityValidationException(nameof(PostMedia), "O tamanho do arquivo de mídia deve ser maior que zero.", ErrorCodes.CONTENT_REQUIRED);
            }
            return Task.CompletedTask;
        }

        protected override Task ValidateEntityForUpdate(params PostMedia[] obj)
        {
            return Task.CompletedTask;
        }

        public override async Task<PostMedia?> FindAsync(object key, bool track = false)
        {
            PostMedia? result = null;

            if (_cache != null && !track)
            {
                result = _cache.GetEntity(key.ToString()!);
                if (result != null)
                    return result;
            }

            IQueryable<PostMedia> query = _context.Set<PostMedia>().Where(pm => pm.Id == (Guid)key).Include(pm => pm.Post);

            if (!track)
                query = query.AsNoTracking();

            result = await query.FirstOrDefaultAsync();

            if (result != null && !track)
                _cache?.AddEntity(result);

            return result;
        }

        public override async Task<int> AddAsync(params PostMedia[] obj)
        {
            await ValidateEntityForInsert(obj);
            DbSet<PostMedia> dbSet = _context.Set<PostMedia>();

            foreach (PostMedia media in obj)
            {
                Post? trackedPost = _context.Set<Post>().Where(p => p.Id == media.Post.Id).FirstOrDefault();

                if (trackedPost == null)
                    throw new EntityValidationException(nameof(PostMedia), $"O post '{media.Post.Id}' associado à mídia não foi encontrado.", ErrorCodes.POST_NOT_FOUND);

                media.Post = trackedPost;
                dbSet.Add(media);
            }
            return await _context.SaveChangesAsync();
        }

        public Task<List<PostMedia>> GetPostMediasAsync(Guid postId)
        {
            IQueryable<PostMedia> query = _context.Set<PostMedia>()
                .Where(pm => pm.Post.Id == postId)
                .OrderBy(pm => pm.Order);

            return query.ToListAsync();
        }

        public Task UpdateMediaOrderAsync(Guid mediaId, int newOrder)
        {
            IQueryable<PostMedia> query = _context.Set<PostMedia>()
                .Where(pm => pm.Id == mediaId);

            return query.ExecuteUpdateAsync(pm => pm.SetProperty(p => p.Order, newOrder));
        }

        public override async Task<int> DeleteAsync(params PostMedia[] obj)
        {
            DbSet<PostMedia> dbSet = _context.Set<PostMedia>();

            Guid postId = obj.Length > 0 ? obj[0].Post.Id : Guid.Empty;

            if (postId == Guid.Empty)
                throw new EntityValidationException(nameof(PostMedia), "O post associado à mídia é obrigatório para deleção.", ErrorCodes.POST_NOT_FOUND);

            List<PostMedia> postMedias = await _context.Set<PostMedia>()
                .Where(pm => pm.Post.Id == postId)
                .OrderBy(pm => pm.Order)
                .ToListAsync();

            foreach (PostMedia media in obj)
            {
                PostMedia? nextMedia = postMedias
                    .Where(pm => pm.Order > media.Order)
                    .OrderBy(pm => pm.Order)
                    .FirstOrDefault();

                if (nextMedia != null)
                {
                    // Ajusta a ordem das outras mídias
                    foreach (var pm in postMedias.Where(pm => pm.Order > media.Order))
                    {
                        pm.Order -= 1;
                        dbSet.Update(pm);
                    }
                }
                dbSet.Remove(media);
            }
            return await _context.SaveChangesAsync();
        }
    }
}