using ClinicFlow.Application.Authentication;
using ClinicFlow.Application.PatientHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/patients/{patientId:long}/history")]
[Authorize(Policy = ClinicFlowAuthorizationPolicies.ViewClinicData)]
public sealed class PatientHistoryController(IPatientHistoryService patientHistoryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PatientHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientHistoryDto>> GetByPatientIdAsync(long patientId, CancellationToken cancellationToken)
    {
        var history = await patientHistoryService.GetByPatientIdAsync(patientId, cancellationToken).ConfigureAwait(false);
        return history is null ? NotFound() : Ok(history);
    }
}
