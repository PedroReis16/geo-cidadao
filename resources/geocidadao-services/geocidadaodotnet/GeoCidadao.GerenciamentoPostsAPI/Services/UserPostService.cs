using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Models.Extensions;

namespace GeoCidadao.GerenciamentoPostsAPI.Services
{
    internal class UserPostService(ILogger<UserPostService> logger, IServiceProvider serviceProvider) : IUserPostService
    {
        private readonly ILogger<UserPostService> _logger = logger;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public async Task RemoveAllUserContentAsync(Guid userId)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            IPostDao postDao = scope.ServiceProvider.GetRequiredService<IPostDao>();

            List<Post> userContent = await postDao.GetUserContentAsync(userId);

            List<Task> deleteTasks = new();

            foreach (Post post in userContent)
            {
                deleteTasks.Add(Task.Run(async () =>
                {
                    using IServiceScope taskScope = _serviceProvider.CreateScope();
                    IMediaBucketService mediaBucketService = taskScope.ServiceProvider.GetRequiredService<IMediaBucketService>();
                    
                    try
                    {
                        if (post.Medias?.Count > 0)
                        {
                            foreach(var media in post.Medias)
                            {
                                deleteTasks.Add(mediaBucketService.DeleteMediaAsync(post.Id, media.Id, media.MediaType));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Erro ao deletar o post de ID {post.Id} do usuário de ID {userId}: {ex.GetFullMessage()}.");
                    }
                    finally
                    {
                        INotifyPostChangedService notifyService = scope.ServiceProvider.GetRequiredService<INotifyPostChangedService>();
                        notifyService.NotifyPostDeleted(post.Id);
                    }
                }));
            }
            
            await Task.WhenAll(deleteTasks);
            await postDao.DeleteUserPostsAsync(userId);
            _logger.LogInformation($"Foram deletados {userContent.Count} posts do usuário de ID {userId}.");
        }
    }
}