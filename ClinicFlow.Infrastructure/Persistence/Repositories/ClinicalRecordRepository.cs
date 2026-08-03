using ClinicFlow.Application.ClinicalRecords;
using ClinicFlow.Domain.ClinicalRecords;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Persistence.Repositories;

public sealed class ClinicalRecordRepository(ClinicFlowDbContext context) : IClinicalRecordRepository
{
    public async Task<ClinicalRecord?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await context.ClinicalRecords.AsNoTracking().FirstOrDefaultAsync(record => record.Id == id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ClinicalRecord?> GetByAppointmentIdAsync(long appointmentId, CancellationToken cancellationToken = default)
    {
        return await context.ClinicalRecords.AsNoTracking().FirstOrDefaultAsync(record => record.AppointmentId == appointmentId, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<ClinicalRecord>> GetByPatientIdAsync(long patientId, CancellationToken cancellationToken = default)
    {
        return await context.ClinicalRecords
            .AsNoTracking()
            .Where(record => record.PatientId == patientId && !record.IsDeleted)
            .OrderByDescending(record => record.CreatedAt)
            .ThenByDescending(record => record.Id)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<PagedResult<ClinicalRecord>> SearchAsync(
        long? appointmentId,
        long? patientId,
        long? doctorId,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.ClinicalRecords.AsNoTracking()
            .Where(record => !record.IsDeleted)
            .AsQueryable();

        if (appointmentId is not null)
        {
            query = query.Where(record => record.AppointmentId == appointmentId);
        }

        if (patientId is not null)
        {
            query = query.Where(record => record.PatientId == patientId);
        }

        if (doctorId is not null)
        {
            query = query.Where(record => record.DoctorId == doctorId);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalized = searchTerm.Trim();
            query = query.Where(record =>
                record.ChiefComplaint.Contains(normalized) ||
                (record.Diagnosis != null && record.Diagnosis.Contains(normalized)) ||
                (record.Prescription != null && record.Prescription.Contains(normalized)) ||
                (record.Notes != null && record.Notes.Contains(normalized)));
        }

        var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query
            .OrderByDescending(record => record.CreatedAt)
            .ThenByDescending(record => record.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PagedResult<ClinicalRecord>(items, page, pageSize, totalCount);
    }

    public async Task AddAsync(ClinicalRecord record, CancellationToken cancellationToken = default)
    {
        await context.ClinicalRecords.AddAsync(record, cancellationToken).ConfigureAwait(false);
    }

    public void Update(ClinicalRecord record)
    {
        context.ClinicalRecords.Update(record);
    }

    public void Remove(ClinicalRecord record)
    {
        context.ClinicalRecords.Update(record);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
