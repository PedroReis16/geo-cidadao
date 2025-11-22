using GeoCidadao.EngagementServiceAPI.Contracts;
using GeoCidadao.EngagementServiceAPI.Models.DTOs;
using GeoCidadao.Models.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.EngagementServiceAPI.Controllers
{
    [ApiController]
    [Route("interactions/{postId}")]
    [Authorize]
    public class PostInteractionController(IPostInteractionService interactionService) : ControllerBase
    {
        private readonly IPostInteractionService _interactionService = interactionService;

        /// <summary>
        /// Curtir um post
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <returns></returns>
        [HttpPost("like")]
        [Authorize(Policy = "Posts.Read")]
        public async Task<IActionResult> LikePost(Guid postId)
        {
            Guid userId = HttpContext.User.GetUserId();
            await _interactionService.LikePostAsync(postId, userId);
            return Created();
        }

        /// <summary>
        /// Remover curtida de um post
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <returns></returns>
        [HttpDelete("like")]
        [Authorize(Policy = "Posts.Read")]
        public async Task<IActionResult> UnlikePost(Guid postId)
        {
            Guid userId = HttpContext.User.GetUserId();
            await _interactionService.UnlikePostAsync(postId, userId);
            return NoContent();
        }


        /// <summary>
        /// Denunciar um post
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <returns></returns>
        [HttpPost("delation")]
        [Authorize(Policy = "Posts.Read")]
        public async Task<IActionResult> DelatePost(Guid postId, [FromBody] DelationDTO delationDetails)
        {
            Guid userId = HttpContext.User.GetUserId();
            await _interactionService.ReportPostAsync(postId, userId, delationDetails);

            return Created();
        }
    }
}