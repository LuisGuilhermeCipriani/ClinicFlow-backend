using ClinicFlow.Domain.Exceptions;
using ClinicFlow.Domain.Primitives;

namespace ClinicFlow.Domain.Users;

public sealed class User : AuditableEntity
{
    public const int MaxUsernameLength = 50;
    public const int MaxDisplayNameLength = 150;
    public const int MaxEmailLength = 254;
    public const int MaxRoleLength = 50;
    public const int MaxPasswordHashLength = 256;

    public string Username { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public string Role { get; private set; } = string.Empty;

    public UserStatus Status { get; private set; } = UserStatus.Active;

    private User()
    {
    }

    public static User Create(
        string username,
        string displayName,
        string email,
        string passwordHash,
        string role,
        DateTimeOffset createdAt,
        string? createdBy)
    {
        Validate(username, displayName, email, passwordHash, role);

        var user = new User
        {
            Username = Normalize(username),
            DisplayName = Normalize(displayName),
            Email = Normalize(email),
            PasswordHash = passwordHash.Trim(),
            Role = Normalize(role),
            Status = UserStatus.Active
        };

        user.MarkCreated(createdAt, createdBy);
        return user;
    }

    public void Update(
        string username,
        string displayName,
        string email,
        string role,
        DateTimeOffset updatedAt,
        string? updatedBy)
    {
        Validate(username, displayName, email, PasswordHash, role);

        Username = Normalize(username);
        DisplayName = Normalize(displayName);
        Email = Normalize(email);
        Role = Normalize(role);
        MarkUpdated(updatedAt, updatedBy);
    }

    public void ChangePassword(string passwordHash, DateTimeOffset updatedAt, string? updatedBy)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainValidationException(["A senha do usuário é obrigatória."]);
        }

        var normalizedPasswordHash = passwordHash.Trim();
        if (normalizedPasswordHash.Length > MaxPasswordHashLength)
        {
            throw new DomainValidationException([$"O hash de senha deve ter no máximo {MaxPasswordHashLength} caracteres."]);
        }

        PasswordHash = normalizedPasswordHash;
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Activate(DateTimeOffset updatedAt, string? updatedBy)
    {
        Status = UserStatus.Active;
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Deactivate(DateTimeOffset updatedAt, string? updatedBy)
    {
        Status = UserStatus.Inactive;
        MarkUpdated(updatedAt, updatedBy);
    }

    private static void Validate(
        string username,
        string displayName,
        string email,
        string passwordHash,
        string role)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(username))
        {
            errors.Add("O nome de usuário é obrigatório.");
        }
        else if (username.Trim().Length > MaxUsernameLength)
        {
            errors.Add($"O nome de usuário deve ter no máximo {MaxUsernameLength} caracteres.");
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            errors.Add("O nome do usuário é obrigatório.");
        }
        else if (displayName.Trim().Length > MaxDisplayNameLength)
        {
            errors.Add($"O nome do usuário deve ter no máximo {MaxDisplayNameLength} caracteres.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add("O e-mail do usuário é obrigatório.");
        }
        else if (email.Trim().Length > MaxEmailLength)
        {
            errors.Add($"O e-mail do usuário deve ter no máximo {MaxEmailLength} caracteres.");
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            errors.Add("A senha do usuário é obrigatória.");
        }
        else if (passwordHash.Trim().Length > MaxPasswordHashLength)
        {
            errors.Add($"O hash de senha deve ter no máximo {MaxPasswordHashLength} caracteres.");
        }

        if (string.IsNullOrWhiteSpace(role))
        {
            errors.Add("O perfil do usuário é obrigatório.");
        }
        else if (role.Trim().Length > MaxRoleLength)
        {
            errors.Add($"O perfil do usuário deve ter no máximo {MaxRoleLength} caracteres.");
        }

        if (errors.Count > 0)
        {
            throw new DomainValidationException(errors);
        }
    }

    private static string Normalize(string value)
    {
        return value.Trim();
    }
}
