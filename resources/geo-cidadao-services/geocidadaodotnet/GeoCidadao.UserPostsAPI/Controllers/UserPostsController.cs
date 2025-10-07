using GeoCidadao.UserPostsAPI.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.UserPostsAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserPostsController(IUserPostsService service) : ControllerBase
{
    private readonly IUserPostsService _service = service;

    [HttpPost("post")]
    public async Task<IActionResult> CreatePost()
    {
        await _service.CreatePostAsync();

        return Created();
    }

    [HttpPut("post/{postId}")]
    public async Task<IActionResult> UpdatePost(Guid postId)
    {
        await _service.UpdatePostAsync(postId);

        return Ok();
    }

    [HttpDelete("post/{postId}")]
    public async Task<IActionResult> DeletePost(Guid postId)
    {
        await _service.DeletePostAsync(postId);

        return NoContent();
    }
}