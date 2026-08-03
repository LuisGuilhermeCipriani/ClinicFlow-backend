using ClinicFlow.Application.Appointments;
using ClinicFlow.Application.ClinicalRecords;
using ClinicFlow.Application.Doctors;
using ClinicFlow.Domain.Appointments;
using ClinicFlow.Domain.ClinicalRecords;
using ClinicFlow.Domain.Doctors;
using ClinicFlow.Domain.Patients;

namespace ClinicFlow.Application.PatientHistory;

public sealed class PatientHistoryService(
    IPatientRepository patientRepository,
    IAppointmentRepository appointmentRepository,
    IAppointmentHistoryRepository appointmentHistoryRepository,
    IClinicalRecordRepository clinicalRecordRepository,
    IDoctorRepository doctorRepository) : IPatientHistoryService
{
    public async Task<PatientHistoryDto?> GetByPatientIdAsync(long patientId, CancellationToken cancellationToken = default)
    {
        var patient = await patientRepository.GetByIdAsync(patientId, cancellationToken).ConfigureAwait(false);
        if (patient is null)
        {
            return null;
        }

        var appointments = await appointmentRepository.GetByPatientIdAsync(patientId, cancellationToken).ConfigureAwait(false);
        var clinicalRecords = await clinicalRecordRepository.GetByPatientIdAsync(patientId, cancellationToken).ConfigureAwait(false);
        var entries = new List<PatientHistoryEntryDto>();

        foreach (var appointment in appointments)
        {
            var doctorName = await GetDoctorNameAsync(appointment.DoctorId, cancellationToken).ConfigureAwait(false);

            entries.Add(new PatientHistoryEntryDto(
                appointment.Id,
                MapAppointmentStatus(appointment.Status),
                appointment.CreatedAt,
                GetAppointmentTitle(appointment.Status),
                BuildAppointmentDescription(appointment, doctorName),
                appointment.Id,
                null,
                appointment.DoctorId,
                doctorName,
                appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                FormatMinutes(appointment.StartMinute),
                FormatMinutes(appointment.EndMinute)));

            var histories = await appointmentHistoryRepository.GetByAppointmentIdAsync(appointment.Id, cancellationToken).ConfigureAwait(false);
            foreach (var history in histories)
            {
                entries.Add(new PatientHistoryEntryDto(
                    history.Id,
                    MapAppointmentChangeType(history.ChangeType),
                    history.CreatedAt,
                    GetAppointmentHistoryTitle(history.ChangeType),
                    history.Reason,
                    appointment.Id,
                    null,
                    appointment.DoctorId,
                    doctorName,
                    history.ChangeType == AppointmentChangeType.Rescheduled ? history.NewAppointmentDate?.ToString("yyyy-MM-dd") : appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                    history.ChangeType == AppointmentChangeType.Rescheduled && history.NewStartMinute is not null ? FormatMinutes(history.NewStartMinute.Value) : FormatMinutes(appointment.StartMinute),
                    history.ChangeType == AppointmentChangeType.Rescheduled && history.NewEndMinute is not null ? FormatMinutes(history.NewEndMinute.Value) : FormatMinutes(appointment.EndMinute)));
            }
        }

        foreach (var record in clinicalRecords)
        {
            var appointment = appointments.FirstOrDefault(item => item.Id == record.AppointmentId)
                ?? await appointmentRepository.GetByIdAsync(record.AppointmentId, cancellationToken).ConfigureAwait(false);

            var doctorName = await GetDoctorNameAsync(record.DoctorId, cancellationToken).ConfigureAwait(false);

            entries.Add(new PatientHistoryEntryDto(
                record.Id,
                PatientHistoryEntryType.ClinicalRecord,
                record.CreatedAt,
                "Prontuário registrado",
                BuildClinicalRecordDescription(record),
                record.AppointmentId,
                record.Id,
                record.DoctorId,
                doctorName,
                appointment?.AppointmentDate.ToString("yyyy-MM-dd"),
                appointment is null ? null : FormatMinutes(appointment.StartMinute),
                appointment is null ? null : FormatMinutes(appointment.EndMinute)));
        }

        var ordered = entries
            .OrderByDescending(entry => entry.OccurredAt)
            .ThenByDescending(entry => entry.EntryId)
            .ToArray();

        return new PatientHistoryDto(patient.Id, patient.Name, ordered);
    }

    private async Task<string> GetDoctorNameAsync(long doctorId, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByIdAsync(doctorId, cancellationToken).ConfigureAwait(false);
        return doctor?.Name ?? string.Empty;
    }

    private static PatientHistoryEntryType MapAppointmentStatus(AppointmentStatus status)
    {
        return status switch
        {
            AppointmentStatus.Scheduled => PatientHistoryEntryType.AppointmentScheduled,
            AppointmentStatus.Completed => PatientHistoryEntryType.AppointmentCompleted,
            _ => PatientHistoryEntryType.AppointmentCancelled
        };
    }

    private static PatientHistoryEntryType MapAppointmentChangeType(AppointmentChangeType changeType)
    {
        return changeType switch
        {
            AppointmentChangeType.Cancelled => PatientHistoryEntryType.AppointmentCancelled,
            _ => PatientHistoryEntryType.AppointmentRescheduled
        };
    }

    private static string GetAppointmentTitle(AppointmentStatus status)
    {
        return status switch
        {
            AppointmentStatus.Scheduled => "Consulta agendada",
            AppointmentStatus.Completed => "Consulta concluída",
            _ => "Consulta cancelada"
        };
    }

    private static string GetAppointmentHistoryTitle(AppointmentChangeType changeType)
    {
        return changeType switch
        {
            AppointmentChangeType.Cancelled => "Consulta cancelada",
            _ => "Consulta reagendada"
        };
    }

    private static string BuildAppointmentDescription(Appointment appointment, string doctorName)
    {
        return $"Consulta com {doctorName} em {appointment.AppointmentDate:dd/MM/yyyy}";
    }

    private static string BuildClinicalRecordDescription(ClinicalRecord record)
    {
        return string.IsNullOrWhiteSpace(record.Diagnosis)
            ? record.ChiefComplaint
            : $"{record.ChiefComplaint} - {record.Diagnosis}";
    }

    private static string FormatMinutes(int minutes)
    {
        var hours = minutes / 60;
        var remainder = minutes % 60;
        return $"{hours:00}:{remainder:00}";
    }
}
