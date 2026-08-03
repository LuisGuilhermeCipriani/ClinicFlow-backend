namespace ClinicFlow.Application.Users;

public sealed record UpdateUserRequest(
    string Username,
    string DisplayName,
    string Email,
    string Role,
    string? Password);
