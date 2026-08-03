namespace ClinicFlow.Application.ClinicalRecords;

public sealed record UpdateClinicalRecordRequest(
    string ChiefComplaint,
    string? Diagnosis,
    string? Prescription,
    string? Notes);
