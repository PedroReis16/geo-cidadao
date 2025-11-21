using GeoCidadao.Database.Contracts;
using GeoCidadao.Models.Entities.EngagementServiceAPI;

namespace GeoCidadao.EngagementServiceAPI.Database.Contracts
{
    public interface IPostCommentsDao : IRepository<PostComment>
    {
    }
}