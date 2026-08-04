using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ClinicFlow.Infrastructure.Persistence;

public sealed class ClinicFlowDbContextFactory : IDesignTimeDbContextFactory<ClinicFlowDbContext>
{
    public ClinicFlowDbContext CreateDbContext(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("OracleDatabase");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "User Id=CLINICFLOW_APP;Password=CHANGE_ME_LOCALLY_123;Data Source=host.docker.internal:1521/XEPDB1";
        }

        var optionsBuilder = new DbContextOptionsBuilder<ClinicFlowDbContext>();
        optionsBuilder.UseOracle(connectionString, oracleOptions =>
        {
            oracleOptions.MigrationsHistoryTable(ClinicFlowDbContext.MigrationsHistoryTable, ClinicFlowDbContext.DefaultSchema);
        });

        return new ClinicFlowDbContext(optionsBuilder.Options);
    }
}
