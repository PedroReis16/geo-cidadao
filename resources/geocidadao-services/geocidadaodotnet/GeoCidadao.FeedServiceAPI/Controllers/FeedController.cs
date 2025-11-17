using GeoCidadao.FeedServiceAPI.Contracts;
using GeoCidadao.FeedServiceAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.FeedServiceAPI.Controllers
{
    /// <summary>
    /// Controller para gerenciamento do feed de usuários
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class FeedController : ControllerBase
    {
        private readonly IFeedService _feedService;
        private readonly ILogger<FeedController> _logger;

        public FeedController(IFeedService feedService, ILogger<FeedController> logger)
        {
            _feedService = feedService;
            _logger = logger;
        }

        /// <summary>
        /// Obtém o feed geral de posts
        /// </summary>
        /// <param name="pageSize">Número de posts por página (padrão: 20, máximo: 100)</param>
        /// <param name="page">Número da página (padrão: 1)</param>
        /// <returns>Lista de posts do feed</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<FeedPostDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetFeed([FromQuery] int pageSize = 20, [FromQuery] int page = 1)
        {
            // Valida parâmetros
            if (page < 1)
                page = 1;
            
            if (pageSize < 1)
                pageSize = 20;
            
            if (pageSize > 100)
                pageSize = 100;

            var feed = await _feedService.GetFeedAsync(pageSize, page);

            if (feed.Count == 0)
                return NoContent();

            return Ok(feed);
        }

        /// <summary>
        /// Obtém o feed de posts de um usuário específico
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <param name="pageSize">Número de posts por página (padrão: 20, máximo: 100)</param>
        /// <param name="page">Número da página (padrão: 1)</param>
        /// <returns>Lista de posts do feed do usuário</returns>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(List<FeedPostDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetUserFeed(Guid userId, [FromQuery] int pageSize = 20, [FromQuery] int page = 1)
        {
            // Valida parâmetros
            if (page < 1)
                page = 1;
            
            if (pageSize < 1)
                pageSize = 20;
            
            if (pageSize > 100)
                pageSize = 100;

            var feed = await _feedService.GetUserFeedAsync(userId, pageSize, page);

            if (feed.Count == 0)
                return NoContent();

            return Ok(feed);
        }

        /// <summary>
        /// Invalida o cache do feed (requer permissões administrativas)
        /// </summary>
        /// <returns>Confirmação de invalidação</returns>
        [HttpPost("invalidate-cache")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> InvalidateCache()
        {
            await _feedService.InvalidateFeedCacheAsync();
            return Ok(new { message = "Cache invalidado com sucesso" });
        }
    }
}
