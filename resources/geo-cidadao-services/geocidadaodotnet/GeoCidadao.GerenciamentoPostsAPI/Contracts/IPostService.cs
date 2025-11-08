using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace GeoCidadao.GerenciamentoPostsAPI.Contracts
{
    public interface IPostService
    {
        Task<PostDTO> CreatePostAsync(Guid userId, NewPostDTO newPost);
        Task DeletePostAsync(Guid userId, Guid postId);
        Task<PostDTO?> GetPostAsync(Guid postId);
        Task<List<PostDTO>> GetUserPostsAsync(Guid userId);
        Task UpdatePostAsync(Guid postId, UpdatePostDTO updatedPost);
        Task UploadPostMediaAsync(Guid postId, IFormFile mediaFile);
    }
}