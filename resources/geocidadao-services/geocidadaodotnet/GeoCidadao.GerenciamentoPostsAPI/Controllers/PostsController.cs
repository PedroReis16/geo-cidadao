using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Models.OAuth;
using GeoCidadao.OAuth.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoPostsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PostsController(IPostService service, IPostInteractionService interactionService) : ControllerBase
    {
        private readonly IPostService _service = service;
        private readonly IPostInteractionService _interactionService = interactionService;


        /// <summary>
        /// Obter posts de um usuário
        /// </summary>
        /// <param name="userId">Id do usuário</param>
        /// <param name="itemsCount">Número máximo de posts a serem retornados</param>
        /// <param name="pageNumber">Número da página (iniciando em 1)</param>
        /// <returns></returns>
        [HttpGet("{userId}/posts")]
        [Authorize(Policy = "Posts.Read")]
        public async Task<IActionResult> GetUserPosts(Guid userId, [FromQuery] int? itemsCount, [FromQuery] int? pageNumber)
        {
            List<PostDTO> posts = await _service.GetUserPostsAsync(userId, itemsCount, pageNumber);

            if (posts.Count == 0)
                return NoContent();

            return Ok(posts);
        }

        /// <summary>
        /// Obter posts por localização
        /// </summary>
        /// <param name="latitude">Latitude do ponto central</param>
        /// <param name="longitude">Longitude do ponto central</param>
        /// <param name="radiusKm">Raio de busca em quilômetros</param>
        /// <param name="city">Filtrar por cidade</param>
        /// <param name="state">Filtrar por estado</param>
        /// <param name="country">Filtrar por país</param>
        /// <param name="itemsCount">Número máximo de posts a serem retornados</param>
        /// <param name="pageNumber">Número da página (iniciando em 1)</param>
        /// <returns></returns>
        [HttpGet("by-location")]
        [Authorize(Policy = "Posts.Read")]
        public async Task<IActionResult> GetPostsByLocation(
            [FromQuery] double? latitude,
            [FromQuery] double? longitude,
            [FromQuery] double? radiusKm,
            [FromQuery] string? city,
            [FromQuery] string? state,
            [FromQuery] string? country,
            [FromQuery] int? itemsCount,
            [FromQuery] int? pageNumber)
        {
            var locationQuery = new LocationQueryDTO
            {
                Latitude = latitude,
                Longitude = longitude,
                RadiusKm = radiusKm,
                City = city,
                State = state,
                Country = country,
                ItemsCount = itemsCount,
                PageNumber = pageNumber
            };

            List<PostWithLocationDTO> posts = await _service.GetPostsByLocationAsync(locationQuery);

            if (posts.Count == 0)
                return NoContent();

            return Ok(posts);
        }

        /// <summary>
        /// Obter post por Id
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <returns></returns>
        [HttpGet("{postId}")]
        [Authorize(Policy = "Posts.Read")]
        public async Task<IActionResult> GetPost(Guid postId)
        {
            PostDTO? post = await _service.GetPostAsync(postId);

            if (post == null)
                return NotFound();

            return Ok(post);
        }

        /// <summary>
        /// Criar novo post
        /// </summary>
        /// <param name="newPost">Dados do novo post</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "Posts.Create")]
        public async Task<IActionResult> CreateNewPost([FromBody] NewPostDTO newPost)
        {
            Guid userId = HttpContext.User.GetUserId();

            PostDTO createdPost = await _service.CreatePostAsync(userId, newPost);

            return CreatedAtAction(nameof(GetPost), new { postId = createdPost.Id }, createdPost);
        }

        /// <summary>
        /// Atualizar post
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <param name="updatedPost">Dados do post atualizado</param>
        /// <returns></returns>
        [HttpPut("{postId}")]
        [OwnerOrPermissionByProperty<Post>("UserId", "Posts.Edit.Self", "Posts.Edit.Any")]
        public async Task<IActionResult> UpdatePost(Guid postId, [FromBody] UpdatePostDTO updatedPost)
        {
            await _service.UpdatePostAsync(postId, updatedPost);
            return NoContent();
        }

        /// <summary>
        /// Deletar post
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <returns></returns>
        [HttpDelete("{postId}")]
        [OwnerOrPermissionByProperty<Post>("UserId", "Posts.Delete.Self", "Moderators", "Posts.Delete.Any")]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            await _service.DeletePostAsync(postId);
            return NoContent();
        }

        /// <summary>
        /// Curtir um post
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <returns></returns>
        [HttpPost("{postId}/like")]
        [Authorize(Policy = "Posts.Read")]
        public async Task<IActionResult> LikePost(Guid postId)
        {
            Guid userId = HttpContext.User.GetUserId();
            PostLikeDTO like = await _interactionService.LikePostAsync(postId, userId);
            return Ok(like);
        }

        /// <summary>
        /// Remover curtida de um post
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <returns></returns>
        [HttpDelete("{postId}/like")]
        [Authorize(Policy = "Posts.Read")]
        public async Task<IActionResult> UnlikePost(Guid postId)
        {
            Guid userId = HttpContext.User.GetUserId();
            await _interactionService.UnlikePostAsync(postId, userId);
            return NoContent();
        }

        /// <summary>
        /// Criar comentário em um post
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <param name="createCommentDto">Dados do comentário</param>
        /// <returns></returns>
        [HttpPost("{postId}/comments")]
        [Authorize(Policy = "Posts.Read")]
        public async Task<IActionResult> CreateComment(Guid postId, [FromBody] CreateCommentDTO createCommentDto)
        {
            Guid userId = HttpContext.User.GetUserId();
            PostCommentDTO comment = await _interactionService.CreateCommentAsync(postId, userId, createCommentDto);
            return CreatedAtAction(nameof(GetPostComments), new { postId }, comment);
        }

        /// <summary>
        /// Editar comentário
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <param name="commentId">Id do comentário</param>
        /// <param name="updateCommentDto">Dados atualizados do comentário</param>
        /// <returns></returns>
        [HttpPut("{postId}/comments/{commentId}")]
        [Authorize(Policy = "Posts.Read")]
        public async Task<IActionResult> UpdateComment(Guid postId, Guid commentId, [FromBody] UpdateCommentDTO updateCommentDto)
        {
            Guid userId = HttpContext.User.GetUserId();
            PostCommentDTO comment = await _interactionService.UpdateCommentAsync(postId, commentId, userId, updateCommentDto);
            return Ok(comment);
        }

        /// <summary>
        /// Deletar comentário
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <param name="commentId">Id do comentário</param>
        /// <returns></returns>
        [HttpDelete("{postId}/comments/{commentId}")]
        [Authorize(Policy = "Posts.Read")]
        public async Task<IActionResult> DeleteComment(Guid postId, Guid commentId)
        {
            Guid userId = HttpContext.User.GetUserId();
            bool isModerator = HttpContext.User.IsInRole("Moderators");
            
            await _interactionService.DeleteCommentAsync(postId, commentId, userId, isModerator);
            return NoContent();
        }

        /// <summary>
        /// Listar comentários de um post
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <param name="itemsCount">Número máximo de comentários a serem retornados</param>
        /// <param name="pageNumber">Número da página (iniciando em 1)</param>
        /// <returns></returns>
        [HttpGet("{postId}/comments")]
        [Authorize(Policy = "Posts.Read")]
        public async Task<IActionResult> GetPostComments(Guid postId, [FromQuery] int? itemsCount, [FromQuery] int? pageNumber)
        {
            List<PostCommentDTO> comments = await _interactionService.GetPostCommentsAsync(postId, itemsCount, pageNumber);

            if (comments.Count == 0)
                return NoContent();

            return Ok(comments);
        }
    }
}