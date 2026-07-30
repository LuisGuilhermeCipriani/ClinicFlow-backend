using ClinicFlow.Domain.DoctorSchedules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Persistence.Configurations;

public sealed class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule>
{
    public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("DOCTOR_SCHEDULES");
        builder.HasKey(schedule => schedule.Id);

        builder.Property(schedule => schedule.Id)
            .ValueGeneratedOnAdd();

        builder.Property(schedule => schedule.DoctorId)
            .IsRequired();

        builder.Property(schedule => schedule.DayOfWeek)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(schedule => schedule.StartMinute)
            .IsRequired();

        builder.Property(schedule => schedule.EndMinute)
            .IsRequired();

        builder.Property(schedule => schedule.SlotDurationMinutes)
            .IsRequired();

        builder.Property(schedule => schedule.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(schedule => schedule.CreatedAt)
            .IsRequired();

        builder.Property(schedule => schedule.CreatedBy)
            .HasMaxLength(128);

        builder.Property(schedule => schedule.UpdatedBy)
            .HasMaxLength(128);

        builder.Property(schedule => schedule.DeletedBy)
            .HasMaxLength(128);

        builder.HasOne(schedule => schedule.Doctor)
            .WithMany()
            .HasForeignKey(schedule => schedule.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(schedule => new { schedule.DoctorId, schedule.DayOfWeek, schedule.StartMinute, schedule.EndMinute })
            .IsUnique()
            .HasDatabaseName("UX_DOCTOR_SCHEDULES_SLOT");
    }
}
