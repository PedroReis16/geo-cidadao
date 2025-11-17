using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.SearchServiceAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult HealthCheck()
    {
        return Ok(new { Message = "GeoCidadao.SearchServiceAPI" });
    }
}
