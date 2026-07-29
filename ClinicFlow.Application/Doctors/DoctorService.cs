using ClinicFlow.Application.Specialties;
using ClinicFlow.Domain.Doctors;

namespace ClinicFlow.Application.Doctors;

public sealed class DoctorService(
    IDoctorRepository doctorRepository,
    ISpecialtyRepository specialtyRepository) : IDoctorService
{
    public async Task<DoctorDetailsDto> CreateAsync(CreateDoctorRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var specialty = await specialtyRepository.GetByIdAsync(request.SpecialtyId, cancellationToken).ConfigureAwait(false);
        if (specialty is null || specialty.IsDeleted)
        {
            throw new InvalidOperationException("A especialidade informada não existe.");
        }

        if (await doctorRepository.ExistsByCrmAsync(request.CrmNumber, request.CrmState, cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException("Já existe um médico com esse CRM.");
        }

        var doctor = Doctor.Create(
            request.Name,
            request.CrmNumber,
            request.CrmState,
            request.SpecialtyId,
            request.Email,
            request.Phone,
            DateTimeOffset.UtcNow,
            "system");

        await doctorRepository.AddAsync(doctor, cancellationToken).ConfigureAwait(false);
        await doctorRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return MapToDetailsDto(doctor, specialty.Name);
    }

    public async Task<DoctorDetailsDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var doctor = await doctorRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return doctor is null ? null : MapToDetailsDto(doctor, doctor.Specialty?.Name ?? string.Empty);
    }

    public async Task<PagedResult<DoctorDto>> SearchAsync(DoctorSearchRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var result = await doctorRepository.SearchAsync(
            request.SearchTerm,
            request.Status,
            request.SpecialtyId,
            page,
            pageSize,
            cancellationToken).ConfigureAwait(false);

        return new PagedResult<DoctorDto>(
            result.Items.Select(doctor => MapToDto(doctor, doctor.Specialty?.Name ?? string.Empty)).ToArray(),
            result.Page,
            result.PageSize,
            result.TotalCount);
    }

    public async Task<DoctorDetailsDto?> UpdateAsync(long id, UpdateDoctorRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var doctor = await doctorRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (doctor is null)
        {
            return null;
        }

        var specialty = await specialtyRepository.GetByIdAsync(request.SpecialtyId, cancellationToken).ConfigureAwait(false);
        if (specialty is null || specialty.IsDeleted)
        {
            throw new InvalidOperationException("A especialidade informada não existe.");
        }

        if (await doctorRepository.ExistsByCrmAsync(request.CrmNumber, request.CrmState, id, cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException("Já existe um médico com esse CRM.");
        }

        doctor.Update(
            request.Name,
            request.CrmNumber,
            request.CrmState,
            request.SpecialtyId,
            request.Email,
            request.Phone,
            DateTimeOffset.UtcNow,
            "system");

        if (request.Status == DoctorStatus.Active)
        {
            doctor.Activate(DateTimeOffset.UtcNow, "system");
        }
        else
        {
            doctor.Deactivate(DateTimeOffset.UtcNow, "system");
        }

        doctorRepository.Update(doctor);
        await doctorRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return MapToDetailsDto(doctor, specialty.Name);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var doctor = await doctorRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (doctor is null)
        {
            return false;
        }

        doctor.Delete(DateTimeOffset.UtcNow, "system");
        doctorRepository.Remove(doctor);
        await doctorRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task<DoctorDetailsDto?> SetStatusAsync(long id, bool isActive, CancellationToken cancellationToken = default)
    {
        var doctor = await doctorRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (doctor is null)
        {
            return null;
        }

        if (isActive)
        {
            doctor.Activate(DateTimeOffset.UtcNow, "system");
        }
        else
        {
            doctor.Deactivate(DateTimeOffset.UtcNow, "system");
        }

        doctorRepository.Update(doctor);
        await doctorRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return MapToDetailsDto(doctor, doctor.Specialty?.Name ?? string.Empty);
    }

    private static DoctorDto MapToDto(Doctor doctor, string specialtyName)
    {
        return new DoctorDto(
            doctor.Id,
            doctor.Name,
            doctor.CrmNumber,
            doctor.CrmState,
            doctor.SpecialtyId,
            specialtyName,
            doctor.Email,
            doctor.Phone,
            doctor.Status,
            doctor.IsDeleted,
            doctor.CreatedAt,
            doctor.UpdatedAt);
    }

    private static DoctorDetailsDto MapToDetailsDto(Doctor doctor, string specialtyName)
    {
        return new DoctorDetailsDto(
            doctor.Id,
            doctor.Name,
            doctor.CrmNumber,
            doctor.CrmState,
            doctor.SpecialtyId,
            specialtyName,
            doctor.Email,
            doctor.Phone,
            doctor.Status,
            doctor.IsDeleted,
            doctor.CreatedAt,
            doctor.CreatedBy,
            doctor.UpdatedAt,
            doctor.UpdatedBy,
            doctor.DeletedAt,
            doctor.DeletedBy);
    }
}
