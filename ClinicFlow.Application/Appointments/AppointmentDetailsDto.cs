using ClinicFlow.Domain.Appointments;

namespace ClinicFlow.Application.Appointments;

public sealed record AppointmentDetailsDto(
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
    string? CreatedBy,
    DateTimeOffset? UpdatedAt,
    string? UpdatedBy,
    DateTimeOffset? DeletedAt,
    string? DeletedBy);
