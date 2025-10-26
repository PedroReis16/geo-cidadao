using GeoCidadao.Database;
using GeoCidadao.Database.EFDao;
using GeoCidadao.GerenciamentoPostsAPI.Database.CacheContracts;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.Model.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Model.Enums;
using GeoCidadao.Model.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GeoCidadao.GerenciamentoPostsAPI.Database.EFDao
{
    internal class PostDao(GeoDbContext context, IPostDaoCache? cache = null) : BaseDao<Post>(context, cache), IPostDao
    {
        protected override IPostDaoCache? GetCache() => _cache as IPostDaoCache;

        protected override Task ValidateEntityForInsert(params Post[] obj)
        {
            foreach (Post post in obj)
            {
                if (post.UserId == Guid.Empty)
                    throw new EntityValidationException(nameof(Post), "O post deve estar associado a um usuário antes de ser salvo.", ErrorCodes.USER_ID_REQUIRED);
                if (string.IsNullOrEmpty(post.Content))
                    throw new EntityValidationException(nameof(Post), "O post deve conter algum tipo de conteúdo antes de ser salvo", ErrorCodes.CONTENT_REQUIRED);
            }
            return Task.CompletedTask;
        }

        protected override Task ValidateEntityForUpdate(params Post[] obj)
        {
            return Task.CompletedTask;
        }

        public override async Task<int> AddAsync(params Post[] obj)
        {
            DbSet<Post> postsSet = _context.Set<Post>();
            await ValidateEntityForInsert(obj);

            foreach (Post post in obj)
            {
                postsSet.Add(post);
            }

            return await _context.SaveChangesAsync();
        }
    }
}