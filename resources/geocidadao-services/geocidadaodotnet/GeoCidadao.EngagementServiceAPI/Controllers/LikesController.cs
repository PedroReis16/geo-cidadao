using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.EngagementServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LikesController : ControllerBase
    {

        /// <summary>
        /// Curtir um post
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <returns></returns>
        [HttpPost("{postId}/like")]
        [Authorize(Policy = "Posts.Read")]
        public async Task<IActionResult> LikePost(Guid postId)
        {
            // Guid userId = HttpContext.User.GetUserId();
            // PostLikeDTO like = await _interactionService.LikePostAsync(postId, userId);
            return Ok();
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
            // Guid userId = HttpContext.User.GetUserId();
            // await _interactionService.UnlikePostAsync(postId, userId);
            return NoContent();
        }
    }
}