using ClinicFlow.Application.Authentication;
using ClinicFlow.Application.ClinicalRecords;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = ClinicFlowAuthorizationPolicies.ViewClinicalRecords)]
public sealed class ClinicalRecordsController(IClinicalRecordService clinicalRecordService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ClinicalRecordDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ClinicalRecordDto>>> SearchAsync(
        [FromQuery] ClinicalRecordSearchRequest request,
        CancellationToken cancellationToken)
    {
        var result = await clinicalRecordService.SearchAsync(request, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ClinicalRecordDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClinicalRecordDetailsDto>> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var record = await clinicalRecordService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return record is null ? NotFound() : Ok(record);
    }

    [HttpGet("appointment/{appointmentId:long}")]
    [ProducesResponseType(typeof(ClinicalRecordDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClinicalRecordDetailsDto>> GetByAppointmentIdAsync(long appointmentId, CancellationToken cancellationToken)
    {
        var record = await clinicalRecordService.GetByAppointmentIdAsync(appointmentId, cancellationToken).ConfigureAwait(false);
        return record is null ? NotFound() : Ok(record);
    }

    [HttpPost]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicalRecords)]
    [ProducesResponseType(typeof(ClinicalRecordDetailsDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ClinicalRecordDetailsDto>> CreateAsync([FromBody] CreateClinicalRecordRequest request, CancellationToken cancellationToken)
    {
        var record = await clinicalRecordService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = record.Id }, record);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicalRecords)]
    [ProducesResponseType(typeof(ClinicalRecordDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClinicalRecordDetailsDto>> UpdateAsync(long id, [FromBody] UpdateClinicalRecordRequest request, CancellationToken cancellationToken)
    {
        var record = await clinicalRecordService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return record is null ? NotFound() : Ok(record);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicalRecords)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var deleted = await clinicalRecordService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }
}
