using ClinicFlow.Domain.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Persistence.Configurations;

public sealed class AppointmentHistoryConfiguration : IEntityTypeConfiguration<AppointmentHistory>
{
    public void Configure(EntityTypeBuilder<AppointmentHistory> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("APPOINTMENT_HISTORY");
        builder.HasKey(history => history.Id);

        builder.Property(history => history.Id)
            .ValueGeneratedOnAdd();

        builder.Property(history => history.AppointmentId)
            .IsRequired();

        builder.Property(history => history.ChangeType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(history => history.PreviousAppointmentDate)
            .HasColumnType("DATE")
            .IsRequired();

        builder.Property(history => history.PreviousStartMinute)
            .IsRequired();

        builder.Property(history => history.PreviousEndMinute)
            .IsRequired();

        builder.Property(history => history.NewAppointmentDate)
            .HasColumnType("DATE");

        builder.Property(history => history.NewStartMinute);

        builder.Property(history => history.NewEndMinute);

        builder.Property(history => history.Reason)
            .HasMaxLength(AppointmentHistory.MaxReasonLength);

        builder.Property(history => history.CreatedAt)
            .IsRequired();

        builder.Property(history => history.CreatedBy)
            .HasMaxLength(128);

        builder.Property(history => history.UpdatedBy)
            .HasMaxLength(128);

        builder.Property(history => history.DeletedBy)
            .HasMaxLength(128);

        builder.HasOne(history => history.Appointment)
            .WithMany()
            .HasForeignKey(history => history.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(history => history.AppointmentId)
            .HasDatabaseName("IX_APPOINTMENT_HISTORY_APPOINTMENT");

        builder.HasIndex(history => history.CreatedAt)
            .HasDatabaseName("IX_APPOINTMENT_HISTORY_CREATED_AT");
    }
}
