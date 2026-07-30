namespace ClinicFlow.Application.Patients;

public interface IPatientService
{
    Task<PatientDetailsDto> CreateAsync(CreatePatientRequest request, CancellationToken cancellationToken = default);

    Task<PatientDetailsDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<PagedResult<PatientDto>> SearchAsync(PatientSearchRequest request, CancellationToken cancellationToken = default);

    Task<PatientDetailsDto?> UpdateAsync(long id, UpdatePatientRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);

    Task<PatientDetailsDto?> SetStatusAsync(long id, bool isActive, CancellationToken cancellationToken = default);
}
