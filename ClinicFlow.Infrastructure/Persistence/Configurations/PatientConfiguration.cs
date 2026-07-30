using ClinicFlow.Domain.Patients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Persistence.Configurations;

public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("PATIENTS");
        builder.HasKey(patient => patient.Id);

        builder.Property(patient => patient.Id)
            .ValueGeneratedOnAdd();

        builder.Property(patient => patient.Name)
            .HasMaxLength(Patient.MaxNameLength)
            .IsRequired();

        builder.Property(patient => patient.Cpf)
            .HasMaxLength(Patient.MaxCpfLength)
            .IsRequired();

        builder.Property(patient => patient.BirthDate)
            .HasColumnType("DATE")
            .IsRequired();

        builder.Property(patient => patient.Gender)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(patient => patient.Email)
            .HasMaxLength(Patient.MaxEmailLength)
            .IsRequired();

        builder.Property(patient => patient.Phone)
            .HasMaxLength(Patient.MaxPhoneLength)
            .IsRequired();

        builder.Property(patient => patient.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(patient => patient.CreatedAt)
            .IsRequired();

        builder.Property(patient => patient.CreatedBy)
            .HasMaxLength(128);

        builder.Property(patient => patient.UpdatedBy)
            .HasMaxLength(128);

        builder.Property(patient => patient.DeletedBy)
            .HasMaxLength(128);

        builder.HasIndex(patient => patient.Cpf)
            .IsUnique()
            .HasDatabaseName("UX_PATIENTS_CPF");
    }
}
