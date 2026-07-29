namespace ClinicFlow.Application.Doctors;

public sealed record CreateDoctorRequest(
    string Name,
    string CrmNumber,
    string CrmState,
    long SpecialtyId,
    string Email,
    string Phone);
