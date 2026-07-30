using ClinicFlow.Domain.Patients;

namespace ClinicFlow.Application.Patients;

public sealed record PatientDto(
    long Id,
    string Name,
    string Cpf,
    DateTime BirthDate,
    PatientGender Gender,
    string Email,
    string Phone,
    PatientStatus Status,
    bool IsDeleted,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
