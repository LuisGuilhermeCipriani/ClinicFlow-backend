using ClinicFlow.Domain.Doctors;

namespace ClinicFlow.Application.Doctors;

public interface IDoctorRepository
{
    Task<Doctor?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<PagedResult<Doctor>> SearchAsync(
        string? searchTerm,
        DoctorStatus? status,
        long? specialtyId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCrmAsync(string crmNumber, string crmState, long? excludeId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Doctor doctor, CancellationToken cancellationToken = default);

    void Update(Doctor doctor);

    void Remove(Doctor doctor);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
