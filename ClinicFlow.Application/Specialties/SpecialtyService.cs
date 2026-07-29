using ClinicFlow.Domain.Specialties;

namespace ClinicFlow.Application.Specialties;

public sealed class SpecialtyService(ISpecialtyRepository repository) : ISpecialtyService
{
    public async Task<SpecialtyDetailsDto> CreateAsync(CreateSpecialtyRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (await repository.ExistsByNameAsync(request.Name, cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException("Já existe uma especialidade com esse nome.");
        }

        var specialty = Specialty.Create(request.Name, request.Description, DateTimeOffset.UtcNow, "system");
        await repository.AddAsync(specialty, cancellationToken).ConfigureAwait(false);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return MapToDetailsDto(specialty);
    }

    public async Task<SpecialtyDetailsDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var specialty = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return specialty is null ? null : MapToDetailsDto(specialty);
    }

    public async Task<PagedResult<SpecialtyDto>> SearchAsync(SpecialtySearchRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var result = await repository.SearchAsync(request.SearchTerm, request.Status, page, pageSize, cancellationToken).ConfigureAwait(false);

        return new PagedResult<SpecialtyDto>(
            result.Items.Select(MapToDto).ToArray(),
            result.Page,
            result.PageSize,
            result.TotalCount);
    }

    public async Task<SpecialtyDetailsDto?> UpdateAsync(long id, UpdateSpecialtyRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var specialty = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (specialty is null)
        {
            return null;
        }

        if (await repository.ExistsByNameAsync(request.Name, id, cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException("Já existe uma especialidade com esse nome.");
        }

        specialty.Update(request.Name, request.Description, DateTimeOffset.UtcNow, "system");

        if (request.Status == SpecialtyStatus.Active)
        {
            specialty.Activate(DateTimeOffset.UtcNow, "system");
        }
        else
        {
            specialty.Deactivate(DateTimeOffset.UtcNow, "system");
        }

        repository.Update(specialty);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return MapToDetailsDto(specialty);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var specialty = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (specialty is null)
        {
            return false;
        }

        specialty.Delete(DateTimeOffset.UtcNow, "system");
        repository.Remove(specialty);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task<SpecialtyDetailsDto?> SetStatusAsync(long id, bool isActive, CancellationToken cancellationToken = default)
    {
        var specialty = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (specialty is null)
        {
            return null;
        }

        if (isActive)
        {
            specialty.Activate(DateTimeOffset.UtcNow, "system");
        }
        else
        {
            specialty.Deactivate(DateTimeOffset.UtcNow, "system");
        }

        repository.Update(specialty);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return MapToDetailsDto(specialty);
    }

    private static SpecialtyDto MapToDto(Specialty specialty)
    {
        return new SpecialtyDto(
            specialty.Id,
            specialty.Name,
            specialty.Description,
            specialty.Status,
            specialty.IsDeleted,
            specialty.CreatedAt,
            specialty.UpdatedAt);
    }

    private static SpecialtyDetailsDto MapToDetailsDto(Specialty specialty)
    {
        return new SpecialtyDetailsDto(
            specialty.Id,
            specialty.Name,
            specialty.Description,
            specialty.Status,
            specialty.IsDeleted,
            specialty.CreatedAt,
            specialty.CreatedBy,
            specialty.UpdatedAt,
            specialty.UpdatedBy,
            specialty.DeletedAt,
            specialty.DeletedBy);
    }
}
