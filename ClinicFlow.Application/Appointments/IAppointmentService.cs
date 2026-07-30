namespace ClinicFlow.Application.Appointments;

public interface IAppointmentService
{
    Task<AppointmentDetailsDto> CreateAsync(CreateAppointmentRequest request, CancellationToken cancellationToken = default);

    Task<AppointmentDetailsDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<PagedResult<AppointmentDto>> SearchAsync(AppointmentSearchRequest request, CancellationToken cancellationToken = default);

    Task<AppointmentDetailsDto?> UpdateAsync(long id, UpdateAppointmentRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
