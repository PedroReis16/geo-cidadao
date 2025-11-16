using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.AnalyticsServiceAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult HealthCheck()
    {
        return Ok(new { Message = "GeoCidadao.AnalyticsServiceAPI" });
    }
}
