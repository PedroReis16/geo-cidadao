using Microsoft.AspNetCore.Mvc;

namespace <PROJECT_NAME>.Controllers;

[Route("[controller]")]
[ApiController]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult HealthCheck()
    {
        return Ok(new { Message = "<PROJECT_NAME>" });
    }
}
