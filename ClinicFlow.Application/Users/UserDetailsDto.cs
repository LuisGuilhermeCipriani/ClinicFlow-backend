using ClinicFlow.Domain.Users;

namespace ClinicFlow.Application.Users;

public sealed record UserDetailsDto(
    long Id,
    string Username,
    string DisplayName,
    string Email,
    string Role,
    UserStatus Status,
    DateTimeOffset CreatedAt,
    string? CreatedBy,
    DateTimeOffset? UpdatedAt,
    string? UpdatedBy,
    DateTimeOffset? DeletedAt,
    string? DeletedBy);
