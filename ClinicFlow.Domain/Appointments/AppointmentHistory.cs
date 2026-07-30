using ClinicFlow.Domain.Exceptions;
using ClinicFlow.Domain.Primitives;

namespace ClinicFlow.Domain.Appointments;

public sealed class AppointmentHistory : AuditableEntity
{
    public const int MaxReasonLength = 500;

    public long AppointmentId { get; private set; }

    public Appointment? Appointment { get; private set; }

    public AppointmentChangeType ChangeType { get; private set; }

    public DateTime PreviousAppointmentDate { get; private set; }

    public int PreviousStartMinute { get; private set; }

    public int PreviousEndMinute { get; private set; }

    public DateTime? NewAppointmentDate { get; private set; }

    public int? NewStartMinute { get; private set; }

    public int? NewEndMinute { get; private set; }

    public string? Reason { get; private set; }

    private AppointmentHistory()
    {
    }

    public static AppointmentHistory CreateCancellation(
        long appointmentId,
        DateTime previousAppointmentDate,
        int previousStartMinute,
        int previousEndMinute,
        string? reason,
        DateTimeOffset createdAt,
        string? createdBy)
    {
        Validate(appointmentId, previousAppointmentDate, previousStartMinute, previousEndMinute, reason);

        var history = new AppointmentHistory
        {
            AppointmentId = appointmentId,
            ChangeType = AppointmentChangeType.Cancelled,
            PreviousAppointmentDate = previousAppointmentDate.Date,
            PreviousStartMinute = previousStartMinute,
            PreviousEndMinute = previousEndMinute,
            Reason = NormalizeReason(reason)
        };

        history.MarkCreated(createdAt, createdBy);
        return history;
    }

    public static AppointmentHistory CreateReschedule(
        long appointmentId,
        DateTime previousAppointmentDate,
        int previousStartMinute,
        int previousEndMinute,
        DateTime newAppointmentDate,
        int newStartMinute,
        int newEndMinute,
        string? reason,
        DateTimeOffset createdAt,
        string? createdBy)
    {
        Validate(appointmentId, previousAppointmentDate, previousStartMinute, previousEndMinute, reason);
        ValidateNewValues(newAppointmentDate, newStartMinute, newEndMinute);

        var history = new AppointmentHistory
        {
            AppointmentId = appointmentId,
            ChangeType = AppointmentChangeType.Rescheduled,
            PreviousAppointmentDate = previousAppointmentDate.Date,
            PreviousStartMinute = previousStartMinute,
            PreviousEndMinute = previousEndMinute,
            NewAppointmentDate = newAppointmentDate.Date,
            NewStartMinute = newStartMinute,
            NewEndMinute = newEndMinute,
            Reason = NormalizeReason(reason)
        };

        history.MarkCreated(createdAt, createdBy);
        return history;
    }

    private static void Validate(
        long appointmentId,
        DateTime previousAppointmentDate,
        int previousStartMinute,
        int previousEndMinute,
        string? reason)
    {
        var errors = new List<string>();

        if (appointmentId <= 0)
        {
            errors.Add("A consulta vinculada ao histórico é obrigatória.");
        }

        if (previousAppointmentDate == default)
        {
            errors.Add("A data anterior da consulta é obrigatória.");
        }

        if (previousStartMinute < 0)
        {
            errors.Add("O horário anterior inicial da consulta é inválido.");
        }

        if (previousEndMinute <= previousStartMinute)
        {
            errors.Add("O horário anterior final da consulta é inválido.");
        }

        if (reason is not null && reason.Trim().Length > MaxReasonLength)
        {
            errors.Add($"A justificativa deve ter no máximo {MaxReasonLength} caracteres.");
        }

        if (errors.Count > 0)
        {
            throw new DomainValidationException(errors);
        }
    }

    private static void ValidateNewValues(DateTime newAppointmentDate, int newStartMinute, int newEndMinute)
    {
        var errors = new List<string>();

        if (newAppointmentDate == default)
        {
            errors.Add("A nova data da consulta é obrigatória.");
        }

        if (newStartMinute < 0)
        {
            errors.Add("O novo horário inicial da consulta é inválido.");
        }

        if (newEndMinute <= newStartMinute)
        {
            errors.Add("O novo horário final da consulta é inválido.");
        }

        if (errors.Count > 0)
        {
            throw new DomainValidationException(errors);
        }
    }

    private static string? NormalizeReason(string? reason)
    {
        return string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
    }
}
