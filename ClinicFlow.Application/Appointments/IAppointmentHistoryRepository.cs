using ClinicFlow.Domain.Appointments;

namespace ClinicFlow.Application.Appointments;

public interface IAppointmentHistoryRepository
{
    Task<IReadOnlyCollection<AppointmentHistory>> GetByAppointmentIdAsync(long appointmentId, CancellationToken cancellationToken = default);

    Task AddAsync(AppointmentHistory history, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
