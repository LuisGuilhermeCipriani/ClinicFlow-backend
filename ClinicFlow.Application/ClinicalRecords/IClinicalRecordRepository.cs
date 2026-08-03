using ClinicFlow.Domain.ClinicalRecords;

namespace ClinicFlow.Application.ClinicalRecords;

public interface IClinicalRecordRepository
{
    Task<ClinicalRecord?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<ClinicalRecord?> GetByAppointmentIdAsync(long appointmentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ClinicalRecord>> GetByPatientIdAsync(long patientId, CancellationToken cancellationToken = default);

    Task<PagedResult<ClinicalRecord>> SearchAsync(
        long? appointmentId,
        long? patientId,
        long? doctorId,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(ClinicalRecord record, CancellationToken cancellationToken = default);

    void Update(ClinicalRecord record);

    void Remove(ClinicalRecord record);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
