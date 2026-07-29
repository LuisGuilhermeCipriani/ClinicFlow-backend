namespace ClinicFlow.Application.Doctors;

public interface IDoctorService
{
    Task<DoctorDetailsDto> CreateAsync(CreateDoctorRequest request, CancellationToken cancellationToken = default);

    Task<DoctorDetailsDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<PagedResult<DoctorDto>> SearchAsync(DoctorSearchRequest request, CancellationToken cancellationToken = default);

    Task<DoctorDetailsDto?> UpdateAsync(long id, UpdateDoctorRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);

    Task<DoctorDetailsDto?> SetStatusAsync(long id, bool isActive, CancellationToken cancellationToken = default);
}
