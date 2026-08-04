namespace ClinicFlow.Application.Dashboard;

public sealed record DashboardSummaryDto(
    long TotalSpecialties,
    long ActiveSpecialties,
    long TotalDoctors,
    long ActiveDoctors,
    long TotalPatients,
    long ActivePatients,
    long TotalAppointments,
    long ScheduledAppointments,
    long CompletedAppointments,
    long CancelledAppointments,
    long AppointmentsToday,
    long AppointmentsNext7Days,
    long TotalClinicalRecords,
    DateTimeOffset GeneratedAt);
