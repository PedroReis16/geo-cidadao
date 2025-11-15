using GeoCidadao.GerenciamentoUsuariosAPI.Contracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Controllers
{
    /// <summary>
    /// Controller for managing user interests (regions, cities, and post categories)
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UserInterestsController : ControllerBase
    {
        private readonly IUserInterestsService _service;

        public UserInterestsController(IUserInterestsService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get user interests by user ID
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>User interests or NotFound if not configured</returns>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(UserInterestsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserInterests(Guid userId)
        {
            UserInterestsDTO? interests = await _service.GetUserInterestsAsync(userId);

            if (interests == null)
                return NotFound();

            return Ok(interests);
        }

        /// <summary>
        /// Create user interests
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="interestsDTO">The interests data</param>
        /// <returns>Created user interests</returns>
        [HttpPost("{userId}")]
        [ProducesResponseType(typeof(UserInterestsDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUserInterests(Guid userId, [FromBody] UpdateUserInterestsDTO interestsDTO)
        {
            UserInterestsDTO interests = await _service.CreateUserInterestsAsync(userId, interestsDTO);
            return CreatedAtAction(nameof(GetUserInterests), new { userId }, interests);
        }

        /// <summary>
        /// Update user interests
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="interestsDTO">The updated interests data</param>
        /// <returns>Updated user interests</returns>
        [HttpPut("{userId}")]
        [ProducesResponseType(typeof(UserInterestsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUserInterests(Guid userId, [FromBody] UpdateUserInterestsDTO interestsDTO)
        {
            UserInterestsDTO interests = await _service.UpdateUserInterestsAsync(userId, interestsDTO);
            return Ok(interests);
        }

        /// <summary>
        /// Delete user interests
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>NoContent on success</returns>
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUserInterests(Guid userId)
        {
            await _service.DeleteUserInterestsAsync(userId);
            return NoContent();
        }
    }
}
