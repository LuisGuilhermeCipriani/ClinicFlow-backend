using ClinicFlow.Application.Dashboard;
using ClinicFlow.Domain.Appointments;
using ClinicFlow.Domain.Doctors;
using ClinicFlow.Domain.Patients;
using ClinicFlow.Domain.Specialties;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Persistence.Repositories;

public sealed class DashboardRepository(ClinicFlowDbContext context) : IDashboardRepository
{
    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var today = now.UtcDateTime.Date;
        var next7Days = today.AddDays(7);

        var totalSpecialties = await context.Specialties.CountAsync(cancellationToken).ConfigureAwait(false);
        var activeSpecialties = await context.Specialties.CountAsync(specialty => !specialty.IsDeleted && specialty.Status == SpecialtyStatus.Active, cancellationToken).ConfigureAwait(false);

        var totalDoctors = await context.Doctors.CountAsync(cancellationToken).ConfigureAwait(false);
        var activeDoctors = await context.Doctors.CountAsync(doctor => !doctor.IsDeleted && doctor.Status == DoctorStatus.Active, cancellationToken).ConfigureAwait(false);

        var totalPatients = await context.Patients.CountAsync(cancellationToken).ConfigureAwait(false);
        var activePatients = await context.Patients.CountAsync(patient => !patient.IsDeleted && patient.Status == PatientStatus.Active, cancellationToken).ConfigureAwait(false);

        var totalAppointments = await context.Appointments.CountAsync(appointment => !appointment.IsDeleted, cancellationToken).ConfigureAwait(false);
        var scheduledAppointments = await context.Appointments.CountAsync(appointment => !appointment.IsDeleted && appointment.Status == AppointmentStatus.Scheduled, cancellationToken).ConfigureAwait(false);
        var completedAppointments = await context.Appointments.CountAsync(appointment => !appointment.IsDeleted && appointment.Status == AppointmentStatus.Completed, cancellationToken).ConfigureAwait(false);
        var cancelledAppointments = await context.Appointments.CountAsync(appointment => !appointment.IsDeleted && appointment.Status == AppointmentStatus.Cancelled, cancellationToken).ConfigureAwait(false);
        var appointmentsToday = await context.Appointments.CountAsync(appointment => !appointment.IsDeleted && appointment.AppointmentDate == today, cancellationToken).ConfigureAwait(false);
        var appointmentsNext7Days = await context.Appointments.CountAsync(appointment => !appointment.IsDeleted && appointment.AppointmentDate > today && appointment.AppointmentDate <= next7Days, cancellationToken).ConfigureAwait(false);

        var totalClinicalRecords = await context.ClinicalRecords.CountAsync(record => !record.IsDeleted, cancellationToken).ConfigureAwait(false);

        return new DashboardSummaryDto(
            totalSpecialties,
            activeSpecialties,
            totalDoctors,
            activeDoctors,
            totalPatients,
            activePatients,
            totalAppointments,
            scheduledAppointments,
            completedAppointments,
            cancelledAppointments,
            appointmentsToday,
            appointmentsNext7Days,
            totalClinicalRecords,
            now);
    }
}
