using ClinicFlow.Domain.Patients;

namespace ClinicFlow.Application.Patients;

public sealed record CreatePatientRequest(
    string Name,
    string Cpf,
    DateTime BirthDate,
    PatientGender Gender,
    string Email,
    string Phone);
