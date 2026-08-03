using ClinicFlow.Domain.ClinicalRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Persistence.Configurations;

public sealed class ClinicalRecordConfiguration : IEntityTypeConfiguration<ClinicalRecord>
{
    public void Configure(EntityTypeBuilder<ClinicalRecord> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("CLINICAL_RECORDS");
        builder.HasKey(record => record.Id);

        builder.Property(record => record.Id)
            .ValueGeneratedOnAdd();

        builder.Property(record => record.AppointmentId)
            .IsRequired();

        builder.Property(record => record.PatientId)
            .IsRequired();

        builder.Property(record => record.DoctorId)
            .IsRequired();

        builder.Property(record => record.ChiefComplaint)
            .IsRequired()
            .HasMaxLength(ClinicalRecord.MaxChiefComplaintLength);

        builder.Property(record => record.Diagnosis)
            .HasMaxLength(ClinicalRecord.MaxDiagnosisLength);

        builder.Property(record => record.Prescription)
            .HasMaxLength(ClinicalRecord.MaxPrescriptionLength);

        builder.Property(record => record.Notes)
            .HasMaxLength(ClinicalRecord.MaxNotesLength);

        builder.Property(record => record.CreatedAt)
            .IsRequired();

        builder.Property(record => record.CreatedBy)
            .HasMaxLength(128);

        builder.Property(record => record.UpdatedBy)
            .HasMaxLength(128);

        builder.Property(record => record.DeletedBy)
            .HasMaxLength(128);

        builder.HasOne(record => record.Appointment)
            .WithMany()
            .HasForeignKey(record => record.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(record => record.Patient)
            .WithMany()
            .HasForeignKey(record => record.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(record => record.Doctor)
            .WithMany()
            .HasForeignKey(record => record.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(record => record.AppointmentId)
            .IsUnique()
            .HasDatabaseName("UX_CLINICAL_RECORDS_APPOINTMENT");

        builder.HasIndex(record => record.PatientId)
            .HasDatabaseName("IX_CLINICAL_RECORDS_PATIENT");

        builder.HasIndex(record => record.DoctorId)
            .HasDatabaseName("IX_CLINICAL_RECORDS_DOCTOR");
    }
}
