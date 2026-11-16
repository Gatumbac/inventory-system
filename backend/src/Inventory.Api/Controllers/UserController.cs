using Inventory.Core.Abstractions;
using Inventory.Core.Constants;
using Inventory.Core.Dtos;
using Inventory.Core.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

[ApiController]
[Authorize(Roles = RoleNames.Admin)]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<UserResponse>>> GetUsers([FromQuery] UserFilterRequest filterRequest)
    {
        var response = await _userService.GetUsersAsync(filterRequest);
        return Ok(response);
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> CreateUser(UserRequest userRequest)
    {
        var result = await _userService.CreateUserAsync(userRequest);
        if (result.Status == SaveUserStatus.DuplicateEmail)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: ErrorMessages.UserEmailDuplicated);
        }

        return StatusCode(StatusCodes.Status201Created, result.User);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RemoveUser(int id)
    {
        var currentUserId = int.Parse(User.FindFirst("sub")!.Value);
        var response = await _userService.RemoveUserAsync(id, currentUserId);
        if (response == DeleteUserResult.NotFound)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: ErrorMessages.UserNotFound);
        }

        if (response == DeleteUserResult.Self)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: ErrorMessages.UserCannotDeactivateSelf);
        }

        if (response == DeleteUserResult.LastAdmin)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: ErrorMessages.UserLastAdmin);
        }

        return NoContent();
    }

    [HttpPost("{id}/reactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> ReactivateUser(int id)
    {
        var response = await _userService.ReactivateUserAsync(id);
        if (response == ReactivateUserResult.NotFound)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: ErrorMessages.UserNotFound);
        }

        if (response == ReactivateUserResult.DuplicateEmail)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: ErrorMessages.UserEmailDuplicated);
        }

        return NoContent();
    }
}
