using ClinicFlow.Domain.Doctors;

namespace ClinicFlow.Application.Doctors;

public sealed record DoctorSearchRequest(
    string? SearchTerm = null,
    DoctorStatus? Status = null,
    long? SpecialtyId = null,
    int Page = 1,
    int PageSize = 10);
