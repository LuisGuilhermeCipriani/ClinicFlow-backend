using ClinicFlow.Application.Appointments;
using ClinicFlow.Application.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = ClinicFlowAuthorizationPolicies.ViewClinicData)]
public sealed class AppointmentsController(IAppointmentService appointmentService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AppointmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AppointmentDto>>> SearchAsync(
        [FromQuery] AppointmentSearchRequest request,
        CancellationToken cancellationToken)
    {
        var result = await appointmentService.SearchAsync(request, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(AppointmentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentDetailsDto>> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var appointment = await appointmentService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return appointment is null ? NotFound() : Ok(appointment);
    }

    [HttpPost]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(typeof(AppointmentDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AppointmentDetailsDto>> CreateAsync(
        [FromBody] CreateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var appointment = await appointmentService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(typeof(AppointmentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentDetailsDto>> UpdateAsync(
        long id,
        [FromBody] UpdateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var appointment = await appointmentService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return appointment is null ? NotFound() : Ok(appointment);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var deleted = await appointmentService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPatch("{id:long}/cancel")]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(typeof(AppointmentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentDetailsDto>> CancelAsync(
        long id,
        [FromBody] CancelAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var appointment = await appointmentService.CancelAsync(id, request, cancellationToken).ConfigureAwait(false);
        return appointment is null ? NotFound() : Ok(appointment);
    }

    [HttpPatch("{id:long}/reschedule")]
    [Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageClinicData)]
    [ProducesResponseType(typeof(AppointmentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentDetailsDto>> RescheduleAsync(
        long id,
        [FromBody] RescheduleAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var appointment = await appointmentService.RescheduleAsync(id, request, cancellationToken).ConfigureAwait(false);
        return appointment is null ? NotFound() : Ok(appointment);
    }

    [HttpGet("{id:long}/history")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AppointmentHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<AppointmentHistoryDto>>> GetHistoryAsync(long id, CancellationToken cancellationToken)
    {
        var history = await appointmentService.GetHistoryAsync(id, cancellationToken).ConfigureAwait(false);
        return history is null ? NotFound() : Ok(history);
    }
}
