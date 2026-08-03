using ClinicFlow.Application.Authentication;
using ClinicFlow.Domain.Users;

namespace ClinicFlow.Application.Users;

public sealed class UserService(
    IUserRepository repository,
    IUserPasswordHasher passwordHasher) : IUserService
{
    public async Task<UserDetailsDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await EnsureUsernameIsAvailableAsync(request.Username, cancellationToken).ConfigureAwait(false);
        await EnsureEmailIsAvailableAsync(request.Email, cancellationToken).ConfigureAwait(false);
        EnsureRoleIsValid(request.Role);

        var user = User.Create(
            request.Username,
            request.DisplayName,
            request.Email,
            passwordHasher.HashPassword(request.Password),
            request.Role,
            DateTimeOffset.UtcNow,
            "system");

        await repository.AddAsync(user, cancellationToken).ConfigureAwait(false);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return MapToDetailsDto(user);
    }

    public async Task<UserDetailsDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return user is null ? null : MapToDetailsDto(user);
    }

    public async Task<PagedResult<UserDto>> SearchAsync(UserSearchRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var result = await repository.SearchAsync(request.SearchTerm, request.Status, request.Role, page, pageSize, cancellationToken).ConfigureAwait(false);

        return new PagedResult<UserDto>(
            result.Items.Select(MapToDto).ToArray(),
            result.Page,
            result.PageSize,
            result.TotalCount);
    }

    public async Task<UserDetailsDto?> UpdateAsync(long id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return null;
        }

        var existing = await repository.GetByUsernameAsync(request.Username, cancellationToken).ConfigureAwait(false);
        if (existing is not null && existing.Id != id)
        {
            throw new InvalidOperationException("Já existe um usuário com este nome de usuário.");
        }

        existing = await repository.GetByEmailAsync(request.Email, cancellationToken).ConfigureAwait(false);
        if (existing is not null && existing.Id != id)
        {
            throw new InvalidOperationException("Já existe um usuário com este e-mail.");
        }

        EnsureRoleIsValid(request.Role);

        user.Update(request.Username, request.DisplayName, request.Email, request.Role, DateTimeOffset.UtcNow, "system");

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.ChangePassword(passwordHasher.HashPassword(request.Password), DateTimeOffset.UtcNow, "system");
        }

        repository.Update(user);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return MapToDetailsDto(user);
    }

    public async Task<UserDetailsDto?> SetStatusAsync(long id, bool isActive, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return null;
        }

        if (isActive)
        {
            user.Activate(DateTimeOffset.UtcNow, "system");
        }
        else
        {
            user.Deactivate(DateTimeOffset.UtcNow, "system");
        }

        repository.Update(user);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return MapToDetailsDto(user);
    }

    private async Task EnsureUsernameIsAvailableAsync(string username, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByUsernameAsync(username, cancellationToken).ConfigureAwait(false);
        if (existing is not null)
        {
            throw new InvalidOperationException("Já existe um usuário com este nome de usuário.");
        }
    }

    private async Task EnsureEmailIsAvailableAsync(string email, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByEmailAsync(email, cancellationToken).ConfigureAwait(false);
        if (existing is not null)
        {
            throw new InvalidOperationException("Já existe um usuário com este e-mail.");
        }
    }

    private static void EnsureRoleIsValid(string role)
    {
        if (!string.Equals(role, ClinicFlowRoles.Admin, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(role, ClinicFlowRoles.Receptionist, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(role, ClinicFlowRoles.Doctor, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("O perfil do usuário é inválido.");
        }
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto(user.Id, user.Username, user.DisplayName, user.Email, user.Role, user.Status, user.CreatedAt, user.UpdatedAt);
    }

    private static UserDetailsDto MapToDetailsDto(User user)
    {
        return new UserDetailsDto(
            user.Id,
            user.Username,
            user.DisplayName,
            user.Email,
            user.Role,
            user.Status,
            user.CreatedAt,
            user.CreatedBy,
            user.UpdatedAt,
            user.UpdatedBy,
            user.DeletedAt,
            user.DeletedBy);
    }
}
