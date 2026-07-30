using ClinicFlow.Domain.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Persistence.Configurations;

public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("APPOINTMENTS");
        builder.HasKey(appointment => appointment.Id);

        builder.Property(appointment => appointment.Id)
            .ValueGeneratedOnAdd();

        builder.Property(appointment => appointment.DoctorId)
            .IsRequired();

        builder.Property(appointment => appointment.PatientId)
            .IsRequired();

        builder.Property(appointment => appointment.AppointmentDate)
            .HasColumnType("DATE")
            .IsRequired();

        builder.Property(appointment => appointment.StartMinute)
            .IsRequired();

        builder.Property(appointment => appointment.EndMinute)
            .IsRequired();

        builder.Property(appointment => appointment.DurationMinutes)
            .IsRequired();

        builder.Property(appointment => appointment.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(appointment => appointment.CreatedAt)
            .IsRequired();

        builder.Property(appointment => appointment.CreatedBy)
            .HasMaxLength(128);

        builder.Property(appointment => appointment.UpdatedBy)
            .HasMaxLength(128);

        builder.Property(appointment => appointment.DeletedBy)
            .HasMaxLength(128);

        builder.HasOne(appointment => appointment.Doctor)
            .WithMany()
            .HasForeignKey(appointment => appointment.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(appointment => appointment.Patient)
            .WithMany()
            .HasForeignKey(appointment => appointment.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(appointment => appointment.DoctorId)
            .HasDatabaseName("IX_APPOINTMENTS_DOCTOR");

        builder.HasIndex(appointment => appointment.PatientId)
            .HasDatabaseName("IX_APPOINTMENTS_PATIENT");

        builder.HasIndex(appointment => new { appointment.DoctorId, appointment.AppointmentDate, appointment.StartMinute, appointment.EndMinute })
            .IsUnique()
            .HasDatabaseName("UX_APPOINTMENTS_SLOT");
    }
}
