namespace ClinicFlow.Application.Specialties;

public interface ISpecialtyService
{
    Task<SpecialtyDetailsDto> CreateAsync(CreateSpecialtyRequest request, CancellationToken cancellationToken = default);

    Task<SpecialtyDetailsDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<PagedResult<SpecialtyDto>> SearchAsync(SpecialtySearchRequest request, CancellationToken cancellationToken = default);

    Task<SpecialtyDetailsDto?> UpdateAsync(long id, UpdateSpecialtyRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);

    Task<SpecialtyDetailsDto?> SetStatusAsync(long id, bool isActive, CancellationToken cancellationToken = default);
}
