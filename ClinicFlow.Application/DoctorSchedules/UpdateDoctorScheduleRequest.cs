using ClinicFlow.Domain.DoctorSchedules;

namespace ClinicFlow.Application.DoctorSchedules;

public sealed record UpdateDoctorScheduleRequest(
    long DoctorId,
    DayOfWeek DayOfWeek,
    string StartTime,
    string EndTime,
    int SlotDurationMinutes,
    DoctorScheduleStatus Status);
