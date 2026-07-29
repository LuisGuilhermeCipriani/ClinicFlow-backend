using ClinicFlow.Domain.Specialties;

namespace ClinicFlow.Application.Specialties;

public sealed record SpecialtySearchRequest(
    string? SearchTerm = null,
    SpecialtyStatus? Status = null,
    int Page = 1,
    int PageSize = 10);
