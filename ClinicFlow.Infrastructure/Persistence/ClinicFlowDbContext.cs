using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Persistence;

public sealed class ClinicFlowDbContext(DbContextOptions<ClinicFlowDbContext> options) : DbContext(options)
{
    public const string DefaultSchema = "CLINICFLOW_APP";
    public const string MigrationsHistoryTable = "__EFMigrationsHistory";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasDefaultSchema(DefaultSchema);

        base.OnModelCreating(modelBuilder);
    }
}
