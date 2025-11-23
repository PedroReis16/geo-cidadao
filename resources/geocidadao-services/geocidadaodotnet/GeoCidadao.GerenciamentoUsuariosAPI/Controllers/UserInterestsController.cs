using GeoCidadao.GerenciamentoUsuariosAPI.Contracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Models.DTOs;
using GeoCidadao.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Controllers
{
    /// <summary>
    /// Controller for managing user interests (regions, cities, and post categories)
    /// </summary>
    [ApiController]
    [Route("[controller]/{userId}")]
    public class UserInterestsController(IUserInterestsService service) : ControllerBase
    {
        private readonly IUserInterestsService _service = service;


        /// <summary>
        /// Buscar preferências de postagem do usuário pelo Id
        /// </summary>
        /// <param name="userId">O Id do usuário</param>
        /// <returns>Preferências de postagem do usuário ou NoContent se não configurado</returns>
        [HttpGet]
        public async Task<IActionResult> GetUserInterests(Guid userId)
        {
            UserInterestsDTO? interests = await _service.GetUserInterestsAsync(userId);

            if (interests == null)
                return NoContent();

            return Ok(interests);
        }

        /// <summary>
        /// Atualizar as categorias seguidas pelo usuário
        /// </summary>
        /// <param name="userId">O Id do usuário</param>
        /// <param name="categories">Lista de categorias a serem seguidas ou deixadas de seguir</param>
        [HttpPatch("categories")]
        public async Task<IActionResult> UpdateFollowedCategories(Guid userId, [FromBody] List<PostCategory> categories)
        {
            await _service.UpdateUserFollowedCategoriesAsync(userId, categories);
            return NoContent();
        }

        /// <summary>
        /// Atualizar os perfis de interesse do usuário
        /// </summary>
        /// <param name="userId">O Id do usuário</param>
        /// <param name="followedUserId">Id do perfil a ser seguido ou deixado de seguir</param>
        [HttpPatch("users")]
        public async Task<IActionResult> UpdateFollowedUser(Guid userId, [FromBody] Guid followedUserId)
        {
            await _service.UpdateUserFollowedUsersAsync(userId, followedUserId);
            return NoContent();
        }

        /// <summary>
        /// Atualizar as cidades seguidas pelo usuário
        /// </summary>
        /// <param name="userId">O Id do usuário</param>
        /// <param name="city">Nome da cidade a ser seguida ou deixada de seguir</param>
        [HttpPatch("cities")]
        public async Task<IActionResult> UpdateFollowedCity(Guid userId, [FromBody] string city)
        {
            await _service.UpdateUserFollowedCitiesAsync(userId, city);
            return NoContent();
        }

        /// <summary>
        /// Atualizar os bairros seguidos pelo usuário
        /// </summary>
        /// <param name="userId">O Id do usuário</param>
        /// <param name="district">Nome do bairro a ser seguido ou deixado de seguir</param>
        /// <returns></returns>
        [HttpPatch("districts")]
        public async Task<IActionResult> UpdateFollowedDistrict(Guid userId, [FromBody] string district)
        {
            await _service.UpdateUserFollowedDistrictsAsync(userId, district);
            return NoContent();
        }

    }
}
