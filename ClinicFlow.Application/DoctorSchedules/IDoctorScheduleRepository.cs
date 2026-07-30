using ClinicFlow.Domain.DoctorSchedules;

namespace ClinicFlow.Application.DoctorSchedules;

public interface IDoctorScheduleRepository
{
    Task<DoctorSchedule?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<PagedResult<DoctorSchedule>> SearchAsync(
        long? doctorId,
        DayOfWeek? dayOfWeek,
        DoctorScheduleStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        long doctorId,
        DayOfWeek dayOfWeek,
        int startMinute,
        int endMinute,
        long? excludeId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(DoctorSchedule schedule, CancellationToken cancellationToken = default);

    void Update(DoctorSchedule schedule);

    void Remove(DoctorSchedule schedule);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
