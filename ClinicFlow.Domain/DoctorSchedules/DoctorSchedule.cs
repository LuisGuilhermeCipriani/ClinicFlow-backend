using ClinicFlow.Domain.Doctors;
using ClinicFlow.Domain.Exceptions;
using ClinicFlow.Domain.Primitives;

namespace ClinicFlow.Domain.DoctorSchedules;

public sealed class DoctorSchedule : AuditableEntity
{
    public const int MaxSlotDurationMinutes = 240;
    public const int MinSlotDurationMinutes = 10;
    public const int MinutesPerDay = 24 * 60;

    public long DoctorId { get; private set; }

    public Doctor? Doctor { get; private set; }

    public DayOfWeek DayOfWeek { get; private set; }

    public int StartMinute { get; private set; }

    public int EndMinute { get; private set; }

    public int SlotDurationMinutes { get; private set; }

    public DoctorScheduleStatus Status { get; private set; } = DoctorScheduleStatus.Active;

    private DoctorSchedule()
    {
    }

    public static DoctorSchedule Create(
        long doctorId,
        DayOfWeek dayOfWeek,
        int startMinute,
        int endMinute,
        int slotDurationMinutes,
        DateTimeOffset createdAt,
        string? createdBy)
    {
        Validate(doctorId, dayOfWeek, startMinute, endMinute, slotDurationMinutes);

        var schedule = new DoctorSchedule
        {
            DoctorId = doctorId,
            DayOfWeek = dayOfWeek,
            StartMinute = startMinute,
            EndMinute = endMinute,
            SlotDurationMinutes = slotDurationMinutes,
            Status = DoctorScheduleStatus.Active
        };

        schedule.MarkCreated(createdAt, createdBy);
        return schedule;
    }

    public void Update(
        long doctorId,
        DayOfWeek dayOfWeek,
        int startMinute,
        int endMinute,
        int slotDurationMinutes,
        DateTimeOffset updatedAt,
        string? updatedBy)
    {
        Validate(doctorId, dayOfWeek, startMinute, endMinute, slotDurationMinutes);

        DoctorId = doctorId;
        DayOfWeek = dayOfWeek;
        StartMinute = startMinute;
        EndMinute = endMinute;
        SlotDurationMinutes = slotDurationMinutes;
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Activate(DateTimeOffset updatedAt, string? updatedBy)
    {
        Status = DoctorScheduleStatus.Active;
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Deactivate(DateTimeOffset updatedAt, string? updatedBy)
    {
        Status = DoctorScheduleStatus.Inactive;
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Delete(DateTimeOffset deletedAt, string? deletedBy)
    {
        Status = DoctorScheduleStatus.Inactive;
        MarkDeleted(deletedAt, deletedBy);
    }

    private static void Validate(
        long doctorId,
        DayOfWeek dayOfWeek,
        int startMinute,
        int endMinute,
        int slotDurationMinutes)
    {
        var errors = new List<string>();

        if (doctorId <= 0)
        {
            errors.Add("O médico da agenda é obrigatório.");
        }

        if (!Enum.IsDefined(typeof(DayOfWeek), dayOfWeek))
        {
            errors.Add("O dia da semana da agenda é obrigatório.");
        }

        if (startMinute < 0 || startMinute >= MinutesPerDay)
        {
            errors.Add("O horário inicial da agenda é inválido.");
        }

        if (endMinute <= 0 || endMinute > MinutesPerDay)
        {
            errors.Add("O horário final da agenda é inválido.");
        }

        if (startMinute >= endMinute)
        {
            errors.Add("O horário inicial da agenda deve ser anterior ao horário final.");
        }

        if (slotDurationMinutes < MinSlotDurationMinutes || slotDurationMinutes > MaxSlotDurationMinutes)
        {
            errors.Add($"A duração dos horários deve ficar entre {MinSlotDurationMinutes} e {MaxSlotDurationMinutes} minutos.");
        }

        if (errors.Count > 0)
        {
            throw new DomainValidationException(errors);
        }
    }
}
