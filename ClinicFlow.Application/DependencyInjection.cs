using ClinicFlow.Application.Doctors;
using ClinicFlow.Application.Patients;
using ClinicFlow.Application.Specialties;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<ISpecialtyService, SpecialtyService>();

        return services;
    }
}
