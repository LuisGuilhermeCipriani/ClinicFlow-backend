using ClinicFlow.Domain.Doctors;
using ClinicFlow.Domain.Exceptions;
using ClinicFlow.Domain.Patients;
using ClinicFlow.Domain.Primitives;

namespace ClinicFlow.Domain.Appointments;

public sealed class Appointment : AuditableEntity
{
    public const int MinDurationMinutes = 10;
    public const int MaxDurationMinutes = 240;
    public const int MinutesPerDay = 24 * 60;

    public long DoctorId { get; private set; }

    public Doctor? Doctor { get; private set; }

    public long PatientId { get; private set; }

    public Patient? Patient { get; private set; }

    public DateTime AppointmentDate { get; private set; }

    public int StartMinute { get; private set; }

    public int EndMinute { get; private set; }

    public int DurationMinutes { get; private set; }

    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Scheduled;

    private Appointment()
    {
    }

    public static Appointment Create(
        long doctorId,
        long patientId,
        DateTime appointmentDate,
        int startMinute,
        int durationMinutes,
        DateTimeOffset createdAt,
        string? createdBy)
    {
        Validate(doctorId, patientId, appointmentDate, startMinute, durationMinutes);

        var appointment = new Appointment
        {
            DoctorId = doctorId,
            PatientId = patientId,
            AppointmentDate = appointmentDate.Date,
            StartMinute = startMinute,
            DurationMinutes = durationMinutes,
            EndMinute = startMinute + durationMinutes,
            Status = AppointmentStatus.Scheduled
        };

        appointment.MarkCreated(createdAt, createdBy);
        return appointment;
    }

    public void Update(
        long doctorId,
        long patientId,
        DateTime appointmentDate,
        int startMinute,
        int durationMinutes,
        DateTimeOffset updatedAt,
        string? updatedBy)
    {
        Validate(doctorId, patientId, appointmentDate, startMinute, durationMinutes);

        DoctorId = doctorId;
        PatientId = patientId;
        AppointmentDate = appointmentDate.Date;
        StartMinute = startMinute;
        DurationMinutes = durationMinutes;
        EndMinute = startMinute + durationMinutes;
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Cancel(DateTimeOffset updatedAt, string? updatedBy)
    {
        Status = AppointmentStatus.Cancelled;
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Schedule(DateTimeOffset updatedAt, string? updatedBy)
    {
        Status = AppointmentStatus.Scheduled;
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Complete(DateTimeOffset updatedAt, string? updatedBy)
    {
        Status = AppointmentStatus.Completed;
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Delete(DateTimeOffset deletedAt, string? deletedBy)
    {
        Status = AppointmentStatus.Cancelled;
        MarkDeleted(deletedAt, deletedBy);
    }

    private static void Validate(
        long doctorId,
        long patientId,
        DateTime appointmentDate,
        int startMinute,
        int durationMinutes)
    {
        var errors = new List<string>();

        if (doctorId <= 0)
        {
            errors.Add("O médico da consulta é obrigatório.");
        }

        if (patientId <= 0)
        {
            errors.Add("O paciente da consulta é obrigatório.");
        }

        if (appointmentDate == default)
        {
            errors.Add("A data da consulta é obrigatória.");
        }

        if (startMinute < 0 || startMinute >= MinutesPerDay)
        {
            errors.Add("O horário inicial da consulta é inválido.");
        }

        if (durationMinutes < MinDurationMinutes || durationMinutes > MaxDurationMinutes)
        {
            errors.Add($"A duração da consulta deve ficar entre {MinDurationMinutes} e {MaxDurationMinutes} minutos.");
        }

        var endMinute = startMinute + durationMinutes;
        if (startMinute >= endMinute)
        {
            errors.Add("O horário inicial da consulta deve ser anterior ao horário final.");
        }

        if (endMinute > MinutesPerDay)
        {
            errors.Add("A consulta não pode ultrapassar o fim do dia.");
        }

        if (errors.Count > 0)
        {
            throw new DomainValidationException(errors);
        }
    }
}
