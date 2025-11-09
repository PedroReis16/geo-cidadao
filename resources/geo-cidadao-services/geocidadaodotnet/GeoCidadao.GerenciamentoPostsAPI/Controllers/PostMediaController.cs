using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.Models.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoPostsAPI.Controllers
{
    [ApiController]
    [Route("{postId}/media")]
    [Authorize]
    public class PostMediaController(IPostMediaService service) : ControllerBase
    {
        private readonly IPostMediaService _service = service;

        /// <summary>
        /// Incrementar media para um post
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="mediaFile"></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IActionResult> UploadPostMedia(Guid postId, [FromForm] IFormFile mediaFile)
        {
            try
            {
                await _service.UploadPostMediaAsync(postId, mediaFile);

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

        }


        /// <summary>
        /// Deletar mídia de um post
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        [HttpDelete("{mediaId}")]
        public async Task<IActionResult> DeletePostMedia(Guid postId, Guid mediaId)
        {
            try
            {
                await _service.DeleteMediaPostAsync(postId, mediaId);

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}