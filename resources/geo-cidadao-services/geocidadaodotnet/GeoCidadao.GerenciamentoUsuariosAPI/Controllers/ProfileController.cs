using GeoCidadao.GerenciamentoUsuariosAPI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController(IProfileService service) : ControllerBase
    {
        private readonly IProfileService _service = service;

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProfile(Guid userId)
        {
            return Ok();
        }

        [HttpGet("{userId}/photo")]
        public async Task<IActionResult> GetUserPhoto(Guid userId)
        {
            return Ok();
        }

        [HttpPatch("{userId}/photo")]
        public async Task<IActionResult> UpdateUserPhoto(Guid userId, [FromBody] string photoBase64)
        {
            return NoContent();
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