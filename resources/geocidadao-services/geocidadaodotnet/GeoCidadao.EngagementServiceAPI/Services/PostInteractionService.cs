using GeoCidadao.EngagementServiceAPI.Contracts;
using GeoCidadao.EngagementServiceAPI.Contracts.QueueServices;
using GeoCidadao.EngagementServiceAPI.Database.Contracts;
using GeoCidadao.EngagementServiceAPI.Models.DTOs;
using GeoCidadao.Models.Entities.EngagementServiceAPI;
using GeoCidadao.Models.Enums;
using GeoCidadao.Models.Exceptions;

namespace GeoCidadao.EngagementServiceAPI.Services
{
    public class PostInteractionService(ILogger<PostInteractionService> logger, IServiceScopeFactory serviceScopeFactory) : IPostInteractionService
    {
        private readonly ILogger<PostInteractionService> _logger = logger;
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

        public async Task LikePostAsync(Guid postId, Guid userId)
        {
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IPostLikesDao postLikesDao = scope.ServiceProvider.GetRequiredService<IPostLikesDao>();

                PostLike newLike = new()
                {
                    PostId = postId,
                    UserId = userId,
                };
                await postLikesDao.AddAsync(newLike);

                _logger.LogInformation($"Usuário '{userId}' curtiu o post '{postId}'");

                NotifyPostInteraction(postId, InteractionType.PostLike);
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMessage = $"Ocorreu um erro ao curtir o post '{postId}' pelo usuário '{userId}': {ex.Message}";
                _logger.LogError(ex, errorMessage);
                throw new Exception(errorMessage, ex);
            }
        }

        public async Task UnlikePostAsync(Guid postId, Guid userId)
        {
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IPostLikesDao postLikesDao = scope.ServiceProvider.GetRequiredService<IPostLikesDao>();

                await postLikesDao.RemovePostLikeAsync(postId, userId);

                _logger.LogInformation($"Usuário '{userId}' removeu a curtida do post '{postId}' ");

                NotifyPostInteraction(postId, InteractionType.PostUnlike);
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMessage = $"Ocorreu um erro ao remover a curtida do post '{postId}' pelo usuário '{userId}': {ex.Message}";
                _logger.LogError(ex, errorMessage);
                throw new Exception(errorMessage, ex);
            }
        }

        public Task ReportPostAsync(Guid postId, Guid reporterUserId, DelationDTO delationDetails)
        {
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                INotifyPostInteraction notifyService = scope.ServiceProvider.GetRequiredService<INotifyPostInteraction>();

                notifyService.NotifyPostReported(postId, reporterUserId, delationDetails);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Erro ao notificar a denúncia sobre o post '{postId}': {ex.Message}";
                _logger.LogError(ex, errorMessage);
                throw new Exception(errorMessage, ex);
            }
        }

        private void NotifyPostInteraction(Guid postId, InteractionType interactionType)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var notifyService = scope.ServiceProvider.GetRequiredService<INotifyPostInteraction>();
            notifyService.NotifyPostInteraction(postId, interactionType);
        }
    }
}