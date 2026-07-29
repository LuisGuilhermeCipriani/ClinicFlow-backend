using ClinicFlow.Domain.Specialties;

namespace ClinicFlow.Application.Specialties;

public interface ISpecialtyRepository
{
    Task<Specialty?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<PagedResult<Specialty>> SearchAsync(
        string? searchTerm,
        SpecialtyStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(string name, long? excludeId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Specialty specialty, CancellationToken cancellationToken = default);

    void Update(Specialty specialty);

    void Remove(Specialty specialty);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
