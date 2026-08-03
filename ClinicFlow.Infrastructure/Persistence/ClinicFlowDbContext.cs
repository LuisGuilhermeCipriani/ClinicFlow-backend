using ClinicFlow.Domain.Appointments;
using ClinicFlow.Domain.ClinicalRecords;
using ClinicFlow.Domain.Doctors;
using ClinicFlow.Domain.DoctorSchedules;
using ClinicFlow.Domain.Patients;
using ClinicFlow.Domain.Specialties;
using ClinicFlow.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Persistence;

public sealed class ClinicFlowDbContext(DbContextOptions<ClinicFlowDbContext> options) : DbContext(options)
{
    public const string DefaultSchema = "CLINICFLOW_APP";
    public const string MigrationsHistoryTable = "__EFMigrationsHistory";

    public DbSet<Doctor> Doctors => Set<Doctor>();

    public DbSet<Appointment> Appointments => Set<Appointment>();

    public DbSet<AppointmentHistory> AppointmentHistories => Set<AppointmentHistory>();

    public DbSet<ClinicalRecord> ClinicalRecords => Set<ClinicalRecord>();

    public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();

    public DbSet<Patient> Patients => Set<Patient>();

    public DbSet<Specialty> Specialties => Set<Specialty>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasDefaultSchema(DefaultSchema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClinicFlowDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
