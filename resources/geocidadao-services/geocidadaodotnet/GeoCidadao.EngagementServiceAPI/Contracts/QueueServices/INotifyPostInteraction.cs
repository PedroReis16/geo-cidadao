using GeoCidadao.EngagementServiceAPI.Models.DTOs;
using GeoCidadao.Models.Enums;

namespace GeoCidadao.EngagementServiceAPI.Contracts.QueueServices
{
    public interface INotifyPostInteraction
    {
        void NotifyPostInteraction(Guid postId, InteractionType interactionType);
        void NotifyPostReported(Guid postId, Guid reporterUserId, DelationDTO postReport);
    }
}