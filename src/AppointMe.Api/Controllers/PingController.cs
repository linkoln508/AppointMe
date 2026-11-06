using Microsoft.AspNetCore.Mvc;

namespace AppointMe.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PingController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public PingController(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Ping()
    {
        var response = new
        {
            status = "ok",
            environment = _environment.EnvironmentName,
            serverTime = DateTime.Now,
            appName = _configuration["App:Name"]
        };

        return Ok(response);
    }
}