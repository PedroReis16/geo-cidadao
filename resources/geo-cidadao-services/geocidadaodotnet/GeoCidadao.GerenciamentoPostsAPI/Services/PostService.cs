using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts;

namespace GeoCidadao.GerenciamentoPostsAPI.Services
{
    public class PostService : IPostService
    {
        public Task<PostDTO?> GetPostAsync(Guid postId)
        {
            throw new NotImplementedException();
        }

        public Task CreatePostAsync(Guid userId, NewPostDTO newPost)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePostAsync(Guid postId, UpdatePostDTO updatedPost)
        {
            throw new NotImplementedException();
        }

        public Task DeletePostAsync(Guid postId)
        {
            throw new NotImplementedException();
        }

        public Task<List<PostDTO>> GetUserPostsAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}