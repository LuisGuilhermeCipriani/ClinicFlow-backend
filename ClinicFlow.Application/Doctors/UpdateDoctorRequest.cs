namespace ClinicFlow.Application.Doctors;

public sealed record UpdateDoctorRequest(
    string Name,
    string CrmNumber,
    string CrmState,
    long SpecialtyId,
    string Email,
    string Phone,
    ClinicFlow.Domain.Doctors.DoctorStatus Status);
