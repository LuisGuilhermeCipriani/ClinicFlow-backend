using ClinicFlow.Application.Doctors;
using ClinicFlow.Domain.Doctors;
using ClinicFlow.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Persistence.Repositories;

public sealed class DoctorRepository(ClinicFlowDbContext context) : IDoctorRepository
{
    public async Task<Doctor?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await context.Doctors
            .Include(doctor => doctor.Specialty)
            .FirstOrDefaultAsync(doctor => doctor.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<PagedResult<Doctor>> SearchAsync(
        string? searchTerm,
        DoctorStatus? status,
        long? specialtyId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.Doctors
            .Include(doctor => doctor.Specialty)
            .AsNoTracking()
            .Where(doctor => !doctor.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalized = searchTerm.Trim().ToUpper();
            query = query.Where(doctor =>
                doctor.Name.ToUpper().Contains(normalized) ||
                doctor.CrmNumber.ToUpper().Contains(normalized) ||
                doctor.Email.ToUpper().Contains(normalized) ||
                doctor.Phone.ToUpper().Contains(normalized) ||
                (doctor.Specialty != null && doctor.Specialty.Name.ToUpper().Contains(normalized)));
        }

        if (status is not null)
        {
            query = query.Where(doctor => doctor.Status == status);
        }

        if (specialtyId is not null)
        {
            query = query.Where(doctor => doctor.SpecialtyId == specialtyId);
        }

        var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query
            .OrderBy(doctor => doctor.Name)
            .ThenBy(doctor => doctor.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PagedResult<Doctor>(items, page, pageSize, totalCount);
    }

    public async Task<bool> ExistsByCrmAsync(string crmNumber, string crmState, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalizedNumber = crmNumber.Trim().ToUpper();
        var normalizedState = crmState.Trim().ToUpper();
        var query = context.Doctors.Where(doctor =>
            doctor.CrmNumber.ToUpper() == normalizedNumber &&
            doctor.CrmState.ToUpper() == normalizedState &&
            EF.Property<int>(doctor, nameof(AuditableEntity.IsDeleted)) == 0);

        if (excludeId.HasValue)
        {
            var value = excludeId.Value;
            query = query.Where(doctor => doctor.Id != value);
        }

        return await query.CountAsync(cancellationToken).ConfigureAwait(false) > 0;
    }

    public async Task AddAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        await context.Doctors.AddAsync(doctor, cancellationToken).ConfigureAwait(false);
    }

    public void Update(Doctor doctor)
    {
        context.Doctors.Update(doctor);
    }

    public void Remove(Doctor doctor)
    {
        context.Doctors.Update(doctor);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
