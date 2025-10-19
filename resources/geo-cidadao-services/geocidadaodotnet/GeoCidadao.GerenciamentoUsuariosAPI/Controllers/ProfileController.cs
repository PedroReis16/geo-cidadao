using GeoCidadao.GerenciamentoUsuariosAPI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController(IUserProfileService service) : ControllerBase
    {
        private readonly IUserProfileService _service = service;

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProfile(Guid userId)
        {
            return Ok();
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUserProfile(Guid userId, [FromBody] object updatedProfile)
        {
            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUserProfile(Guid userId)
        {
            return NoContent();
        }
    }
}