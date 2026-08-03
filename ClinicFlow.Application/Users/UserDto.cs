using ClinicFlow.Domain.Users;

namespace ClinicFlow.Application.Users;

public sealed record UserDto(
    long Id,
    string Username,
    string DisplayName,
    string Email,
    string Role,
    UserStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
