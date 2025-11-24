using GeoCidadao.FeedServiceAPI.Contracts;
using GeoCidadao.FeedServiceAPI.Models.DTOs;
using GeoCidadao.FeedServiceAPI.Services;
using GeoCidadao.Models.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GeoCidadao.FeedServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class FeedController(IFeedService feedService) : ControllerBase
    {
        private readonly IFeedService _feedService = feedService;

        [HttpGet]
        public async Task<IActionResult> GetFeed([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            Guid userId = HttpContext.User.GetUserId();

            List<PostDTO> posts = await _feedService.GetFeedAsync(userId, page, pageSize);
            return Ok(posts);
        }
    }
}
