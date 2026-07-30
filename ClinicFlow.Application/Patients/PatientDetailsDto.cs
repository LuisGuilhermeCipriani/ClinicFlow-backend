using ClinicFlow.Domain.Patients;

namespace ClinicFlow.Application.Patients;

public sealed record PatientDetailsDto(
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
    string? CreatedBy,
    DateTimeOffset? UpdatedAt,
    string? UpdatedBy,
    DateTimeOffset? DeletedAt,
    string? DeletedBy);
