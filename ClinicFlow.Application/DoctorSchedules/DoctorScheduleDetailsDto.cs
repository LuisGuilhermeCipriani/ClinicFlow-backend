using ClinicFlow.Domain.DoctorSchedules;

namespace ClinicFlow.Application.DoctorSchedules;

public sealed record DoctorScheduleDetailsDto(
    long Id,
    long DoctorId,
    string DoctorName,
    DayOfWeek DayOfWeek,
    string StartTime,
    string EndTime,
    int SlotDurationMinutes,
    DoctorScheduleStatus Status,
    bool IsDeleted,
    DateTimeOffset CreatedAt,
    string? CreatedBy,
    DateTimeOffset? UpdatedAt,
    string? UpdatedBy,
    DateTimeOffset? DeletedAt,
    string? DeletedBy);
