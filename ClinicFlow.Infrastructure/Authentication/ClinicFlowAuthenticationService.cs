using ClinicFlow.Application.Authentication;
using Microsoft.Extensions.Options;

namespace ClinicFlow.Infrastructure.Authentication;

public sealed class ClinicFlowAuthenticationService(
    IOptions<ClinicFlowAuthenticationOptions> options,
    IAuthenticationTokenService tokenService) : IAuthenticationService
{
    private readonly ClinicFlowAuthenticationOptions authenticationOptions = options.Value;

    public Task<AuthenticationResponseDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Task.FromResult<AuthenticationResponseDto?>(null);
        }

        var user = authenticationOptions.Users.FirstOrDefault(item =>
            string.Equals(item.Username.Trim(), request.Username.Trim(), StringComparison.OrdinalIgnoreCase) &&
            string.Equals(item.Password, request.Password, StringComparison.Ordinal));

        if (user is null)
        {
            return Task.FromResult<AuthenticationResponseDto?>(null);
        }

        var issuedAt = DateTimeOffset.UtcNow;
        var expiresAt = issuedAt.AddMinutes(authenticationOptions.TokenLifetimeMinutes <= 0 ? 480 : authenticationOptions.TokenLifetimeMinutes);
        var authenticatedUser = new AuthenticatedUser(
            user.Username.Trim(),
            string.IsNullOrWhiteSpace(user.DisplayName) ? user.Username.Trim() : user.DisplayName.Trim(),
            string.IsNullOrWhiteSpace(user.Role) ? "User" : user.Role.Trim());

        var accessToken = tokenService.CreateToken(authenticatedUser, issuedAt, expiresAt);

        return Task.FromResult<AuthenticationResponseDto?>(new AuthenticationResponseDto(
            accessToken,
            "Bearer",
            expiresAt,
            authenticatedUser.Username,
            authenticatedUser.DisplayName,
            authenticatedUser.Role));
    }
}
