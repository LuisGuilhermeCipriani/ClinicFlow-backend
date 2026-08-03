using ClinicFlow.Application.DoctorSchedules;
using ClinicFlow.Application.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = ClinicFlowAuthorizationPolicies.ViewClinicData)]
public sealed class DoctorSchedulesController(IDoctorScheduleService doctorScheduleService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<DoctorScheduleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<DoctorScheduleDto>>> SearchAsync(
        [FromQuery] DoctorScheduleSearchRequest request,
        CancellationToken cancellationToken)
    {
        var result = await doctorScheduleService.SearchAsync(request, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(DoctorScheduleDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoctorScheduleDetailsDto>> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var schedule = await doctorScheduleService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return schedule is null ? NotFound() : Ok(schedule);
    }

    [HttpPost]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(typeof(DoctorScheduleDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DoctorScheduleDetailsDto>> CreateAsync(
        [FromBody] CreateDoctorScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var schedule = await doctorScheduleService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = schedule.Id }, schedule);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(typeof(DoctorScheduleDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoctorScheduleDetailsDto>> UpdateAsync(
        long id,
        [FromBody] UpdateDoctorScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var schedule = await doctorScheduleService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return schedule is null ? NotFound() : Ok(schedule);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var deleted = await doctorScheduleService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPatch("{id:long}/activate")]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(typeof(DoctorScheduleDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoctorScheduleDetailsDto>> ActivateAsync(long id, CancellationToken cancellationToken)
    {
        var schedule = await doctorScheduleService.SetStatusAsync(id, true, cancellationToken).ConfigureAwait(false);
        return schedule is null ? NotFound() : Ok(schedule);
    }

    [HttpPatch("{id:long}/deactivate")]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(typeof(DoctorScheduleDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoctorScheduleDetailsDto>> DeactivateAsync(long id, CancellationToken cancellationToken)
    {
        var schedule = await doctorScheduleService.SetStatusAsync(id, false, cancellationToken).ConfigureAwait(false);
        return schedule is null ? NotFound() : Ok(schedule);
    }
}
