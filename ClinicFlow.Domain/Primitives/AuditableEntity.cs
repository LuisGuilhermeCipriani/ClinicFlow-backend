namespace ClinicFlow.Domain.Primitives;

public abstract class AuditableEntity : Entity
{
    public DateTimeOffset CreatedAt { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public string? UpdatedBy { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTimeOffset? DeletedAt { get; private set; }

    public string? DeletedBy { get; private set; }

    public void MarkCreated(DateTimeOffset createdAt, string? createdBy)
    {
        ValidateTimestamp(createdAt);

        CreatedAt = createdAt;
        CreatedBy = NormalizeUserName(createdBy);
    }

    public void MarkUpdated(DateTimeOffset updatedAt, string? updatedBy)
    {
        ValidateTimestamp(updatedAt);

        UpdatedAt = updatedAt;
        UpdatedBy = NormalizeUserName(updatedBy);
    }

    public void MarkDeleted(DateTimeOffset deletedAt, string? deletedBy)
    {
        ValidateTimestamp(deletedAt);

        IsDeleted = true;
        DeletedAt = deletedAt;
        DeletedBy = NormalizeUserName(deletedBy);
    }

    private static void ValidateTimestamp(DateTimeOffset timestamp)
    {
        if (timestamp == default)
        {
            throw new ArgumentException("O carimbo de data e hora precisa ser informado.", nameof(timestamp));
        }
    }

    private static string? NormalizeUserName(string? userName)
    {
        return string.IsNullOrWhiteSpace(userName) ? null : userName.Trim();
    }
}
