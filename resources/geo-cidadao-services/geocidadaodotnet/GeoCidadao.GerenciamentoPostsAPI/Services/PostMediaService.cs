using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.Models.Constants;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Models.Enums;
using GeoCidadao.Models.Exceptions;
using GeoCidadao.Models.Extensions;

namespace GeoCidadao.GerenciamentoPostsAPI.Services
{
    public class PostMediaService(
        ILogger<PostMediaService> logger,
        IHttpContextAccessor? contextAccessor,
        IServiceScopeFactory scopeFactory) : IPostMediaService
    {
        private readonly ILogger<PostMediaService> _logger = logger;
        private readonly HttpContext? _context = contextAccessor?.HttpContext;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public async Task UploadPostMediaAsync(Guid postId, int order, IFormFile mediaFile)
        {
            Guid mediaId = Guid.NewGuid();
            string fileExtension = string.Empty;

            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                IPostMediaDao postMediaDao = scope.ServiceProvider.GetRequiredService<IPostMediaDao>();
                IMediaBucketService mediaBucketService = scope.ServiceProvider.GetRequiredService<IMediaBucketService>();

                List<PostMedia> postMedias = await postMediaDao.GetPostMediasAsync(postId);

                if (postMedias.Count >= 10)
                    throw new EntityValidationException(nameof(PostMedia), $"O post '{postId}' já possui o número máximo de mídias permitidas (10).", ErrorCodes.POST_MEDIA_LIMIT_EXCEEDED);


                await mediaBucketService.UploadMediaAsync(postId, mediaId, mediaFile.OpenReadStream(), out fileExtension);

                _logger.LogInformation($"Mídia '{mediaId}' do post '{postId}' enviada com sucesso.");

                PostMedia postMedia = new PostMedia
                {
                    Id = mediaId,
                    Post = new Post { Id = postId },
                    MediaType = fileExtension,
                    FileSize = mediaFile.Length,
                };

                if (order == 0)
                {
                    postMedia.Order = postMedias.Count + 1;
                    await postMediaDao.AddAsync(postMedia);
                }
                else
                {
                    postMedia.Order = order;
                    await postMediaDao.AddAsync(postMedia);

                    // Reordenar as outras mídias conforme necessário
                    List<Task> updateTasks = new List<Task>();

                    var mediasToReorder = postMedias.Where(pm => pm.Order >= order).OrderBy(pm => pm.Order).ToList();
                    foreach (var media in mediasToReorder)
                    {
                        media.Order += 1;
                        updateTasks.Add(postMediaDao.UpdateMediaOrderAsync(media.Id, media.Order));
                    }
                    await Task.WhenAll(updateTasks);
                }

                NotifyPostChanged(postId);
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar adicionar mídia ao post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.EntityId, postId },
                });

                _ = DeleteMediaWithErrorAsync(postId, mediaId, fileExtension);

                throw new Exception(errorMsg, ex);
            }
        }

        private Task DeleteMediaWithErrorAsync(Guid postId, Guid mediaId, string mediaType)
        {
            if(string.IsNullOrEmpty(mediaType))
                return Task.CompletedTask;

            using IServiceScope scope = _scopeFactory.CreateScope();
            IMediaBucketService mediaBucketService = scope.ServiceProvider.GetRequiredService<IMediaBucketService>();
            return mediaBucketService.DeleteMediaAsync(postId, mediaId, mediaType);
        }

        public async Task DeleteMediaPostAsync(Guid postId, Guid mediaId)
        {
            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                IPostMediaDao postMediaDao = scope.ServiceProvider.GetRequiredService<IPostMediaDao>();
                IMediaBucketService mediaBucketService = scope.ServiceProvider.GetRequiredService<IMediaBucketService>();

                PostMedia? postMedia = await postMediaDao.FindAsync(mediaId);

                if (postMedia == null || postMedia.Post.Id != postId)
                    throw new EntityValidationException(nameof(PostMedia), $"Mídia '{mediaId}' não encontrada para o post '{postId}'.", ErrorCodes.POST_MEDIA_NOT_FOUND);

                await mediaBucketService.DeleteMediaAsync(postId, mediaId, postMedia.MediaType);

                _logger.LogInformation($"Mídia '{mediaId}' do post '{postId}' deletada com sucesso.");

                await postMediaDao.DeleteAsync(postMedia);

                NotifyPostChanged(postId);
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar deletar a mídia '{mediaId}' do post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.EntityId, postId },
                    { "PostMediaId", mediaId },
                });
                throw new Exception(errorMsg, ex);
            }
        }

        public async Task ReorderPostMediasAsync(Guid postId, List<Guid> mediaIdsInOrder)
        {
            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                IPostMediaDao postMediaDao = scope.ServiceProvider.GetRequiredService<IPostMediaDao>();

                List<PostMedia> postMedias = await postMediaDao.GetPostMediasAsync(postId);

                if (mediaIdsInOrder.Count != postMedias.Count || !mediaIdsInOrder.All(id => postMedias.Any(pm => pm.Id == id)))
                    throw new EntityValidationException(nameof(PostMedia), "A lista de IDs de mídia fornecida não corresponde às mídias existentes do post.", ErrorCodes.CONTENT_REQUIRED);

                for (int i = 0; i < mediaIdsInOrder.Count; i++)
                {
                    PostMedia media = postMedias.First(pm => pm.Id == mediaIdsInOrder[i]);
                    media.Order = i + 1; // Ordem iniciando em 1
                }
                await postMediaDao.UpdateAsync(postMedias.ToArray());

                NotifyPostChanged(postId);
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar reordenar as mídias do post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.EntityId, postId },
                });
                throw new Exception(errorMsg, ex);
            }
        }

        public async Task DeletePostMediasAsync(Guid postId)
        {
            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                IPostDao postDao = scope.ServiceProvider.GetRequiredService<IPostDao>();
                IPostMediaDao postMediaDao = scope.ServiceProvider.GetRequiredService<IPostMediaDao>();
                IMediaBucketService mediaBucketService = scope.ServiceProvider.GetRequiredService<IMediaBucketService>();

                Post? post = await postDao.FindAsync(postId);

                if (post == null)
                    throw new EntityValidationException(nameof(Post), $"Post '{postId}' não encontrado para deleção de mídia", ErrorCodes.POST_NOT_FOUND);

                List<PostMedia> postMedias = await postMediaDao.GetPostMediasAsync(postId);

                List<Task> deleteTasks = new List<Task>();

                foreach (var postMedia in postMedias)
                {
                    deleteTasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            await mediaBucketService.DeleteMediaAsync(post.Id, postMedia.Id, postMedia.MediaType);

                            _logger.LogInformation($"Mídia '{postMedia.Id}' do post '{postId}' deletada com sucesso.");
                        }
                        catch (Exception ex)
                        {
                            string errorMsg = $"Ocorreu um erro ao tentar deletar a mídia '{postMedia.Id}' do post '{postId}': {ex.GetFullMessage()}";
                            _logger.LogError(ex, errorMsg, _context, new()
                            {
                                { LogConstants.EntityId, postId },
                                { "PostMediaId", postMedia.Id },
                            });
                        }

                        await postMediaDao.DeleteAsync(postMedia.Id);
                        NotifyPostChanged(postId);
                    }));
                }

                await Task.WhenAll(deleteTasks);
            }
            catch (Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar deletar as mídias do post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.EntityId, postId },
                });
                throw new Exception(errorMsg, ex);
            }
        }

        private void NotifyPostChanged(Guid postId)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            INotifyPostChangedService notifyService = scope.ServiceProvider.GetRequiredService<INotifyPostChangedService>();

            notifyService.NotifyPostChanged(postId);
        }
    }
}