using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/status")]
[AllowAnonymous]
public class StatusController : ControllerBase
{
    private readonly IHostEnvironment environment;

    public StatusController(IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);

        this.environment = environment;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiStatusResponse), StatusCodes.Status200OK)]
    public ActionResult<ApiStatusResponse> Get()
    {
        var response = new ApiStatusResponse(
            ApplicationName: "ClinicFlow API",
            Environment: environment.EnvironmentName,
            TimestampUtc: DateTimeOffset.UtcNow);

        return Ok(response);
    }
}

public sealed record ApiStatusResponse(string ApplicationName, string Environment, DateTimeOffset TimestampUtc);
