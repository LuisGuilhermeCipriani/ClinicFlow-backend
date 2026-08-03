namespace ClinicFlow.Application.Authentication;

public sealed record AuthenticationResponseDto(
    string AccessToken,
    string TokenType,
    DateTimeOffset ExpiresAtUtc,
    string Username,
    string DisplayName,
    string Role);
