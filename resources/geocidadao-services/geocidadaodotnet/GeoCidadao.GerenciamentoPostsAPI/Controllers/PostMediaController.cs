using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using GeoCidadao.OAuth.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoPostsAPI.Controllers
{
    [ApiController]
    [Route("posts/{postId}/media")]
    [Authorize]
    public class PostMediaController(IPostMediaService service) : ControllerBase
    {
        private readonly IPostMediaService _service = service;

        /// <summary>
        /// Incrementar media para um post
        /// </summary>
        /// <param name="postId">Identificador do post</param>
        /// <param name="mediaFile">Arquivo de mídia</param>
        /// <param name="order">Ordem de exibição da mídia. Padrão 0. Se não for específicado, as mídias serão exibidas na ordem em que foram adicionadas.</param>
        /// <returns></returns>
        [HttpPatch]
        [OwnerOrPermissionByProperty<Post>("UserId", "Posts.Edit.Self")]
        public async Task<IActionResult> UploadPostMedia(Guid postId, [FromForm] IFormFile mediaFile, [FromForm] int order = 0)
        {
            await _service.UploadPostMediaAsync(postId, order, mediaFile);

            return Ok();
        }

        /// <summary>
        /// Reordenar mídias de um post
        /// </summary>
        /// <param name="postId">Identificador do post</param>
        /// <param name="mediaIdsInOrder">Lista de IDs de mídias na ordem desejada</param>
        /// <returns></returns>
        [HttpPatch("reorder")]
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