using ClinicFlow.Domain.Exceptions;
using ClinicFlow.Domain.Primitives;
using ClinicFlow.Domain.Specialties;

namespace ClinicFlow.Domain.Doctors;

public sealed class Doctor : AuditableEntity
{
    public const int MaxNameLength = 150;
    public const int MaxCrmNumberLength = 20;
    public const int MaxCrmStateLength = 2;
    public const int MaxEmailLength = 254;
    public const int MaxPhoneLength = 20;

    public string Name { get; private set; } = string.Empty;

    public string CrmNumber { get; private set; } = string.Empty;

    public string CrmState { get; private set; } = string.Empty;

    public long SpecialtyId { get; private set; }

    public Specialty? Specialty { get; private set; }

    public string Email { get; private set; } = string.Empty;

    public string Phone { get; private set; } = string.Empty;

    public DoctorStatus Status { get; private set; } = DoctorStatus.Active;

    private Doctor()
    {
    }

    public static Doctor Create(
        string name,
        string crmNumber,
        string crmState,
        long specialtyId,
        string email,
        string phone,
        DateTimeOffset createdAt,
        string? createdBy)
    {
        Validate(name, crmNumber, crmState, specialtyId, email, phone);

        var doctor = new Doctor
        {
            Name = NormalizeText(name),
            CrmNumber = NormalizeCrmNumber(crmNumber),
            CrmState = NormalizeCrmState(crmState),
            SpecialtyId = specialtyId,
            Email = NormalizeEmail(email),
            Phone = NormalizePhone(phone),
            Status = DoctorStatus.Active
        };

        doctor.MarkCreated(createdAt, createdBy);
        return doctor;
    }

    public void Update(
        string name,
        string crmNumber,
        string crmState,
        long specialtyId,
        string email,
        string phone,
        DateTimeOffset updatedAt,
        string? updatedBy)
    {
        Validate(name, crmNumber, crmState, specialtyId, email, phone);

        Name = NormalizeText(name);
        CrmNumber = NormalizeCrmNumber(crmNumber);
        CrmState = NormalizeCrmState(crmState);
        SpecialtyId = specialtyId;
        Email = NormalizeEmail(email);
        Phone = NormalizePhone(phone);
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Activate(DateTimeOffset updatedAt, string? updatedBy)
    {
        Status = DoctorStatus.Active;
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Deactivate(DateTimeOffset updatedAt, string? updatedBy)
    {
        Status = DoctorStatus.Inactive;
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Delete(DateTimeOffset deletedAt, string? deletedBy)
    {
        Status = DoctorStatus.Inactive;
        MarkDeleted(deletedAt, deletedBy);
    }

    private static void Validate(
        string name,
        string crmNumber,
        string crmState,
        long specialtyId,
        string email,
        string phone)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add("O nome do médico é obrigatório.");
        }
        else if (name.Trim().Length is < 3 or > MaxNameLength)
        {
            errors.Add($"O nome do médico deve ter entre 3 e {MaxNameLength} caracteres.");
        }

        if (string.IsNullOrWhiteSpace(crmNumber))
        {
            errors.Add("O número do CRM é obrigatório.");
        }
        else if (crmNumber.Trim().Length is < 4 or > MaxCrmNumberLength)
        {
            errors.Add($"O número do CRM deve ter entre 4 e {MaxCrmNumberLength} caracteres.");
        }

        if (string.IsNullOrWhiteSpace(crmState))
        {
            errors.Add("A sigla do CRM é obrigatória.");
        }
        else if (crmState.Trim().Length != MaxCrmStateLength)
        {
            errors.Add("A sigla do CRM deve conter exatamente 2 letras.");
        }

        if (specialtyId <= 0)
        {
            errors.Add("A especialidade do médico é obrigatória.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add("O e-mail do médico é obrigatório.");
        }
        else if (email.Trim().Length > MaxEmailLength || !email.Contains('@'))
        {
            errors.Add("O e-mail do médico é inválido.");
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            errors.Add("O telefone do médico é obrigatório.");
        }
        else if (phone.Trim().Length > MaxPhoneLength)
        {
            errors.Add($"O telefone do médico deve ter no máximo {MaxPhoneLength} caracteres.");
        }

        if (errors.Count > 0)
        {
            throw new DomainValidationException(errors);
        }
    }

    private static string NormalizeText(string value)
    {
        return value.Trim();
    }

    private static string NormalizeCrmNumber(string value)
    {
        return value.Trim().ToUpperInvariant();
    }

    private static string NormalizeCrmState(string value)
    {
        return value.Trim().ToUpperInvariant();
    }

    private static string NormalizeEmail(string value)
    {
        return value.Trim();
    }

    private static string NormalizePhone(string value)
    {
        return value.Trim();
    }
}
