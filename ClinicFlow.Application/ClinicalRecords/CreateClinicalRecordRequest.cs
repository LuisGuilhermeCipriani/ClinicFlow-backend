namespace ClinicFlow.Application.ClinicalRecords;

public sealed record CreateClinicalRecordRequest(
    long AppointmentId,
    string ChiefComplaint,
    string? Diagnosis,
    string? Prescription,
    string? Notes);
