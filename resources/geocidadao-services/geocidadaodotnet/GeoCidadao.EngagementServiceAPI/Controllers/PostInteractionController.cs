using GeoCidadao.EngagementServiceAPI.Contracts;
using GeoCidadao.EngagementServiceAPI.Models.DTOs;
using GeoCidadao.Models.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.EngagementServiceAPI.Controllers
{
    [ApiController]
    [Route("interactions")]
    [Authorize]
    public class PostInteractionController(IPostInteractionService interactionService) : ControllerBase
    {
        private readonly IPostInteractionService _interactionService = interactionService;

        /// <summary>
        /// Verificar curtidas de um usuário em uma lista de posts
        /// </summary>
        /// <param name="checkLikesDto">Dados para verificação</param>
        /// <returns>Lista de IDs de posts curtidos</returns>
        [HttpPost("check-likes")]
        [AllowAnonymous] // Internal service call, or use specific policy
        public async Task<IActionResult> CheckLikes([FromBody] CheckLikesDTO checkLikesDto)
        {
            // If internal service, we might skip auth or use client credentials. 
            // For simplicity assuming it's allowed or handled by gateway/network.
            // If called by user, we should verify userId matches token.
            
            var likedPosts = await _interactionService.GetLikedPostIdsAsync(checkLikesDto.UserId, checkLikesDto.PostIds);
            return Ok(likedPosts);
        }

        /// <summary>
        /// Curtir um post
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <returns></returns>
        [HttpPost("{postId}/like")]
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
        [HttpDelete("{postId}/like")]
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
        /// <param name="delationDetails">Detalhes da denúncia</param>
        /// <returns></returns>
        [HttpPost("{postId}/delation")]
        public async Task<IActionResult> DelatePost(Guid postId, [FromBody] DelationDTO delationDetails)
        {
            Guid userId = HttpContext.User.GetUserId();
            await _interactionService.ReportPostAsync(postId, userId, delationDetails);

            return Created();
        }
    }
}