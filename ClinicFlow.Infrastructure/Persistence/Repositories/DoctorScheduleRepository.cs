using ClinicFlow.Application.DoctorSchedules;
using ClinicFlow.Domain.DoctorSchedules;
using ClinicFlow.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Persistence.Repositories;

public sealed class DoctorScheduleRepository(ClinicFlowDbContext context) : IDoctorScheduleRepository
{
    public async Task<DoctorSchedule?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await context.DoctorSchedules
            .Include(schedule => schedule.Doctor)
            .FirstOrDefaultAsync(schedule => schedule.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<PagedResult<DoctorSchedule>> SearchAsync(
        long? doctorId,
        DayOfWeek? dayOfWeek,
        DoctorScheduleStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.DoctorSchedules
            .Include(schedule => schedule.Doctor)
            .AsNoTracking()
            .AsQueryable();

        if (doctorId is not null)
        {
            query = query.Where(schedule => schedule.DoctorId == doctorId);
        }

        if (dayOfWeek is not null)
        {
            query = query.Where(schedule => schedule.DayOfWeek == dayOfWeek);
        }

        if (status is not null)
        {
            query = query.Where(schedule => schedule.Status == status);
        }

        var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query
            .OrderBy(schedule => schedule.DoctorId)
            .ThenBy(schedule => schedule.DayOfWeek)
            .ThenBy(schedule => schedule.StartMinute)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PagedResult<DoctorSchedule>(items, page, pageSize, totalCount);
    }

    public async Task<bool> ExistsAsync(
        long doctorId,
        DayOfWeek dayOfWeek,
        int startMinute,
        int endMinute,
        long? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.DoctorSchedules.Where(schedule =>
            schedule.DoctorId == doctorId &&
            schedule.DayOfWeek == dayOfWeek &&
            schedule.StartMinute == startMinute &&
            schedule.EndMinute == endMinute &&
            EF.Property<int>(schedule, nameof(AuditableEntity.IsDeleted)) == 0);

        if (excludeId.HasValue)
        {
            var value = excludeId.Value;
            query = query.Where(schedule => schedule.Id != value);
        }

        return await query.CountAsync(cancellationToken).ConfigureAwait(false) > 0;
    }

    public async Task AddAsync(DoctorSchedule schedule, CancellationToken cancellationToken = default)
    {
        await context.DoctorSchedules.AddAsync(schedule, cancellationToken).ConfigureAwait(false);
    }

    public void Update(DoctorSchedule schedule)
    {
        context.DoctorSchedules.Update(schedule);
    }

    public void Remove(DoctorSchedule schedule)
    {
        context.DoctorSchedules.Update(schedule);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
