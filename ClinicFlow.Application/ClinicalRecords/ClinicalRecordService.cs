using ClinicFlow.Application.Appointments;
using ClinicFlow.Application.Doctors;
using ClinicFlow.Application.Patients;
using ClinicFlow.Domain.Appointments;
using ClinicFlow.Domain.ClinicalRecords;
using ClinicFlow.Domain.Doctors;
using ClinicFlow.Domain.Patients;

namespace ClinicFlow.Application.ClinicalRecords;

public sealed class ClinicalRecordService(
    IClinicalRecordRepository repository,
    IAppointmentRepository appointmentRepository,
    IDoctorRepository doctorRepository,
    IPatientRepository patientRepository) : IClinicalRecordService
{
    public async Task<ClinicalRecordDetailsDto> CreateAsync(CreateClinicalRecordRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var appointment = await GetCompletedAppointmentAsync(request.AppointmentId, cancellationToken).ConfigureAwait(false);
        var existing = await repository.GetByAppointmentIdAsync(request.AppointmentId, cancellationToken).ConfigureAwait(false);
        if (existing is not null)
        {
            throw new InvalidOperationException("Já existe um prontuário para essa consulta.");
        }

        var record = ClinicalRecord.Create(
            appointment.Id,
            appointment.PatientId,
            appointment.DoctorId,
            request.ChiefComplaint,
            request.Diagnosis,
            request.Prescription,
            request.Notes,
            DateTimeOffset.UtcNow,
            "system");

        await repository.AddAsync(record, cancellationToken).ConfigureAwait(false);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return await MapToDetailsDtoAsync(record, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ClinicalRecordDetailsDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var record = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return record is null ? null : await MapToDetailsDtoAsync(record, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ClinicalRecordDetailsDto?> GetByAppointmentIdAsync(long appointmentId, CancellationToken cancellationToken = default)
    {
        var record = await repository.GetByAppointmentIdAsync(appointmentId, cancellationToken).ConfigureAwait(false);
        return record is null ? null : await MapToDetailsDtoAsync(record, cancellationToken).ConfigureAwait(false);
    }

    public async Task<PagedResult<ClinicalRecordDto>> SearchAsync(ClinicalRecordSearchRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var result = await repository.SearchAsync(
            request.AppointmentId,
            request.PatientId,
            request.DoctorId,
            request.SearchTerm,
            page,
            pageSize,
            cancellationToken).ConfigureAwait(false);

        var appointmentCache = new Dictionary<long, Appointment?>();

        foreach (var record in result.Items)
        {
            if (!appointmentCache.ContainsKey(record.AppointmentId))
            {
                appointmentCache[record.AppointmentId] = await appointmentRepository.GetByIdAsync(record.AppointmentId, cancellationToken).ConfigureAwait(false);
            }
        }

        return new PagedResult<ClinicalRecordDto>(
            result.Items.Select(record => MapToDto(record, appointmentCache[record.AppointmentId])).ToArray(),
            result.Page,
            result.PageSize,
            result.TotalCount);
    }

    public async Task<ClinicalRecordDetailsDto?> UpdateAsync(long id, UpdateClinicalRecordRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var record = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (record is null)
        {
            return null;
        }

        record.Update(request.ChiefComplaint, request.Diagnosis, request.Prescription, request.Notes, DateTimeOffset.UtcNow, "system");
        repository.Update(record);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return await MapToDetailsDtoAsync(record, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var record = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (record is null)
        {
            return false;
        }

        record.Delete(DateTimeOffset.UtcNow, "system");
        repository.Remove(record);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    private async Task<Appointment> GetCompletedAppointmentAsync(long appointmentId, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(appointmentId, cancellationToken).ConfigureAwait(false);
        if (appointment is null || appointment.IsDeleted)
        {
            throw new InvalidOperationException("A consulta informada não foi encontrada.");
        }

        if (appointment.Status != AppointmentStatus.Completed)
        {
            throw new InvalidOperationException("A consulta precisa estar concluída para registrar o prontuário.");
        }

        return appointment;
    }

    private async Task<ClinicalRecordDetailsDto> MapToDetailsDtoAsync(ClinicalRecord record, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(record.AppointmentId, cancellationToken).ConfigureAwait(false);
        var patient = await patientRepository.GetByIdAsync(record.PatientId, cancellationToken).ConfigureAwait(false);
        var doctor = await doctorRepository.GetByIdAsync(record.DoctorId, cancellationToken).ConfigureAwait(false);

        return new ClinicalRecordDetailsDto(
            record.Id,
            record.AppointmentId,
            appointment?.AppointmentDate ?? DateTime.MinValue,
            appointment is null ? string.Empty : FormatMinutes(appointment.StartMinute),
            appointment is null ? string.Empty : FormatMinutes(appointment.EndMinute),
            record.PatientId,
            patient?.Name ?? string.Empty,
            record.DoctorId,
            doctor?.Name ?? string.Empty,
            record.ChiefComplaint,
            record.Diagnosis,
            record.Prescription,
            record.Notes,
            record.CreatedAt,
            record.CreatedBy,
            record.UpdatedAt,
            record.UpdatedBy,
            record.DeletedAt,
            record.DeletedBy);
    }

    private static ClinicalRecordDto MapToDto(ClinicalRecord record, Appointment? appointment)
    {
        return new ClinicalRecordDto(
            record.Id,
            record.AppointmentId,
            appointment?.AppointmentDate ?? DateTime.MinValue,
            appointment is null ? string.Empty : FormatMinutes(appointment.StartMinute),
            appointment is null ? string.Empty : FormatMinutes(appointment.EndMinute),
            record.PatientId,
            appointment?.Patient?.Name ?? string.Empty,
            record.DoctorId,
            appointment?.Doctor?.Name ?? string.Empty,
            record.ChiefComplaint,
            record.Diagnosis,
            record.Prescription,
            record.CreatedAt,
            record.UpdatedAt);
    }

    private static string FormatMinutes(int minutes)
    {
        var hours = minutes / 60;
        var remainder = minutes % 60;
        return $"{hours:00}:{remainder:00}";
    }
}
