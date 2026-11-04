using Microsoft.AspNetCore.Mvc;

namespace AppointMe.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PingController : ControllerBase   // наследуемся от базового класса контроллера
{
    private readonly IConfiguration _configuration;       // поле для настроек
    private readonly IWebHostEnvironment _environment;    // поле для окружения

    public PingController(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Ping()     // IActionResult = "результат действия"
    {
        var response = new
        {
            status = "ok",
            environment = _environment.EnvironmentName, //   environment = название окружения.
            serverTime = DateTime.Now,
            appName = _configuration["App:Name"]
        };

        return Ok(response);
    }
}