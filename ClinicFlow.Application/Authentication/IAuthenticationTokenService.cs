using System.Security.Claims;

namespace ClinicFlow.Application.Authentication;

public interface IAuthenticationTokenService
{
    string CreateToken(AuthenticatedUser user, DateTimeOffset issuedAtUtc, DateTimeOffset expiresAtUtc);

    ClaimsPrincipal? ValidateToken(string token);
}
