namespace ClinicFlow.Application.Appointments;

public sealed record CreateAppointmentRequest(
    long DoctorId,
    long PatientId,
    DateTime AppointmentDate,
    string StartTime,
    int DurationMinutes);
