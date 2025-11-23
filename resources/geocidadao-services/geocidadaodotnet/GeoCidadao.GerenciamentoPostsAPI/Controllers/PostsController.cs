using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Models.OAuth;
using GeoCidadao.OAuth.Attributes;
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
        /// <param name="itemsCount">Número máximo de posts a serem retornados</param>
        /// <param name="pageNumber">Número da página (iniciando em 1)</param>
        /// <returns></returns>
        [HttpGet("{userId}/posts")]
        [Authorize(Policy = "Posts.Read")]
        public async Task<IActionResult> GetUserPosts(Guid userId, [FromQuery] int? itemsCount, [FromQuery] int? pageNumber)
        {
            List<PostDTO> posts = await _service.GetUserPostsAsync(userId, itemsCount, pageNumber);

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
        [Authorize(Policy = "Posts.Read")]
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
        [Authorize(Policy = "Posts.Create")]
        public async Task<IActionResult> CreateNewPost([FromForm] NewPostDTO newPost)
        {
            PostDTO createdPost = await _service.CreatePostAsync(newPost);

            return CreatedAtAction(nameof(GetPost), new { postId = createdPost.Id }, createdPost);
        }

        /// <summary>
        /// Atualizar post
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <param name="updatedPost">Dados do post atualizado</param>
        /// <returns></returns>
        [HttpPut("{postId}")]
        [OwnerOrPermissionByProperty<Post>("UserId", "Posts.Edit.Self", "Posts.Edit.Any")]
        public async Task<IActionResult> UpdatePost(Guid postId, [FromBody] UpdatePostDTO updatedPost)
        {
            await _service.UpdatePostAsync(postId, updatedPost);
            return NoContent();
        }

        /// <summary>
        /// Deletar post
        /// </summary>
        /// <param name="postId">Id do post</param>
        /// <returns></returns>
        [HttpDelete("{postId}")]
        [OwnerOrPermissionByProperty<Post>("UserId", "Posts.Delete.Self", "Moderators", "Posts.Delete.Any")]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            await _service.DeletePostAsync(postId);
            return NoContent();
        }


    }
}