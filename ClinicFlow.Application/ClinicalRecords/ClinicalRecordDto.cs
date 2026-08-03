namespace ClinicFlow.Application.ClinicalRecords;

public sealed record ClinicalRecordDto(
    long Id,
    long AppointmentId,
    DateTime AppointmentDate,
    string AppointmentStartTime,
    string AppointmentEndTime,
    long PatientId,
    string PatientName,
    long DoctorId,
    string DoctorName,
    string ChiefComplaint,
    string? Diagnosis,
    string? Prescription,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
