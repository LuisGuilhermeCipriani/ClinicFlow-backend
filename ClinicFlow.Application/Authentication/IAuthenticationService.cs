namespace ClinicFlow.Application.Authentication;

public interface IAuthenticationService
{
    Task<AuthenticationResponseDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
