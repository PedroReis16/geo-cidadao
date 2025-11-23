using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using GeoCidadao.OAuth.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoPostsAPI.Controllers
{
    [ApiController]
    [Route("posts/{postId}/media")]
    public class PostMediaController(IPostMediaService service) : ControllerBase
    {
        private readonly IPostMediaService _service = service;

        /// <summary>
        /// Buscar mídia do post
        /// </summary>
        ///<returns></returns>
        [HttpGet("{mediaId}")]
        public async Task<IActionResult> GetPostMediaUrl(Guid postId, Guid mediaId)
        {
            string mediaUrl = await _service.GetPostMediaUrlAsync(postId, mediaId);
            return Ok(mediaUrl);
        }

        /// <summary>
        /// Reordenar mídias de um post
        /// </summary>
        /// <param name="postId">Identificador do post</param>
        /// <param name="mediaIdsInOrder">Lista de IDs de mídias na ordem desejada</param>
        /// <returns></returns>
        [HttpPatch("reorder")]
        [Authorize]
        [OwnerOrPermissionByProperty<Post>("UserId", "Posts.Edit.Self", "Posts.Edit.Any")]
        public async Task<IActionResult> ReorderPostMedias(Guid postId, [FromBody] List<Guid> mediaIdsInOrder)
        {
            await _service.ReorderPostMediasAsync(postId, mediaIdsInOrder);

            return Ok();
        }

        /// <summary>
        /// Deletar mídia de um post
        /// </summary>
        /// <param name="postId">Identificador do post</param>
        /// <param name="mediaId">Identificador da mídia</param>
        /// <returns></returns>
        [HttpDelete("{mediaId}")]
        [Authorize]
        [OwnerOrPermissionByProperty<Post>("UserId", "Posts.Edit.Self", "Posts.Edit.Any")]
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