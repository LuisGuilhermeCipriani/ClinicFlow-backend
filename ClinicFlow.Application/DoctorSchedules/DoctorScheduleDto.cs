using ClinicFlow.Domain.DoctorSchedules;

namespace ClinicFlow.Application.DoctorSchedules;

public sealed record DoctorScheduleDto(
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
    DateTimeOffset? UpdatedAt);
