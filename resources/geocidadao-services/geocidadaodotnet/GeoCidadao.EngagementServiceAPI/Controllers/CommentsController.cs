using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.EngagementServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentsController : ControllerBase
    {

        // /// <summary>
        // /// Criar comentário em um post
        // /// </summary>
        // /// <param name="postId">Id do post</param>
        // /// <param name="createCommentDto">Dados do comentário</param>
        // /// <returns></returns>
        // [HttpPost("{postId}/comments")]
        // [Authorize(Policy = "Posts.Read")]
        // public async Task<IActionResult> CreateComment(Guid postId, [FromBody] CreateCommentDTO createCommentDto)
        // {
        //     // Guid userId = HttpContext.User.GetUserId();
        //     // PostCommentDTO comment = await _interactionService.CreateCommentAsync(postId, userId, createCommentDto);
        //     // return CreatedAtAction(nameof(GetPostComments), new { postId }, comment);
        
        //     return Ok();
        // }

        // /// <summary>
        // /// Editar comentário
        // /// </summary>
        // /// <param name="postId">Id do post</param>
        // /// <param name="commentId">Id do comentário</param>
        // /// <param name="updateCommentDto">Dados atualizados do comentário</param>
        // /// <returns></returns>
        // [HttpPut("{postId}/comments/{commentId}")]
        // [Authorize(Policy = "Posts.Read")]
        // public async Task<IActionResult> UpdateComment(Guid postId, Guid commentId, [FromBody] UpdateCommentDTO updateCommentDto)
        // {
        //     // Guid userId = HttpContext.User.GetUserId();
        //     // PostCommentDTO comment = await _interactionService.UpdateCommentAsync(postId, commentId, userId, updateCommentDto);
        //     // return Ok(comment);
        //     return Ok();
        // }

        // /// <summary>
        // /// Deletar comentário
        // /// </summary>
        // /// <param name="postId">Id do post</param>
        // /// <param name="commentId">Id do comentário</param>
        // /// <returns></returns>
        // [HttpDelete("{postId}/comments/{commentId}")]
        // [Authorize(Policy = "Posts.Read")]
        // public async Task<IActionResult> DeleteComment(Guid postId, Guid commentId)
        // {
        //     // Guid userId = HttpContext.User.GetUserId();
        //     // bool isModerator = HttpContext.User.IsInRole("Moderators");

        //     // await _interactionService.DeleteCommentAsync(postId, commentId, userId, isModerator);
        //     return NoContent();
        // }

        // /// <summary>
        // /// Listar comentários de um post
        // /// </summary>
        // /// <param name="postId">Id do post</param>
        // /// <param name="itemsCount">Número máximo de comentários a serem retornados</param>
        // /// <param name="pageNumber">Número da página (iniciando em 1)</param>
        // /// <returns></returns>
        // [HttpGet("{postId}/comments")]
        // [Authorize(Policy = "Posts.Read")]
        // public async Task<IActionResult> GetPostComments(Guid postId, [FromQuery] int? itemsCount, [FromQuery] int? pageNumber)
        // {
        //     // List<PostCommentDTO> comments = await _interactionService.GetPostCommentsAsync(postId, itemsCount, pageNumber);

        //     // if (comments.Count == 0)
        //     //     return NoContent();

        //     // return Ok(comments);
        
        //     return Ok();
        // }
    }
}