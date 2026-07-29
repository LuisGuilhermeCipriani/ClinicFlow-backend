using ClinicFlow.Domain.Doctors;

namespace ClinicFlow.Application.Doctors;

public sealed record DoctorDto(
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
    DateTimeOffset? UpdatedAt);
