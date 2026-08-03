namespace ClinicFlow.Application.Users;

public interface IUserService
{
    Task<UserDetailsDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    Task<UserDetailsDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<PagedResult<UserDto>> SearchAsync(UserSearchRequest request, CancellationToken cancellationToken = default);

    Task<UserDetailsDto?> UpdateAsync(long id, UpdateUserRequest request, CancellationToken cancellationToken = default);

    Task<UserDetailsDto?> SetStatusAsync(long id, bool isActive, CancellationToken cancellationToken = default);
}
