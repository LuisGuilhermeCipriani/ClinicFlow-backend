using ClinicFlow.Application.Specialties;
using ClinicFlow.Domain.Specialties;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Infrastructure.Persistence.Repositories;

public sealed class SpecialtyRepository(ClinicFlowDbContext context) : ISpecialtyRepository
{
    public async Task<Specialty?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await context.Specialties
            .FirstOrDefaultAsync(specialty => specialty.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<PagedResult<Specialty>> SearchAsync(
        string? searchTerm,
        SpecialtyStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.Specialties.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalized = searchTerm.Trim().ToUpper();
            query = query.Where(specialty =>
                specialty.Name.ToUpper().Contains(normalized) ||
                (specialty.Description != null && specialty.Description.ToUpper().Contains(normalized)));
        }

        if (status is not null)
        {
            query = query.Where(specialty => specialty.Status == status);
        }

        var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query
            .OrderBy(specialty => specialty.Name)
            .ThenBy(specialty => specialty.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PagedResult<Specialty>(items, page, pageSize, totalCount);
    }

    public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalized = name.Trim().ToUpper();

        return await context.Specialties.AnyAsync(specialty =>
                specialty.Name.ToUpper() == normalized &&
                (!excludeId.HasValue || specialty.Id != excludeId.Value) &&
                !specialty.IsDeleted,
            cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task AddAsync(Specialty specialty, CancellationToken cancellationToken = default)
    {
        await context.Specialties.AddAsync(specialty, cancellationToken).ConfigureAwait(false);
    }

    public void Update(Specialty specialty)
    {
        context.Specialties.Update(specialty);
    }

    public void Remove(Specialty specialty)
    {
        context.Specialties.Update(specialty);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
