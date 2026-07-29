using ClinicFlow.Domain.Doctors;

namespace ClinicFlow.Application.Doctors;

public sealed record DoctorDetailsDto(
    long Id,
    string Name,
    string CrmNumber,
    string CrmState,
    long SpecialtyId,
    string SpecialtyName,
    string Email,
    string Phone,
    DoctorStatus Status,
    bool IsDeleted,
    DateTimeOffset CreatedAt,
    string? CreatedBy,
    DateTimeOffset? UpdatedAt,
    string? UpdatedBy,
    DateTimeOffset? DeletedAt,
    string? DeletedBy);
