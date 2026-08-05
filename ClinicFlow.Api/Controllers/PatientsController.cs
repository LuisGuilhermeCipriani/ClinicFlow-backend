using ClinicFlow.Application.Patients;
using ClinicFlow.Application.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = ClinicFlowAuthorizationPolicies.ViewClinicData)]
public sealed class PatientsController(IPatientService patientService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PatientDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<PatientDto>>> SearchAsync(
        [FromQuery] PatientSearchRequest request,
        CancellationToken cancellationToken)
    {
        var result = await patientService.SearchAsync(request, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpGet("{id:long}", Name = "Patients_GetById")]
    [ProducesResponseType(typeof(PatientDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDetailsDto>> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var patient = await patientService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return patient is null ? NotFound() : Ok(patient);
    }

    [HttpPost]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(typeof(PatientDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PatientDetailsDto>> CreateAsync(
        [FromBody] CreatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var patient = await patientService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtRoute("Patients_GetById", new { id = patient.Id }, patient);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(typeof(PatientDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDetailsDto>> UpdateAsync(
        long id,
        [FromBody] UpdatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var patient = await patientService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return patient is null ? NotFound() : Ok(patient);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var deleted = await patientService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPatch("{id:long}/activate")]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(typeof(PatientDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDetailsDto>> ActivateAsync(long id, CancellationToken cancellationToken)
    {
        var patient = await patientService.SetStatusAsync(id, true, cancellationToken).ConfigureAwait(false);
        return patient is null ? NotFound() : Ok(patient);
    }

    [HttpPatch("{id:long}/deactivate")]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(typeof(PatientDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDetailsDto>> DeactivateAsync(long id, CancellationToken cancellationToken)
    {
        var patient = await patientService.SetStatusAsync(id, false, cancellationToken).ConfigureAwait(false);
        return patient is null ? NotFound() : Ok(patient);
    }
}
