using ClinicFlow.Domain.Appointments;
using ClinicFlow.Domain.Doctors;
using ClinicFlow.Domain.Exceptions;
using ClinicFlow.Domain.Patients;
using ClinicFlow.Domain.Primitives;

namespace ClinicFlow.Domain.ClinicalRecords;

public sealed class ClinicalRecord : AuditableEntity
{
    public const int MaxChiefComplaintLength = 500;
    public const int MaxDiagnosisLength = 1000;
    public const int MaxPrescriptionLength = 1000;
    public const int MaxNotesLength = 2000;

    public long AppointmentId { get; private set; }

    public Appointment? Appointment { get; private set; }

    public long PatientId { get; private set; }

    public Patient? Patient { get; private set; }

    public long DoctorId { get; private set; }

    public Doctor? Doctor { get; private set; }

    public string ChiefComplaint { get; private set; } = string.Empty;

    public string? Diagnosis { get; private set; }

    public string? Prescription { get; private set; }

    public string? Notes { get; private set; }

    private ClinicalRecord()
    {
    }

    public static ClinicalRecord Create(
        long appointmentId,
        long patientId,
        long doctorId,
        string chiefComplaint,
        string? diagnosis,
        string? prescription,
        string? notes,
        DateTimeOffset createdAt,
        string? createdBy)
    {
        Validate(appointmentId, patientId, doctorId, chiefComplaint, diagnosis, prescription, notes);

        var record = new ClinicalRecord
        {
            AppointmentId = appointmentId,
            PatientId = patientId,
            DoctorId = doctorId,
            ChiefComplaint = Normalize(chiefComplaint),
            Diagnosis = NormalizeOptional(diagnosis),
            Prescription = NormalizeOptional(prescription),
            Notes = NormalizeOptional(notes)
        };

        record.MarkCreated(createdAt, createdBy);
        return record;
    }

    public void Update(
        string chiefComplaint,
        string? diagnosis,
        string? prescription,
        string? notes,
        DateTimeOffset updatedAt,
        string? updatedBy)
    {
        Validate(AppointmentId, PatientId, DoctorId, chiefComplaint, diagnosis, prescription, notes);

        ChiefComplaint = Normalize(chiefComplaint);
        Diagnosis = NormalizeOptional(diagnosis);
        Prescription = NormalizeOptional(prescription);
        Notes = NormalizeOptional(notes);
        MarkUpdated(updatedAt, updatedBy);
    }

    public void Delete(DateTimeOffset deletedAt, string? deletedBy)
    {
        MarkDeleted(deletedAt, deletedBy);
    }

    private static void Validate(
        long appointmentId,
        long patientId,
        long doctorId,
        string chiefComplaint,
        string? diagnosis,
        string? prescription,
        string? notes)
    {
        var errors = new List<string>();

        if (appointmentId <= 0)
        {
            errors.Add("A consulta vinculada ao prontuário é obrigatória.");
        }

        if (patientId <= 0)
        {
            errors.Add("O paciente do prontuário é obrigatório.");
        }

        if (doctorId <= 0)
        {
            errors.Add("O médico do prontuário é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(chiefComplaint))
        {
            errors.Add("A queixa principal é obrigatória.");
        }
        else if (chiefComplaint.Trim().Length > MaxChiefComplaintLength)
        {
            errors.Add($"A queixa principal deve ter no máximo {MaxChiefComplaintLength} caracteres.");
        }

        if (diagnosis is not null && diagnosis.Trim().Length > MaxDiagnosisLength)
        {
            errors.Add($"O diagnóstico deve ter no máximo {MaxDiagnosisLength} caracteres.");
        }

        if (prescription is not null && prescription.Trim().Length > MaxPrescriptionLength)
        {
            errors.Add($"A prescrição deve ter no máximo {MaxPrescriptionLength} caracteres.");
        }

        if (notes is not null && notes.Trim().Length > MaxNotesLength)
        {
            errors.Add($"As observações devem ter no máximo {MaxNotesLength} caracteres.");
        }

        if (errors.Count > 0)
        {
            throw new DomainValidationException(errors);
        }
    }

    private static string Normalize(string value)
    {
        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
