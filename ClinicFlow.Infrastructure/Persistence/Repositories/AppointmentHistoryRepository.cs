using ClinicFlow.Application.Appointments;
using ClinicFlow.Domain.Appointments;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Persistence.Repositories;

public sealed class AppointmentHistoryRepository(ClinicFlowDbContext context) : IAppointmentHistoryRepository
{
    public async Task<IReadOnlyCollection<AppointmentHistory>> GetByAppointmentIdAsync(long appointmentId, CancellationToken cancellationToken = default)
    {
        return await context.AppointmentHistories
            .AsNoTracking()
            .Where(history => history.AppointmentId == appointmentId)
            .OrderBy(history => history.CreatedAt)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task AddAsync(AppointmentHistory history, CancellationToken cancellationToken = default)
    {
        await context.AppointmentHistories.AddAsync(history, cancellationToken).ConfigureAwait(false);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
