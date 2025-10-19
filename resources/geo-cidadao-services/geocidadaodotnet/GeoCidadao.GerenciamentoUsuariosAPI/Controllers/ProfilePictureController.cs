using GeoCidadao.GerenciamentoUsuariosAPI.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilePictureController(IUserPictureService service) : ControllerBase
    {
        private readonly IUserPictureService _service = service;


        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserPhoto(Guid userId)
        {
            return Ok();
        }

        [HttpPatch("{userId}")]
        public async Task<IActionResult> UpdateUserPhoto(Guid userId, [FromForm] IFormFile photo)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return new ContentResult { StatusCode = 406 };

            await _service.UpdateUserPhotoAsync(userId, photo);

            return NoContent();
        }
    }
}