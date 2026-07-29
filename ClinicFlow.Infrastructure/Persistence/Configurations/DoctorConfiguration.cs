using ClinicFlow.Domain.Doctors;
using ClinicFlow.Domain.Specialties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Persistence.Configurations;

public sealed class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("DOCTORS");
        builder.HasKey(doctor => doctor.Id);

        builder.Property(doctor => doctor.Id)
            .ValueGeneratedOnAdd();

        builder.Property(doctor => doctor.Name)
            .HasMaxLength(Doctor.MaxNameLength)
            .IsRequired();

        builder.Property(doctor => doctor.CrmNumber)
            .HasMaxLength(Doctor.MaxCrmNumberLength)
            .IsRequired();

        builder.Property(doctor => doctor.CrmState)
            .HasMaxLength(Doctor.MaxCrmStateLength)
            .IsRequired();

        builder.Property(doctor => doctor.SpecialtyId)
            .IsRequired();

        builder.Property(doctor => doctor.Email)
            .HasMaxLength(Doctor.MaxEmailLength)
            .IsRequired();

        builder.Property(doctor => doctor.Phone)
            .HasMaxLength(Doctor.MaxPhoneLength)
            .IsRequired();

        builder.Property(doctor => doctor.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(doctor => doctor.CreatedAt)
            .IsRequired();

        builder.Property(doctor => doctor.CreatedBy)
            .HasMaxLength(128);

        builder.Property(doctor => doctor.UpdatedBy)
            .HasMaxLength(128);

        builder.Property(doctor => doctor.DeletedBy)
            .HasMaxLength(128);

        builder.HasOne(doctor => doctor.Specialty)
            .WithMany()
            .HasForeignKey(doctor => doctor.SpecialtyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(doctor => new { doctor.CrmState, doctor.CrmNumber })
            .IsUnique()
            .HasDatabaseName("UX_DOCTORS_CRM");
    }
}
