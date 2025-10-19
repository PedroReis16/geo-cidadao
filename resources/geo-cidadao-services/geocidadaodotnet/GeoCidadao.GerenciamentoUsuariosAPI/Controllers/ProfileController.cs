using GeoCidadao.GerenciamentoUsuariosAPI.Contracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfileController(IUserProfileService service) : ControllerBase
    {
        private readonly IUserProfileService _service = service;

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProfile(Guid userId)
        {
            UserDTO? user = await _service.GetUserProfileAsync(userId);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUserProfile(Guid userId, [FromBody] UpdateUserDTO updatedProfile)
        {
            await _service.UpdateUserProfileAsync(userId, updatedProfile);
            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUserProfile(Guid userId)
        {
            await _service.DeleteUserProfileAsync(userId);
            return NoContent();
        }
    }
}