using GeoCidadao.GerenciamentoUsuariosAPI.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController(IProfileService service) : ControllerBase
    {
        private readonly IProfileService _service = service;

        

    }
}