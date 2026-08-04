using ClinicFlow.Application.Patients;
using ClinicFlow.Domain.Patients;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Persistence.Repositories;

public sealed class PatientRepository(ClinicFlowDbContext context) : IPatientRepository
{
    public async Task<Patient?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await context.Patients
            .FirstOrDefaultAsync(patient => patient.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<PagedResult<Patient>> SearchAsync(
        string? searchTerm,
        PatientStatus? status,
        PatientGender? gender,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.Patients
            .AsNoTracking()
            .Where(patient => !patient.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalized = searchTerm.Trim().ToUpperInvariant();
            var digits = new string(searchTerm.Where(char.IsDigit).ToArray());
            query = query.Where(patient =>
                patient.Name.ToUpper().Contains(normalized) ||
                (!string.IsNullOrWhiteSpace(digits) && patient.Cpf.Contains(digits)) ||
                patient.Email.ToUpper().Contains(normalized) ||
                patient.Phone.ToUpper().Contains(normalized));
        }

        if (status is not null)
        {
            query = query.Where(patient => patient.Status == status);
        }

        if (gender is not null)
        {
            query = query.Where(patient => patient.Gender == gender);
        }

        var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query
            .OrderBy(patient => patient.Name)
            .ThenBy(patient => patient.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PagedResult<Patient>(items, page, pageSize, totalCount);
    }

    public async Task<bool> ExistsByCpfAsync(string cpf, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeCpf(cpf);

        return await context.Patients.AnyAsync(patient =>
                patient.Cpf == normalized &&
                (!excludeId.HasValue || patient.Id != excludeId.Value) &&
                !patient.IsDeleted,
            cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task AddAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        await context.Patients.AddAsync(patient, cancellationToken).ConfigureAwait(false);
    }

    public void Update(Patient patient)
    {
        context.Patients.Update(patient);
    }

    public void Remove(Patient patient)
    {
        context.Patients.Update(patient);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }

    private static string NormalizeCpf(string cpf)
    {
        return new string(cpf.Where(char.IsDigit).ToArray());
    }
}
