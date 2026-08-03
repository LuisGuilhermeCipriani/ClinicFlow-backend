namespace ClinicFlow.Application.ClinicalRecords;

public interface IClinicalRecordService
{
    Task<ClinicalRecordDetailsDto> CreateAsync(CreateClinicalRecordRequest request, CancellationToken cancellationToken = default);

    Task<ClinicalRecordDetailsDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<ClinicalRecordDetailsDto?> GetByAppointmentIdAsync(long appointmentId, CancellationToken cancellationToken = default);

    Task<PagedResult<ClinicalRecordDto>> SearchAsync(ClinicalRecordSearchRequest request, CancellationToken cancellationToken = default);

    Task<ClinicalRecordDetailsDto?> UpdateAsync(long id, UpdateClinicalRecordRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
