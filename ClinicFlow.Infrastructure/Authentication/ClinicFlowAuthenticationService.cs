using ClinicFlow.Application.Authentication;
using ClinicFlow.Application.Users;
using Microsoft.Extensions.Options;

namespace ClinicFlow.Infrastructure.Authentication;

public sealed class ClinicFlowAuthenticationService(
    IOptions<ClinicFlowAuthenticationOptions> options,
    IUserRepository userRepository,
    IUserPasswordHasher passwordHasher,
    IAuthenticationTokenService tokenService) : IAuthenticationService
{
    private readonly ClinicFlowAuthenticationOptions authenticationOptions = options.Value;

    public async Task<AuthenticationResponseDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var normalizedUsername = request.Username.Trim();
        var configuredUser = authenticationOptions.Users.FirstOrDefault(item =>
            string.Equals(item.Username.Trim(), normalizedUsername, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(item.Password, request.Password, StringComparison.Ordinal));

        if (configuredUser is not null)
        {
            var configuredIssuedAt = DateTimeOffset.UtcNow;
            var configuredExpiresAt = configuredIssuedAt.AddMinutes(authenticationOptions.TokenLifetimeMinutes <= 0 ? 480 : authenticationOptions.TokenLifetimeMinutes);
            var configuredAuthenticatedUser = new AuthenticatedUser(
                configuredUser.Username.Trim(),
                string.IsNullOrWhiteSpace(configuredUser.DisplayName) ? configuredUser.Username.Trim() : configuredUser.DisplayName.Trim(),
                string.IsNullOrWhiteSpace(configuredUser.Role) ? "User" : configuredUser.Role.Trim());

            var configuredAccessToken = tokenService.CreateToken(configuredAuthenticatedUser, configuredIssuedAt, configuredExpiresAt);

            return new AuthenticationResponseDto(
                configuredAccessToken,
                "Bearer",
                configuredExpiresAt,
                configuredAuthenticatedUser.Username,
                configuredAuthenticatedUser.DisplayName,
                configuredAuthenticatedUser.Role);
        }

        var user = await userRepository.GetByUsernameAsync(normalizedUsername, cancellationToken).ConfigureAwait(false);
        if (user is not null)
        {
            if (user.Status != Domain.Users.UserStatus.Active || !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                return null;
            }

            var issuedAt = DateTimeOffset.UtcNow;
            var expiresAt = issuedAt.AddMinutes(authenticationOptions.TokenLifetimeMinutes <= 0 ? 480 : authenticationOptions.TokenLifetimeMinutes);
            var authenticatedUser = new AuthenticatedUser(user.Username, user.DisplayName, user.Role);

            var accessToken = tokenService.CreateToken(authenticatedUser, issuedAt, expiresAt);

            return new AuthenticationResponseDto(
                accessToken,
                "Bearer",
                expiresAt,
                authenticatedUser.Username,
                authenticatedUser.DisplayName,
                authenticatedUser.Role);
        }

        return null;
    }
}
