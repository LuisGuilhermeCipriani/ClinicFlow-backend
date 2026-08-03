using ClinicFlow.Application.Doctors;
using ClinicFlow.Application.ClinicalRecords;
using ClinicFlow.Application.Appointments;
using ClinicFlow.Application.Authentication;
using ClinicFlow.Application.DoctorSchedules;
using ClinicFlow.Application.Patients;
using ClinicFlow.Application.Specialties;
using ClinicFlow.Application.Users;
using ClinicFlow.Infrastructure.Authentication;
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

        services.Configure<ClinicFlowAuthenticationOptions>(configuration.GetSection(ClinicFlowAuthenticationOptions.SectionName));

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

        services.AddScoped<IAuthenticationService, ClinicFlowAuthenticationService>();
        services.AddSingleton<IAuthenticationTokenService, ClinicFlowTokenService>();
        services.AddSingleton<IUserPasswordHasher, ClinicFlowPasswordHasher>();
        services.AddScoped<IAppointmentHistoryRepository, AppointmentHistoryRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IClinicalRecordRepository, ClinicalRecordRepository>();
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IDoctorScheduleRepository, DoctorScheduleRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<OracleDatabaseHealthCheck>();

        return services;
    }
}
