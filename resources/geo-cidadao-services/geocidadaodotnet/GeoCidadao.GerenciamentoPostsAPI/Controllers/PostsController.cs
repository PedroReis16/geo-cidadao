using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoPostsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        public async Task<IActionResult> CreateNewPost([FromHeader] Guid userId, [FromForm] NewPostDTO newPost)
        {
            await _service.CreatePostAsync(userId, newPost);

            return CreatedAtAction(nameof(GetPost), new { postId = Guid.NewGuid() }, null);
        }

        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost(Guid postId, [FromBody] UpdatePostDTO updatedPost)
        {
            await _service.UpdatePostAsync(postId, updatedPost);
            return NoContent();
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            await _service.DeletePostAsync(postId);
            return NoContent();
        }
    }
}