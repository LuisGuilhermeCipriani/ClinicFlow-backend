using ClinicFlow.Domain.DoctorSchedules;

namespace ClinicFlow.Application.DoctorSchedules;

public sealed record DoctorScheduleSearchRequest(
    long? DoctorId,
    DayOfWeek? DayOfWeek,
    DoctorScheduleStatus? Status,
    int Page = 1,
    int PageSize = 10);
