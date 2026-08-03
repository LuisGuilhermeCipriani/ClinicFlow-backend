namespace ClinicFlow.Application.ClinicalRecords;

public sealed record ClinicalRecordDetailsDto(
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
    string? Notes,
    DateTimeOffset CreatedAt,
    string? CreatedBy,
    DateTimeOffset? UpdatedAt,
    string? UpdatedBy,
    DateTimeOffset? DeletedAt,
    string? DeletedBy);
