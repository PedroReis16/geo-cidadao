using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult HealthCheck()
    {
        return Ok(new { Message = "GeoCidadao.GerenciamentoUsuariosAPI" });
    }
}
