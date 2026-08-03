using ClinicFlow.Application.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController(IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthenticationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthenticationResponseDto>> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await authenticationService.LoginAsync(request, cancellationToken).ConfigureAwait(false);
        if (response is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Credenciais inválidas.",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        return Ok(response);
    }
}
