using ClinicFlow.Application.Doctors;
using ClinicFlow.Application.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = ClinicFlowAuthorizationPolicies.ViewClinicData)]
public sealed class DoctorsController(IDoctorService doctorService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<DoctorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<DoctorDto>>> SearchAsync(
        [FromQuery] DoctorSearchRequest request,
        CancellationToken cancellationToken)
    {
        var result = await doctorService.SearchAsync(request, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpGet("{id:long}", Name = "Doctors_GetById")]
    [ProducesResponseType(typeof(DoctorDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoctorDetailsDto>> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var doctor = await doctorService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return doctor is null ? NotFound() : Ok(doctor);
    }

    [HttpPost]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(typeof(DoctorDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DoctorDetailsDto>> CreateAsync(
        [FromBody] CreateDoctorRequest request,
        CancellationToken cancellationToken)
    {
        var doctor = await doctorService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtRoute("Doctors_GetById", new { id = doctor.Id }, doctor);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(typeof(DoctorDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoctorDetailsDto>> UpdateAsync(
        long id,
        [FromBody] UpdateDoctorRequest request,
        CancellationToken cancellationToken)
    {
        var doctor = await doctorService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return doctor is null ? NotFound() : Ok(doctor);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var deleted = await doctorService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPatch("{id:long}/activate")]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(typeof(DoctorDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoctorDetailsDto>> ActivateAsync(long id, CancellationToken cancellationToken)
    {
        var doctor = await doctorService.SetStatusAsync(id, true, cancellationToken).ConfigureAwait(false);
        return doctor is null ? NotFound() : Ok(doctor);
    }

    [HttpPatch("{id:long}/deactivate")]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(typeof(DoctorDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoctorDetailsDto>> DeactivateAsync(long id, CancellationToken cancellationToken)
    {
        var doctor = await doctorService.SetStatusAsync(id, false, cancellationToken).ConfigureAwait(false);
        return doctor is null ? NotFound() : Ok(doctor);
    }
}
