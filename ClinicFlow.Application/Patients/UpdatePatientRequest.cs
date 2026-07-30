using ClinicFlow.Domain.Patients;

namespace ClinicFlow.Application.Patients;

public sealed record UpdatePatientRequest(
    string Name,
    string Cpf,
    DateTime BirthDate,
    PatientGender Gender,
    string Email,
    string Phone,
    PatientStatus Status);
