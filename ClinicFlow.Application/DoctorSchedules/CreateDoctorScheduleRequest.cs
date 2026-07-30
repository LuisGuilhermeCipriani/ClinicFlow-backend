namespace ClinicFlow.Application.DoctorSchedules;

public sealed record CreateDoctorScheduleRequest(
    long DoctorId,
    DayOfWeek DayOfWeek,
    string StartTime,
    string EndTime,
    int SlotDurationMinutes);
