using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts;
using GeoCidadao.Models.OAuth;
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


        /// <summary>
        /// Obter posts de um usuário
        /// </summary>
        /// <param name="userId">Id do usuário</param>
        /// <returns></returns>
        [HttpGet("{userId}/posts")]
        public async Task<IActionResult> GetUserPosts(Guid userId)
        {

            List<PostDTO> posts = await _service.GetUserPostsAsync(userId);

            if (posts.Count == 0)
                return NoContent();

            return Ok(posts);
        }

        /// <summary>
        /// Obter post por Id
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <returns></returns>
        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPost(Guid postId)
        {
            PostDTO? post = await _service.GetPostAsync(postId);

            if (post == null)
                return NotFound();

            return Ok(post);
        }

        /// <summary>
        /// Criar novo post
        /// </summary>
        /// <param name="newPost">Dados do novo post</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateNewPost([FromBody] NewPostDTO newPost)
        {
            Guid userId = HttpContext.User.GetUserId();

            PostDTO createdPost = await _service.CreatePostAsync(userId, newPost);

            return CreatedAtAction(nameof(GetPost), new { postId = createdPost.Id }, createdPost);
        }       

        /// <summary>
        /// Atualizar post
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <param name="updatedPost">Dados do post atualizado</param>
        /// <returns></returns>
        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost(Guid postId, [FromBody] UpdatePostDTO updatedPost)
        {
            return NoContent();
        }

        /// <summary>
        /// Deletar post
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <returns></returns>
        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            await _service.DeletePostAsync(postId);
            return NoContent();
        }
    }
}