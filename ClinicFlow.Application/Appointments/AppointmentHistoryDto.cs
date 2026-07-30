using ClinicFlow.Domain.Appointments;

namespace ClinicFlow.Application.Appointments;

public sealed record AppointmentHistoryDto(
    long Id,
    long AppointmentId,
    AppointmentChangeType ChangeType,
    DateTime PreviousAppointmentDate,
    string PreviousStartTime,
    string PreviousEndTime,
    DateTime? NewAppointmentDate,
    string? NewStartTime,
    string? NewEndTime,
    string? Reason,
    DateTimeOffset CreatedAt,
    string? CreatedBy);
