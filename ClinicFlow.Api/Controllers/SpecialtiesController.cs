using ClinicFlow.Application.Specialties;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SpecialtiesController(ISpecialtyService specialtyService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SpecialtyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<SpecialtyDto>>> SearchAsync(
        [FromQuery] SpecialtySearchRequest request,
        CancellationToken cancellationToken)
    {
        var result = await specialtyService.SearchAsync(request, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(SpecialtyDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SpecialtyDetailsDto>> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var specialty = await specialtyService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (specialty is null)
        {
            return NotFound();
        }

        return Ok(specialty);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SpecialtyDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SpecialtyDetailsDto>> CreateAsync(
        [FromBody] CreateSpecialtyRequest request,
        CancellationToken cancellationToken)
    {
        var specialty = await specialtyService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = specialty.Id }, specialty);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(SpecialtyDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SpecialtyDetailsDto>> UpdateAsync(
        long id,
        [FromBody] UpdateSpecialtyRequest request,
        CancellationToken cancellationToken)
    {
        var specialty = await specialtyService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        if (specialty is null)
        {
            return NotFound();
        }

        return Ok(specialty);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var deleted = await specialtyService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPatch("{id:long}/activate")]
    [ProducesResponseType(typeof(SpecialtyDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SpecialtyDetailsDto>> ActivateAsync(long id, CancellationToken cancellationToken)
    {
        var specialty = await specialtyService.SetStatusAsync(id, true, cancellationToken).ConfigureAwait(false);
        return specialty is null ? NotFound() : Ok(specialty);
    }

    [HttpPatch("{id:long}/deactivate")]
    [ProducesResponseType(typeof(SpecialtyDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SpecialtyDetailsDto>> DeactivateAsync(long id, CancellationToken cancellationToken)
    {
        var specialty = await specialtyService.SetStatusAsync(id, false, cancellationToken).ConfigureAwait(false);
        return specialty is null ? NotFound() : Ok(specialty);
    }
}
