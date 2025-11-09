using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using GeoCidadao.OAuth.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoPostsAPI.Middlewares
{
    public class PostFetcher(IPostDao postDao) : IResourceFetcher<Post>
    {
        private readonly IPostDao _postDao = postDao;   

        public async Task<Post?> GetAsync(ActionContext ctx, CancellationToken ct = default)
        {
            if (!ctx.RouteData.Values.TryGetValue("postId", out object? postId)) return null;

            return await _postDao.FindAsync((Guid)postId!);
        }
    }
}