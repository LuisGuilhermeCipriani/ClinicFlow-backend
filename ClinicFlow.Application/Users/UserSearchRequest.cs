using ClinicFlow.Domain.Users;

namespace ClinicFlow.Application.Users;

public sealed record UserSearchRequest(string? SearchTerm, UserStatus? Status, string? Role, int Page = 1, int PageSize = 10);
