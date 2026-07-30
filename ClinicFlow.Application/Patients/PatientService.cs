using ClinicFlow.Domain.Patients;

namespace ClinicFlow.Application.Patients;

public sealed class PatientService(IPatientRepository repository) : IPatientService
{
    public async Task<PatientDetailsDto> CreateAsync(CreatePatientRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (await repository.ExistsByCpfAsync(request.Cpf, cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException("Já existe um paciente com esse CPF.");
        }

        var patient = Patient.Create(
            request.Name,
            request.Cpf,
            request.BirthDate,
            request.Gender,
            request.Email,
            request.Phone,
            DateTimeOffset.UtcNow,
            "system");

        await repository.AddAsync(patient, cancellationToken).ConfigureAwait(false);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return MapToDetailsDto(patient);
    }

    public async Task<PatientDetailsDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var patient = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return patient is null ? null : MapToDetailsDto(patient);
    }

    public async Task<PagedResult<PatientDto>> SearchAsync(PatientSearchRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var result = await repository.SearchAsync(
            request.SearchTerm,
            request.Status,
            request.Gender,
            page,
            pageSize,
            cancellationToken).ConfigureAwait(false);

        return new PagedResult<PatientDto>(
            result.Items.Select(MapToDto).ToArray(),
            result.Page,
            result.PageSize,
            result.TotalCount);
    }

    public async Task<PatientDetailsDto?> UpdateAsync(long id, UpdatePatientRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var patient = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (patient is null)
        {
            return null;
        }

        if (await repository.ExistsByCpfAsync(request.Cpf, id, cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException("Já existe um paciente com esse CPF.");
        }

        patient.Update(
            request.Name,
            request.Cpf,
            request.BirthDate,
            request.Gender,
            request.Email,
            request.Phone,
            DateTimeOffset.UtcNow,
            "system");

        if (request.Status == PatientStatus.Active)
        {
            patient.Activate(DateTimeOffset.UtcNow, "system");
        }
        else
        {
            patient.Deactivate(DateTimeOffset.UtcNow, "system");
        }

        repository.Update(patient);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return MapToDetailsDto(patient);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var patient = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (patient is null)
        {
            return false;
        }

        patient.Delete(DateTimeOffset.UtcNow, "system");
        repository.Remove(patient);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task<PatientDetailsDto?> SetStatusAsync(long id, bool isActive, CancellationToken cancellationToken = default)
    {
        var patient = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (patient is null)
        {
            return null;
        }

        if (isActive)
        {
            patient.Activate(DateTimeOffset.UtcNow, "system");
        }
        else
        {
            patient.Deactivate(DateTimeOffset.UtcNow, "system");
        }

        repository.Update(patient);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return MapToDetailsDto(patient);
    }

    private static PatientDto MapToDto(Patient patient)
    {
        return new PatientDto(
            patient.Id,
            patient.Name,
            patient.Cpf,
            patient.BirthDate,
            patient.Gender,
            patient.Email,
            patient.Phone,
            patient.Status,
            patient.IsDeleted,
            patient.CreatedAt,
            patient.UpdatedAt);
    }

    private static PatientDetailsDto MapToDetailsDto(Patient patient)
    {
        return new PatientDetailsDto(
            patient.Id,
            patient.Name,
            patient.Cpf,
            patient.BirthDate,
            patient.Gender,
            patient.Email,
            patient.Phone,
            patient.Status,
            patient.IsDeleted,
            patient.CreatedAt,
            patient.CreatedBy,
            patient.UpdatedAt,
            patient.UpdatedBy,
            patient.DeletedAt,
            patient.DeletedBy);
    }
}
