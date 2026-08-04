using ClinicFlow.Application.Doctors;
using ClinicFlow.Application.ClinicalRecords;
using ClinicFlow.Application.Appointments;
using ClinicFlow.Application.Dashboard;
using ClinicFlow.Application.DoctorSchedules;
using ClinicFlow.Application.PatientHistory;
using ClinicFlow.Application.Patients;
using ClinicFlow.Application.Specialties;
using ClinicFlow.Application.Users;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IDoctorScheduleService, DoctorScheduleService>();
        services.AddScoped<IClinicalRecordService, ClinicalRecordService>();
        services.AddScoped<IPatientHistoryService, PatientHistoryService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<ISpecialtyService, SpecialtyService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
