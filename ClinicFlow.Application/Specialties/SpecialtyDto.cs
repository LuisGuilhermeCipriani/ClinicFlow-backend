using ClinicFlow.Domain.Specialties;

namespace ClinicFlow.Application.Specialties;

public sealed record SpecialtyDto(
    long Id,
    string Name,
    string? Description,
    SpecialtyStatus Status,
    bool IsDeleted,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
