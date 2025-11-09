using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoPostsAPI.Controllers
{
    [ApiController]
    [Route("posts/locations")]
    public class LocationsController(ILocationsService service) : ControllerBase
    {
        private readonly ILocationsService _service = service;


        [HttpGet()]
        public async Task<IActionResult> GetAreaLocations()
        {
            return Ok();
        }
    }
}