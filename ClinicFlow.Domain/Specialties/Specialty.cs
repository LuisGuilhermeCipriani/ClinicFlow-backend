using ClinicFlow.Domain.Exceptions;
using ClinicFlow.Domain.Primitives;

namespace ClinicFlow.Domain.Specialties;

public sealed class Specialty : AuditableEntity
{
    public const int MaxNameLength = 150;
    public const int MaxDescriptionLength = 500;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public SpecialtyStatus Status { get; private set; } = SpecialtyStatus.Active;

    private Specialty()
    {
    }

    public static Specialty Create(string name, string? description, DateTimeOffset createdAt, string? createdBy)
    {
        Validate(name, description);

        var specialty = new Specialty
        {
            Name = NormalizeName(name),
            Description = NormalizeDescription(description),
            Status = SpecialtyStatus.Active
        };

        specialty.MarkCreated(createdAt, createdBy);
        return specialty;
    }

    public void Update(string name, string? description, DateTimeOffset updatedAt, string? updatedBy)
    {
        Validate(name, description);

        Name = NormalizeName(name);
        Description = NormalizeDescription(description);
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Activate(DateTimeOffset updatedAt, string? updatedBy)
    {
        Status = SpecialtyStatus.Active;
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Deactivate(DateTimeOffset updatedAt, string? updatedBy)
    {
        Status = SpecialtyStatus.Inactive;
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Delete(DateTimeOffset deletedAt, string? deletedBy)
    {
        Status = SpecialtyStatus.Inactive;
        MarkDeleted(deletedAt, deletedBy);
    }

    private static void Validate(string name, string? description)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add("O nome da especialidade é obrigatório.");
        }
        else if (name.Trim().Length is < 3 or > MaxNameLength)
        {
            errors.Add($"O nome da especialidade deve ter entre 3 e {MaxNameLength} caracteres.");
        }

        if (description is not null && description.Trim().Length > MaxDescriptionLength)
        {
            errors.Add($"A descrição da especialidade deve ter no máximo {MaxDescriptionLength} caracteres.");
        }

        if (errors.Count > 0)
        {
            throw new DomainValidationException(errors);
        }
    }

    private static string NormalizeName(string name)
    {
        return name.Trim();
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}
