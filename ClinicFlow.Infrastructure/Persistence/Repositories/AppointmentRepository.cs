using ClinicFlow.Application.Appointments;
using ClinicFlow.Domain.Appointments;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Persistence.Repositories;

public sealed class AppointmentRepository(ClinicFlowDbContext context) : IAppointmentRepository
{
    public async Task<Appointment?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await context.Appointments
            .Include(appointment => appointment.Doctor)
            .Include(appointment => appointment.Patient)
            .FirstOrDefaultAsync(appointment => appointment.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<PagedResult<Appointment>> SearchAsync(
        long? doctorId,
        long? patientId,
        DateTime? appointmentDate,
        AppointmentStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.Appointments
            .Include(appointment => appointment.Doctor)
            .Include(appointment => appointment.Patient)
            .AsNoTracking()
            .AsQueryable();

        if (doctorId is not null)
        {
            query = query.Where(appointment => appointment.DoctorId == doctorId);
        }

        if (patientId is not null)
        {
            query = query.Where(appointment => appointment.PatientId == patientId);
        }

        if (appointmentDate is not null)
        {
            var date = appointmentDate.Value.Date;
            query = query.Where(appointment => appointment.AppointmentDate == date);
        }

        if (status is not null)
        {
            query = query.Where(appointment => appointment.Status == status);
        }

        var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query
            .OrderBy(appointment => appointment.AppointmentDate)
            .ThenBy(appointment => appointment.StartMinute)
            .ThenBy(appointment => appointment.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PagedResult<Appointment>(items, page, pageSize, totalCount);
    }

    public async Task<bool> HasDoctorConflictAsync(long doctorId, DateTime appointmentDate, int startMinute, int endMinute, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var date = appointmentDate.Date;

        return await context.Appointments.AnyAsync(appointment =>
                appointment.DoctorId == doctorId &&
                appointment.AppointmentDate == date &&
                appointment.Status == AppointmentStatus.Scheduled &&
                !appointment.IsDeleted &&
                appointment.StartMinute < endMinute &&
                appointment.EndMinute > startMinute &&
                (!excludeId.HasValue || appointment.Id != excludeId.Value),
            cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> HasPatientConflictAsync(long patientId, DateTime appointmentDate, int startMinute, int endMinute, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var date = appointmentDate.Date;

        return await context.Appointments.AnyAsync(appointment =>
                appointment.PatientId == patientId &&
                appointment.AppointmentDate == date &&
                appointment.Status == AppointmentStatus.Scheduled &&
                !appointment.IsDeleted &&
                appointment.StartMinute < endMinute &&
                appointment.EndMinute > startMinute &&
                (!excludeId.HasValue || appointment.Id != excludeId.Value),
            cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        await context.Appointments.AddAsync(appointment, cancellationToken).ConfigureAwait(false);
    }

    public void Update(Appointment appointment)
    {
        context.Appointments.Update(appointment);
    }

    public void Remove(Appointment appointment)
    {
        context.Appointments.Update(appointment);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
