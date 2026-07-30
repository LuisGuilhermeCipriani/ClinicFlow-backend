using ClinicFlow.Application.Doctors;
using ClinicFlow.Domain.DoctorSchedules;
using ClinicFlow.Domain.Doctors;
using System.Globalization;

namespace ClinicFlow.Application.DoctorSchedules;

public sealed class DoctorScheduleService(
    IDoctorScheduleRepository repository,
    IDoctorRepository doctorRepository) : IDoctorScheduleService
{
    public async Task<DoctorScheduleDetailsDto> CreateAsync(CreateDoctorScheduleRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var doctor = await GetDoctorOrThrowAsync(request.DoctorId, cancellationToken).ConfigureAwait(false);
        var startMinute = ParseTimeToMinutes(request.StartTime, nameof(request.StartTime));
        var endMinute = ParseTimeToMinutes(request.EndTime, nameof(request.EndTime));

        if (await repository.ExistsAsync(request.DoctorId, request.DayOfWeek, startMinute, endMinute, cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException("Já existe uma agenda para esse médico nesse horário.");
        }

        var schedule = DoctorSchedule.Create(
            request.DoctorId,
            request.DayOfWeek,
            startMinute,
            endMinute,
            request.SlotDurationMinutes,
            DateTimeOffset.UtcNow,
            "system");

        await repository.AddAsync(schedule, cancellationToken).ConfigureAwait(false);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return MapToDetailsDto(schedule, doctor.Name);
    }

    public async Task<DoctorScheduleDetailsDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var schedule = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (schedule is null)
        {
            return null;
        }

        var doctor = await GetDoctorOrThrowAsync(schedule.DoctorId, cancellationToken).ConfigureAwait(false);
        return MapToDetailsDto(schedule, doctor.Name);
    }

    public async Task<PagedResult<DoctorScheduleDto>> SearchAsync(DoctorScheduleSearchRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var result = await repository.SearchAsync(request.DoctorId, request.DayOfWeek, request.Status, page, pageSize, cancellationToken).ConfigureAwait(false);

        var doctorNames = new Dictionary<long, string>();
        foreach (var schedule in result.Items)
        {
            if (!doctorNames.ContainsKey(schedule.DoctorId))
            {
                var doctor = await doctorRepository.GetByIdAsync(schedule.DoctorId, cancellationToken).ConfigureAwait(false);
                doctorNames[schedule.DoctorId] = doctor?.Name ?? string.Empty;
            }
        }

        return new PagedResult<DoctorScheduleDto>(
            result.Items.Select(schedule => MapToDto(schedule, doctorNames[schedule.DoctorId])).ToArray(),
            result.Page,
            result.PageSize,
            result.TotalCount);
    }

    public async Task<DoctorScheduleDetailsDto?> UpdateAsync(long id, UpdateDoctorScheduleRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var schedule = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (schedule is null)
        {
            return null;
        }

        var doctor = await GetDoctorOrThrowAsync(request.DoctorId, cancellationToken).ConfigureAwait(false);
        var startMinute = ParseTimeToMinutes(request.StartTime, nameof(request.StartTime));
        var endMinute = ParseTimeToMinutes(request.EndTime, nameof(request.EndTime));

        if (await repository.ExistsAsync(request.DoctorId, request.DayOfWeek, startMinute, endMinute, id, cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException("Já existe uma agenda para esse médico nesse horário.");
        }

        schedule.Update(
            request.DoctorId,
            request.DayOfWeek,
            startMinute,
            endMinute,
            request.SlotDurationMinutes,
            DateTimeOffset.UtcNow,
            "system");

        if (request.Status == DoctorScheduleStatus.Active)
        {
            schedule.Activate(DateTimeOffset.UtcNow, "system");
        }
        else
        {
            schedule.Deactivate(DateTimeOffset.UtcNow, "system");
        }

        repository.Update(schedule);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return MapToDetailsDto(schedule, doctor.Name);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var schedule = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (schedule is null)
        {
            return false;
        }

        schedule.Delete(DateTimeOffset.UtcNow, "system");
        repository.Remove(schedule);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task<DoctorScheduleDetailsDto?> SetStatusAsync(long id, bool isActive, CancellationToken cancellationToken = default)
    {
        var schedule = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (schedule is null)
        {
            return null;
        }

        if (isActive)
        {
            schedule.Activate(DateTimeOffset.UtcNow, "system");
        }
        else
        {
            schedule.Deactivate(DateTimeOffset.UtcNow, "system");
        }

        repository.Update(schedule);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var doctor = await GetDoctorOrThrowAsync(schedule.DoctorId, cancellationToken).ConfigureAwait(false);
        return MapToDetailsDto(schedule, doctor.Name);
    }

    private async Task<Doctor> GetDoctorOrThrowAsync(long doctorId, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByIdAsync(doctorId, cancellationToken).ConfigureAwait(false);
        if (doctor is null || doctor.IsDeleted)
        {
            throw new InvalidOperationException("O médico informado não existe.");
        }

        return doctor;
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

    private static DoctorScheduleDto MapToDto(DoctorSchedule schedule, string doctorName)
    {
        return new DoctorScheduleDto(
            schedule.Id,
            schedule.DoctorId,
            doctorName,
            schedule.DayOfWeek,
            FormatMinutes(schedule.StartMinute),
            FormatMinutes(schedule.EndMinute),
            schedule.SlotDurationMinutes,
            schedule.Status,
            schedule.IsDeleted,
            schedule.CreatedAt,
            schedule.UpdatedAt);
    }

    private static DoctorScheduleDetailsDto MapToDetailsDto(DoctorSchedule schedule, string doctorName)
    {
        return new DoctorScheduleDetailsDto(
            schedule.Id,
            schedule.DoctorId,
            doctorName,
            schedule.DayOfWeek,
            FormatMinutes(schedule.StartMinute),
            FormatMinutes(schedule.EndMinute),
            schedule.SlotDurationMinutes,
            schedule.Status,
            schedule.IsDeleted,
            schedule.CreatedAt,
            schedule.CreatedBy,
            schedule.UpdatedAt,
            schedule.UpdatedBy,
            schedule.DeletedAt,
            schedule.DeletedBy);
    }
}
