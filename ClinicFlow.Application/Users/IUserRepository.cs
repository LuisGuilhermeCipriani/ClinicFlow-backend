using ClinicFlow.Domain.Users;

namespace ClinicFlow.Application.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<PagedResult<User>> SearchAsync(string? searchTerm, UserStatus? status, string? role, int page, int pageSize, CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);

    void Update(User user);

    void Remove(User user);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
