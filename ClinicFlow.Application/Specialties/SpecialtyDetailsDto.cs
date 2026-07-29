using ClinicFlow.Domain.Specialties;

namespace ClinicFlow.Application.Specialties;

public sealed record SpecialtyDetailsDto(
    long Id,
    string Name,
    string? Description,
    SpecialtyStatus Status,
    bool IsDeleted,
    DateTimeOffset CreatedAt,
    string? CreatedBy,
    DateTimeOffset? UpdatedAt,
    string? UpdatedBy,
    DateTimeOffset? DeletedAt,
    string? DeletedBy);
