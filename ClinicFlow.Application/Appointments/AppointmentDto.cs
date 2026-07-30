using ClinicFlow.Domain.Appointments;

namespace ClinicFlow.Application.Appointments;

public sealed record AppointmentDto(
    long Id,
    long DoctorId,
    string DoctorName,
    long PatientId,
    string PatientName,
    DateTime AppointmentDate,
    string StartTime,
    string EndTime,
    int DurationMinutes,
    AppointmentStatus Status,
    bool IsDeleted,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
