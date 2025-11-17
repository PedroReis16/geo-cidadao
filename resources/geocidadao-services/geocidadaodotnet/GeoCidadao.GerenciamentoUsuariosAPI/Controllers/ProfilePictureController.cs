using GeoCidadao.GerenciamentoUsuariosAPI.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfilePictureController(IUserPictureService service) : ControllerBase
    {
        private readonly IUserPictureService _service = service;


        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserPhoto(Guid userId)
        {
            string? photoUrl = await _service.GetUserPhotoUrlAsync(userId);

            if (string.IsNullOrWhiteSpace(photoUrl))
                return NoContent();

            return Ok(photoUrl);
        }

        [HttpPatch("{userId}")]
        public async Task<IActionResult> UpdateUserPhoto(Guid userId, [FromForm] IFormFile photo)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return new ContentResult { StatusCode = 406 };

            await _service.UpdateUserPhotoAsync(userId, photo);

            return CreatedAtAction(nameof(GetUserPhoto), new { userId = userId }, null);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUserPhoto(Guid userId)
        {
            await _service.DeleteUserPhotoAsync(userId);

            return NoContent();
        }
    }
}