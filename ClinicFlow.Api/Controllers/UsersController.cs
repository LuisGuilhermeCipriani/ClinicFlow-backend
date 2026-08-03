using ClinicFlow.Application.Authentication;
using ClinicFlow.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = ClinicFlowAuthorizationPolicies.ManageUsers)]
public sealed class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserDto>>> SearchAsync([FromQuery] UserSearchRequest request, CancellationToken cancellationToken)
    {
        var result = await userService.SearchAsync(request, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDetailsDto>> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var user = await userService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<UserDetailsDto>> CreateAsync([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await userService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = user.Id }, user);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDetailsDto>> UpdateAsync(long id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await userService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPatch("{id:long}/activate")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDetailsDto>> ActivateAsync(long id, CancellationToken cancellationToken)
    {
        var user = await userService.SetStatusAsync(id, true, cancellationToken).ConfigureAwait(false);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPatch("{id:long}/deactivate")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDetailsDto>> DeactivateAsync(long id, CancellationToken cancellationToken)
    {
        var user = await userService.SetStatusAsync(id, false, cancellationToken).ConfigureAwait(false);
        return user is null ? NotFound() : Ok(user);
    }
}
