namespace ClinicFlow.Application.DoctorSchedules;

public interface IDoctorScheduleService
{
    Task<DoctorScheduleDetailsDto> CreateAsync(CreateDoctorScheduleRequest request, CancellationToken cancellationToken = default);

    Task<DoctorScheduleDetailsDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<PagedResult<DoctorScheduleDto>> SearchAsync(DoctorScheduleSearchRequest request, CancellationToken cancellationToken = default);

    Task<DoctorScheduleDetailsDto?> UpdateAsync(long id, UpdateDoctorScheduleRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);

    Task<DoctorScheduleDetailsDto?> SetStatusAsync(long id, bool isActive, CancellationToken cancellationToken = default);
}
