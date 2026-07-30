using ClinicFlow.Domain.Appointments;

namespace ClinicFlow.Application.Appointments;

public sealed record AppointmentSearchRequest(
    long? DoctorId,
    long? PatientId,
    DateTime? AppointmentDate,
    AppointmentStatus? Status,
    int Page = 1,
    int PageSize = 10);
