using ClinicFlow.Application.Doctors;
using ClinicFlow.Application.DoctorSchedules;
using ClinicFlow.Application.Patients;
using ClinicFlow.Application.Specialties;
using ClinicFlow.Infrastructure.Persistence;
using ClinicFlow.Infrastructure.Persistence.HealthChecks;
using ClinicFlow.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("OracleDatabase");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "User Id=CLINICFLOW_APP;Password=CHANGE_ME_LOCALLY;Data Source=localhost:1521/XEPDB1";
        }

        services.AddDbContext<ClinicFlowDbContext>(options =>
        {
            options.UseOracle(connectionString, oracleOptions =>
            {
                oracleOptions.MigrationsHistoryTable(ClinicFlowDbContext.MigrationsHistoryTable, ClinicFlowDbContext.DefaultSchema);
            });
        });

        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IDoctorScheduleRepository, DoctorScheduleRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
        services.AddScoped<OracleDatabaseHealthCheck>();

        return services;
    }
}
