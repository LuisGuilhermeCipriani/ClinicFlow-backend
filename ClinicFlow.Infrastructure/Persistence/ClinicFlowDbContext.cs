using ClinicFlow.Domain.Specialties;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Persistence;

public sealed class ClinicFlowDbContext(DbContextOptions<ClinicFlowDbContext> options) : DbContext(options)
{
    public const string DefaultSchema = "CLINICFLOW_APP";
    public const string MigrationsHistoryTable = "__EFMigrationsHistory";

    public DbSet<Specialty> Specialties => Set<Specialty>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasDefaultSchema(DefaultSchema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClinicFlowDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
