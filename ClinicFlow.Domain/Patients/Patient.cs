using ClinicFlow.Domain.Exceptions;
using ClinicFlow.Domain.Primitives;

namespace ClinicFlow.Domain.Patients;

public sealed class Patient : AuditableEntity
{
    public const int MaxNameLength = 150;
    public const int MaxCpfLength = 11;
    public const int MaxEmailLength = 254;
    public const int MaxPhoneLength = 20;

    public string Name { get; private set; } = string.Empty;

    public string Cpf { get; private set; } = string.Empty;

    public DateTime BirthDate { get; private set; }

    public PatientGender Gender { get; private set; }

    public string Email { get; private set; } = string.Empty;

    public string Phone { get; private set; } = string.Empty;

    public PatientStatus Status { get; private set; } = PatientStatus.Active;

    private Patient()
    {
    }

    public static Patient Create(
        string name,
        string cpf,
        DateTime birthDate,
        PatientGender gender,
        string email,
        string phone,
        DateTimeOffset createdAt,
        string? createdBy)
    {
        Validate(name, cpf, birthDate, gender, email, phone);

        var patient = new Patient
        {
            Name = NormalizeText(name),
            Cpf = NormalizeCpf(cpf),
            BirthDate = birthDate.Date,
            Gender = gender,
            Email = NormalizeText(email),
            Phone = NormalizeText(phone),
            Status = PatientStatus.Active
        };

        patient.MarkCreated(createdAt, createdBy);
        return patient;
    }

    public void Update(
        string name,
        string cpf,
        DateTime birthDate,
        PatientGender gender,
        string email,
        string phone,
        DateTimeOffset updatedAt,
        string? updatedBy)
    {
        Validate(name, cpf, birthDate, gender, email, phone);

        Name = NormalizeText(name);
        Cpf = NormalizeCpf(cpf);
        BirthDate = birthDate.Date;
        Gender = gender;
        Email = NormalizeText(email);
        Phone = NormalizeText(phone);
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Activate(DateTimeOffset updatedAt, string? updatedBy)
    {
        Status = PatientStatus.Active;
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Deactivate(DateTimeOffset updatedAt, string? updatedBy)
    {
        Status = PatientStatus.Inactive;
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Delete(DateTimeOffset deletedAt, string? deletedBy)
    {
        Status = PatientStatus.Inactive;
        MarkDeleted(deletedAt, deletedBy);
    }

    private static void Validate(
        string name,
        string cpf,
        DateTime birthDate,
        PatientGender gender,
        string email,
        string phone)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add("O nome do paciente é obrigatório.");
        }
        else if (name.Trim().Length is < 3 or > MaxNameLength)
        {
            errors.Add($"O nome do paciente deve ter entre 3 e {MaxNameLength} caracteres.");
        }

        var normalizedCpf = NormalizeCpf(cpf);
        if (string.IsNullOrWhiteSpace(cpf))
        {
            errors.Add("O CPF do paciente é obrigatório.");
        }
        else if (normalizedCpf.Length != MaxCpfLength)
        {
            errors.Add("O CPF do paciente deve conter exatamente 11 dígitos.");
        }

        var today = DateTime.UtcNow.Date;
        if (birthDate == default)
        {
            errors.Add("A data de nascimento do paciente é obrigatória.");
        }
        else if (birthDate.Date > today)
        {
            errors.Add("A data de nascimento do paciente não pode ser futura.");
        }
        else if (birthDate.Date < today.AddYears(-120))
        {
            errors.Add("A data de nascimento do paciente não parece válida.");
        }

        if (!Enum.IsDefined(typeof(PatientGender), gender))
        {
            errors.Add("O sexo do paciente é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add("O e-mail do paciente é obrigatório.");
        }
        else if (email.Trim().Length > MaxEmailLength || !email.Contains('@'))
        {
            errors.Add("O e-mail do paciente é inválido.");
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            errors.Add("O telefone do paciente é obrigatório.");
        }
        else if (phone.Trim().Length > MaxPhoneLength)
        {
            errors.Add($"O telefone do paciente deve ter no máximo {MaxPhoneLength} caracteres.");
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

    private static string NormalizeCpf(string value)
    {
        return new string(value.Where(char.IsDigit).ToArray());
    }
}
