namespace ClinicFlow.Application.PatientHistory;

public interface IPatientHistoryService
{
    Task<PatientHistoryDto?> GetByPatientIdAsync(long patientId, CancellationToken cancellationToken = default);
}
