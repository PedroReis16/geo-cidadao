using GeoCidadao.GerenciamentoUsuariosAPI.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilePictureController(IProfilePictureService service) : ControllerBase
    {
        private readonly IProfilePictureService _service = service;


        [HttpGet("{userId}/photo")]
        public async Task<IActionResult> GetUserPhoto(Guid userId)
        {
            return Ok();
        }

        [HttpPatch("{userId}/photo")]
        public async Task<IActionResult> UpdateUserPhoto(Guid userId, [FromBody] string photoBase64)
        {
            await _service.UpdateUserPhotoAsync(userId, photoBase64);

            return NoContent();
        }
    }
}