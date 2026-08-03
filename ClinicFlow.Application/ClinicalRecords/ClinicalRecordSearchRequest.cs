namespace ClinicFlow.Application.ClinicalRecords;

public sealed record ClinicalRecordSearchRequest(
    long? AppointmentId,
    long? PatientId,
    long? DoctorId,
    string? SearchTerm,
    int Page = 1,
    int PageSize = 10);
