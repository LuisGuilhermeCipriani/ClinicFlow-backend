using ClinicFlow.Application.Users;
using ClinicFlow.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(ClinicFlowDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var normalizedUsername = username.Trim();
        return await context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Username == normalizedUsername, cancellationToken).ConfigureAwait(false);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim();
        return await context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken).ConfigureAwait(false);
    }

    public async Task<PagedResult<User>> SearchAsync(string? searchTerm, UserStatus? status, string? role, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = context.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearchTerm = searchTerm.Trim();
            query = query.Where(user =>
                user.Username.Contains(normalizedSearchTerm) ||
                user.DisplayName.Contains(normalizedSearchTerm) ||
                user.Email.Contains(normalizedSearchTerm));
        }

        if (status is not null)
        {
            query = query.Where(user => user.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            var normalizedRole = role.Trim();
            query = query.Where(user => user.Role == normalizedRole);
        }

        var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query
            .OrderBy(user => user.Username)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PagedResult<User>(items, page, pageSize, totalCount);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken).ConfigureAwait(false);
    }

    public void Update(User user)
    {
        context.Users.Update(user);
    }

    public void Remove(User user)
    {
        context.Users.Remove(user);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
