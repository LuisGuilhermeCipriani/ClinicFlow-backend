using ClinicFlow.Domain.Patients;

namespace ClinicFlow.Application.Patients;

public sealed record PatientSearchRequest(
    string? SearchTerm,
    PatientStatus? Status,
    PatientGender? Gender,
    int Page = 1,
    int PageSize = 10);
