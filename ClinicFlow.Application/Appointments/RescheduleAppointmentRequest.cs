namespace ClinicFlow.Application.Appointments;

public sealed record RescheduleAppointmentRequest(
    long DoctorId,
    long PatientId,
    DateTime AppointmentDate,
    string StartTime,
    int DurationMinutes,
    string? Reason);
