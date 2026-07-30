using ClinicFlow.Domain.Patients;

namespace ClinicFlow.Application.Patients;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<PagedResult<Patient>> SearchAsync(
        string? searchTerm,
        PatientStatus? status,
        PatientGender? gender,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCpfAsync(string cpf, long? excludeId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Patient patient, CancellationToken cancellationToken = default);

    void Update(Patient patient);

    void Remove(Patient patient);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
