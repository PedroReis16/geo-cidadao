using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts;

namespace GeoCidadao.GerenciamentoPostsAPI.Contracts
{
    public interface IPostService
    {
        Task CreatePostAsync(Guid userId, NewPostDTO newPost);
        Task DeletePostAsync(Guid postId);
        Task<PostDTO?> GetPostAsync(Guid postId);
        Task<List<PostDTO>> GetUserPostsAsync(Guid userId);
        Task UpdatePostAsync(Guid postId, UpdatePostDTO updatedPost);
    }
}