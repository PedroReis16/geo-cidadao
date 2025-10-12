using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoCidadao.UserPostsAPI.Contracts
{
    public interface IUserPostsService
    {
        Task CreatePostAsync();
        Task DeletePostAsync(Guid postId);
        Task UpdatePostAsync(Guid id);
    }
}