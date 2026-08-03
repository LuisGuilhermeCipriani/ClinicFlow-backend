namespace ClinicFlow.Application.PatientHistory;

public sealed record PatientHistoryEntryDto(
    long EntryId,
    PatientHistoryEntryType EntryType,
    DateTimeOffset OccurredAt,
    string Title,
    string? Description,
    long? AppointmentId,
    long? ClinicalRecordId,
    long DoctorId,
    string DoctorName,
    string? AppointmentDate,
    string? StartTime,
    string? EndTime);
