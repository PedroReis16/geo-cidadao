using GeoCidadao.FeedServiceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GeoCidadao.FeedServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class FeedController(FeedService feedService) : ControllerBase
    {
        private readonly FeedService _feedService = feedService;

        [HttpGet]
        public async Task<IActionResult> GetFeed([FromQuery] double? lat, [FromQuery] double? lon, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim);
            var feed = await _feedService.GetFeedAsync(userId, lat, lon, page, pageSize);
            return Ok(feed);
        }
    }
}
