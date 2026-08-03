using ClinicFlow.Domain.Appointments;

namespace ClinicFlow.Application.Appointments;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Appointment>> GetByPatientIdAsync(long patientId, CancellationToken cancellationToken = default);

    Task<PagedResult<Appointment>> SearchAsync(
        long? doctorId,
        long? patientId,
        DateTime? appointmentDate,
        AppointmentStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> HasDoctorConflictAsync(long doctorId, DateTime appointmentDate, int startMinute, int endMinute, long? excludeId = null, CancellationToken cancellationToken = default);

    Task<bool> HasPatientConflictAsync(long patientId, DateTime appointmentDate, int startMinute, int endMinute, long? excludeId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default);

    void Update(Appointment appointment);

    void Remove(Appointment appointment);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
