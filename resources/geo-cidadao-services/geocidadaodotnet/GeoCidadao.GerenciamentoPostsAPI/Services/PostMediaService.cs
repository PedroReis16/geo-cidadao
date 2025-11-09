using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Database.Contracts;
using GeoCidadao.Models.Constants;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Models.Enums;
using GeoCidadao.Models.Exceptions;
using GeoCidadao.Models.Extensions;
using GeoCidadao.Models.OAuth;

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

        public async Task UploadPostMediaAsync(Guid postId, IFormFile mediaFile)
        {
            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();

                Guid userId = _context!.User.GetUserId();

                Post? post = await scope.ServiceProvider.GetRequiredService<IPostDao>().FindAsync(postId);

                if (post == null)
                    throw new EntityValidationException(nameof(Post), $"Post '{postId}' não encontrado para adiçao de mídia", ErrorCodes.POST_NOT_FOUND);

                if (post.UserId != userId)
                {
                    if (!_context.User.Identities.Any(i => i.IsAuthenticated && i.HasClaim(c => c.Type == "role" && c.Value == "admin")))
                        throw new UnauthorizedAccessException($"Usuário '{userId}' não é proprietário do post '{postId}'");
                }

                _logger.LogInformation($"O post '{postId}' será atualizado com nova mídia pelo usuário '{userId}'");
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string errorMsg = $"Ocorreu um erro ao tentar adicionar mídia ao post '{postId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMsg, _context, new()
                {
                    { LogConstants.EntityId, postId },
                });
                throw new Exception(errorMsg, ex);
            }
        }

        public Task DeleteMediaPostAsync(Guid postId, Guid mediaId)
        {
            throw new NotImplementedException();
        }
    }
}