using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts;
using GeoCidadao.Model.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoPostsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PostsController(IPostService service) : ControllerBase
    {
        private readonly IPostService _service = service;

        [HttpGet("{userId}/posts")]
        public async Task<IActionResult> GetUserPosts(Guid userId)
        {

            List<PostDTO> posts = await _service.GetUserPostsAsync(userId);

            if (posts.Count == 0)
                return NoContent();

            return Ok(posts);
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPost(Guid postId)
        {
            PostDTO? post = await _service.GetPostAsync(postId);

            if (post == null)
                return NotFound();

            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewPost([FromBody] NewPostDTO newPost)
        {
            Guid userId = HttpContext.User.GetUserId();

            PostDTO createdPost = await _service.CreatePostAsync(userId, newPost);

            return CreatedAtAction(nameof(GetPost), new { postId = createdPost.Id }, createdPost);
        }


        /// <summary>
        /// Incrementar media para um post
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="mediaFile"></param>
        /// <returns></returns>
        [HttpPatch("{postId}/media")]
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



        [HttpDelete("{postId}/media/{mediaId}")]
        public async Task<IActionResult> DeletePostMedia(Guid postId, Guid mediaId)
        {
            Guid userId = HttpContext.User.GetUserId();

            // await _service.DeleteMediaPostAsync(userId, postId, mediaId);

            return NoContent();
        }

        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost(Guid postId, [FromBody] UpdatePostDTO updatedPost)
        {
            return NoContent();
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(Guid postId, [FromHeader] Guid userId)
        {
            await _service.DeletePostAsync(postId, userId);
            return NoContent();
        }
    }
}