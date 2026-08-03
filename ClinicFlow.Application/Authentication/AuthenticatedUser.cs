namespace ClinicFlow.Application.Authentication;

public sealed record AuthenticatedUser(
    string Username,
    string DisplayName,
    string Role);
