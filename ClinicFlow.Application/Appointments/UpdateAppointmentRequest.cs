using ClinicFlow.Domain.Appointments;

namespace ClinicFlow.Application.Appointments;

public sealed record UpdateAppointmentRequest(
    long DoctorId,
    long PatientId,
    DateTime AppointmentDate,
    string StartTime,
    int DurationMinutes,
    AppointmentStatus Status);
