using ClinicFlow.Application.DoctorSchedules;
using ClinicFlow.Application.Doctors;
using ClinicFlow.Application.Patients;
using ClinicFlow.Domain.Appointments;
using ClinicFlow.Domain.DoctorSchedules;
using ClinicFlow.Domain.Doctors;
using ClinicFlow.Domain.Patients;
using System.Globalization;

namespace ClinicFlow.Application.Appointments;

public sealed class AppointmentService(
    IAppointmentRepository repository,
    IAppointmentHistoryRepository historyRepository,
    IDoctorRepository doctorRepository,
    IPatientRepository patientRepository,
    IDoctorScheduleRepository doctorScheduleRepository) : IAppointmentService
{
    public async Task<AppointmentDetailsDto> CreateAsync(CreateAppointmentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var doctor = await GetActiveDoctorAsync(request.DoctorId, cancellationToken).ConfigureAwait(false);
        var patient = await GetActivePatientAsync(request.PatientId, cancellationToken).ConfigureAwait(false);
        var appointmentDate = request.AppointmentDate.Date;
        var startMinute = ParseTimeToMinutes(request.StartTime, nameof(request.StartTime));
        var endMinute = startMinute + request.DurationMinutes;

        await EnsureDoctorHasAvailabilityAsync(doctor.Id, appointmentDate, startMinute, endMinute, cancellationToken).ConfigureAwait(false);
        await EnsureNoConflictsAsync(doctor.Id, patient.Id, appointmentDate, startMinute, endMinute, cancellationToken).ConfigureAwait(false);

        var appointment = Appointment.Create(
            request.DoctorId,
            request.PatientId,
            appointmentDate,
            startMinute,
            request.DurationMinutes,
            DateTimeOffset.UtcNow,
            "system");

        await repository.AddAsync(appointment, cancellationToken).ConfigureAwait(false);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return MapToDetailsDto(appointment, doctor.Name, patient.Name);
    }

    public async Task<AppointmentDetailsDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var appointment = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (appointment is null)
        {
            return null;
        }

        var doctor = await GetActiveDoctorAsync(appointment.DoctorId, cancellationToken).ConfigureAwait(false);
        var patient = await GetActivePatientAsync(appointment.PatientId, cancellationToken).ConfigureAwait(false);
        return MapToDetailsDto(appointment, doctor.Name, patient.Name);
    }

    public async Task<PagedResult<AppointmentDto>> SearchAsync(AppointmentSearchRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var result = await repository.SearchAsync(
            request.DoctorId,
            request.PatientId,
            request.AppointmentDate?.Date,
            request.Status,
            page,
            pageSize,
            cancellationToken).ConfigureAwait(false);

        var doctorNames = new Dictionary<long, string>();
        var patientNames = new Dictionary<long, string>();

        foreach (var appointment in result.Items)
        {
            if (!doctorNames.ContainsKey(appointment.DoctorId))
            {
                doctorNames[appointment.DoctorId] = (await doctorRepository.GetByIdAsync(appointment.DoctorId, cancellationToken).ConfigureAwait(false))?.Name ?? string.Empty;
            }

            if (!patientNames.ContainsKey(appointment.PatientId))
            {
                patientNames[appointment.PatientId] = (await patientRepository.GetByIdAsync(appointment.PatientId, cancellationToken).ConfigureAwait(false))?.Name ?? string.Empty;
            }
        }

        return new PagedResult<AppointmentDto>(
            result.Items.Select(appointment => MapToDto(appointment, doctorNames[appointment.DoctorId], patientNames[appointment.PatientId])).ToArray(),
            result.Page,
            result.PageSize,
            result.TotalCount);
    }

    public async Task<AppointmentDetailsDto?> UpdateAsync(long id, UpdateAppointmentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var appointment = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (appointment is null)
        {
            return null;
        }

        var doctor = await GetActiveDoctorAsync(request.DoctorId, cancellationToken).ConfigureAwait(false);
        var patient = await GetActivePatientAsync(request.PatientId, cancellationToken).ConfigureAwait(false);
        var appointmentDate = request.AppointmentDate.Date;
        var startMinute = ParseTimeToMinutes(request.StartTime, nameof(request.StartTime));
        var endMinute = startMinute + request.DurationMinutes;

        await EnsureDoctorHasAvailabilityAsync(doctor.Id, appointmentDate, startMinute, endMinute, cancellationToken).ConfigureAwait(false);
        await EnsureNoConflictsAsync(doctor.Id, patient.Id, appointmentDate, startMinute, endMinute, cancellationToken, appointment.Id).ConfigureAwait(false);

        appointment.Update(
            request.DoctorId,
            request.PatientId,
            appointmentDate,
            startMinute,
            request.DurationMinutes,
            DateTimeOffset.UtcNow,
            "system");

        if (request.Status == AppointmentStatus.Scheduled)
        {
            appointment.Schedule(DateTimeOffset.UtcNow, "system");
        }
        else if (request.Status == AppointmentStatus.Cancelled)
        {
            appointment.Cancel(DateTimeOffset.UtcNow, "system");
        }
        else
        {
            appointment.Complete(DateTimeOffset.UtcNow, "system");
        }

        repository.Update(appointment);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return MapToDetailsDto(appointment, doctor.Name, patient.Name);
    }

    public async Task<AppointmentDetailsDto?> CancelAsync(long id, CancelAppointmentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var appointment = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (appointment is null)
        {
            return null;
        }

        if (appointment.Status != AppointmentStatus.Scheduled)
        {
            throw new InvalidOperationException("A consulta precisa estar agendada para ser cancelada.");
        }

        var doctor = await GetActiveDoctorAsync(appointment.DoctorId, cancellationToken).ConfigureAwait(false);
        var patient = await GetActivePatientAsync(appointment.PatientId, cancellationToken).ConfigureAwait(false);

        var history = AppointmentHistory.CreateCancellation(
            id,
            appointment.AppointmentDate,
            appointment.StartMinute,
            appointment.EndMinute,
            request.Reason,
            DateTimeOffset.UtcNow,
            "system");

        appointment.Cancel(DateTimeOffset.UtcNow, "system");

        await historyRepository.AddAsync(history, cancellationToken).ConfigureAwait(false);
        repository.Update(appointment);
        await historyRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return MapToDetailsDto(appointment, doctor.Name, patient.Name);
    }

    public async Task<AppointmentDetailsDto?> RescheduleAsync(long id, RescheduleAppointmentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var appointment = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (appointment is null)
        {
            return null;
        }

        if (appointment.Status != AppointmentStatus.Scheduled)
        {
            throw new InvalidOperationException("A consulta precisa estar agendada para ser reagendada.");
        }

        var doctor = await GetActiveDoctorAsync(request.DoctorId, cancellationToken).ConfigureAwait(false);
        var patient = await GetActivePatientAsync(request.PatientId, cancellationToken).ConfigureAwait(false);
        var appointmentDate = request.AppointmentDate.Date;
        var startMinute = ParseTimeToMinutes(request.StartTime, nameof(request.StartTime));
        var endMinute = startMinute + request.DurationMinutes;

        await EnsureDoctorHasAvailabilityAsync(doctor.Id, appointmentDate, startMinute, endMinute, cancellationToken).ConfigureAwait(false);
        await EnsureNoConflictsAsync(doctor.Id, patient.Id, appointmentDate, startMinute, endMinute, cancellationToken, appointment.Id).ConfigureAwait(false);

        var history = AppointmentHistory.CreateReschedule(
            id,
            appointment.AppointmentDate,
            appointment.StartMinute,
            appointment.EndMinute,
            appointmentDate,
            startMinute,
            endMinute,
            request.Reason,
            DateTimeOffset.UtcNow,
            "system");

        appointment.Update(
            request.DoctorId,
            request.PatientId,
            appointmentDate,
            startMinute,
            request.DurationMinutes,
            DateTimeOffset.UtcNow,
            "system");
        appointment.Schedule(DateTimeOffset.UtcNow, "system");

        await historyRepository.AddAsync(history, cancellationToken).ConfigureAwait(false);
        repository.Update(appointment);
        await historyRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return MapToDetailsDto(appointment, doctor.Name, patient.Name);
    }

    public async Task<IReadOnlyCollection<AppointmentHistoryDto>?> GetHistoryAsync(long id, CancellationToken cancellationToken = default)
    {
        var appointment = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (appointment is null)
        {
            return null;
        }

        var histories = await historyRepository.GetByAppointmentIdAsync(id, cancellationToken).ConfigureAwait(false);
        return histories
            .OrderByDescending(history => history.CreatedAt)
            .Select(MapToHistoryDto)
            .ToArray();
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var appointment = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (appointment is null)
        {
            return false;
        }

        appointment.Delete(DateTimeOffset.UtcNow, "system");
        repository.Remove(appointment);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    private async Task<Doctor> GetActiveDoctorAsync(long doctorId, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByIdAsync(doctorId, cancellationToken).ConfigureAwait(false);
        if (doctor is null || doctor.IsDeleted || doctor.Status != DoctorStatus.Active)
        {
            throw new InvalidOperationException("O médico informado não está disponível.");
        }

        return doctor;
    }

    private async Task<Patient> GetActivePatientAsync(long patientId, CancellationToken cancellationToken)
    {
        var patient = await patientRepository.GetByIdAsync(patientId, cancellationToken).ConfigureAwait(false);
        if (patient is null || patient.IsDeleted || patient.Status != PatientStatus.Active)
        {
            throw new InvalidOperationException("O paciente informado não está disponível.");
        }

        return patient;
    }

    private async Task EnsureDoctorHasAvailabilityAsync(long doctorId, DateTime appointmentDate, int startMinute, int endMinute, CancellationToken cancellationToken)
    {
        var dayOfWeek = appointmentDate.DayOfWeek;
        var schedules = await doctorScheduleRepository.SearchAsync(doctorId, dayOfWeek, DoctorScheduleStatus.Active, 1, 1000, cancellationToken).ConfigureAwait(false);

        var hasAvailability = schedules.Items.Any(schedule =>
            schedule.StartMinute <= startMinute &&
            schedule.EndMinute >= endMinute &&
            !schedule.IsDeleted);

        if (!hasAvailability)
        {
            throw new InvalidOperationException("O médico não possui disponibilidade para o horário informado.");
        }
    }

    private async Task EnsureNoConflictsAsync(long doctorId, long patientId, DateTime appointmentDate, int startMinute, int endMinute, CancellationToken cancellationToken, long? excludeId = null)
    {
        if (await repository.HasDoctorConflictAsync(doctorId, appointmentDate, startMinute, endMinute, excludeId, cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException("Já existe uma consulta para esse médico nesse horário.");
        }

        if (await repository.HasPatientConflictAsync(patientId, appointmentDate, startMinute, endMinute, excludeId, cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException("Já existe uma consulta para esse paciente nesse horário.");
        }
    }

    private static int ParseTimeToMinutes(string value, string parameterName)
    {
        if (!TimeOnly.TryParseExact(value, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
        {
            throw new ArgumentException("O horário deve estar no formato HH:mm.", parameterName);
        }

        return time.Hour * 60 + time.Minute;
    }

    private static string FormatMinutes(int minutes)
    {
        var hours = minutes / 60;
        var remainder = minutes % 60;
        return $"{hours:00}:{remainder:00}";
    }

    private static AppointmentDto MapToDto(Appointment appointment, string doctorName, string patientName)
    {
        return new AppointmentDto(
            appointment.Id,
            appointment.DoctorId,
            doctorName,
            appointment.PatientId,
            patientName,
            appointment.AppointmentDate,
            FormatMinutes(appointment.StartMinute),
            FormatMinutes(appointment.EndMinute),
            appointment.DurationMinutes,
            appointment.Status,
            appointment.IsDeleted,
            appointment.CreatedAt,
            appointment.UpdatedAt);
    }

    private static AppointmentDetailsDto MapToDetailsDto(Appointment appointment, string doctorName, string patientName)
    {
        return new AppointmentDetailsDto(
            appointment.Id,
            appointment.DoctorId,
            doctorName,
            appointment.PatientId,
            patientName,
            appointment.AppointmentDate,
            FormatMinutes(appointment.StartMinute),
            FormatMinutes(appointment.EndMinute),
            appointment.DurationMinutes,
            appointment.Status,
            appointment.IsDeleted,
            appointment.CreatedAt,
            appointment.CreatedBy,
            appointment.UpdatedAt,
            appointment.UpdatedBy,
            appointment.DeletedAt,
            appointment.DeletedBy);
    }

    private static AppointmentHistoryDto MapToHistoryDto(AppointmentHistory history)
    {
        return new AppointmentHistoryDto(
            history.Id,
            history.AppointmentId,
            history.ChangeType,
            history.PreviousAppointmentDate,
            FormatMinutes(history.PreviousStartMinute),
            FormatMinutes(history.PreviousEndMinute),
            history.NewAppointmentDate,
            history.NewStartMinute is null ? null : FormatMinutes(history.NewStartMinute.Value),
            history.NewEndMinute is null ? null : FormatMinutes(history.NewEndMinute.Value),
            history.Reason,
            history.CreatedAt,
            history.CreatedBy);
    }
}
