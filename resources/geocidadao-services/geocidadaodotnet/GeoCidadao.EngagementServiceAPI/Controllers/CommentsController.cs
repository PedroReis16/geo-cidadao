using GeoCidadao.EngagementServiceAPI.Contracts;
using GeoCidadao.EngagementServiceAPI.Models.DTOs.Comments;
using GeoCidadao.Models.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.EngagementServiceAPI.Controllers
{
    [ApiController]
    [Route("{postId}/[controller]")]
    [Authorize]
    public class CommentsController(IPostCommentsService service) : ControllerBase
    {
        private readonly IPostCommentsService _service = service;

        /// <summary>
        /// Obtém os comentários de um post específico.
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <param name="itemsCount">Total de itens por consulta</param>
        /// <param name="pageNumber">Número da página</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPostCommentsAsync(Guid postId, [FromQuery] int? itemsCount, [FromQuery] int? pageNumber, CancellationToken cancellationToken)
        {
            List<CommentDTO>? comments = await _service.GetPostCommentsAsync(postId, itemsCount, pageNumber, cancellationToken);

            if (comments == null || comments.Count == 0)
                return NoContent();


            return Ok(comments);
        }

        /// <summary>
        /// Adiciona um novo comentário a um post.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddPostComment(Guid postId, [FromBody] NewCommentDTO newComment, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.User.GetUserId();

            CommentDTO createdComment = await _service.AddPostCommentAsync(postId, userId, newComment, cancellationToken);

            return Created();
        }

        /// <summary>
        /// Atualiza um comentário existente.
        /// </summary>
        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdatePostComment(Guid postId, Guid commentId, [FromBody] UpdateCommentDTO updatedComment, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.User.GetUserId();

            CommentDTO comment = await _service.UpdatePostCommentAsync(postId, commentId, updatedComment, cancellationToken);

            return Ok(comment);
        }

        /// <summary>
        /// Remove um comentário de um post.
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <param name="commentId">Id do comentário</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeletePostCommentAsync(Guid postId, Guid commentId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.User.GetUserId();
            await _service.DeletePostCommentAsync(postId, commentId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Adiciona um like a um comentário.
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <param name="commentId">Id do comentário</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPatch("{commentId}/like")]
        public async Task<IActionResult> LikePostCommentAsync(Guid postId,  Guid commentId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.User.GetUserId();
            await _service.LikePostCommentAsync(postId, commentId, userId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Remove um like de um comentário.
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <param name="commentId">Id do comentário</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPatch("{commentId}/unlike")]
        public async Task<IActionResult> UnlikePostCommentAsync(Guid postId, Guid commentId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.User.GetUserId();
            await _service.UnlikePostCommentAsync(postId, commentId, userId, cancellationToken);
            return NoContent();
        }
    }
}