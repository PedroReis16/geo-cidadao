using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace GeoCidadao.GerenciamentoPostsAPI.Contracts
{
    public interface IPostService
    {
        Task<PostDTO> CreatePostAsync(Guid userId, NewPostDTO newPost);
        Task DeletePostAsync(Guid userId);
        Task<PostDTO?> GetPostAsync(Guid postId);
        Task<List<PostDTO>> GetUserPostsAsync(Guid userId, int? itemsCount=null, int? pageNumber=null);
        Task UpdatePostAsync(Guid postId, UpdatePostDTO updatedPost);
        Task<List<PostWithLocationDTO>> GetPostsByLocationAsync(LocationQueryDTO locationQuery);
        Task<PostWithLocationDTO?> GetPostWithLocationAsync(Guid postId);
    }
}