using MaisonBean.Application.Interfaces;
using MaisonBean.Application.User.Commands;
using MaisonBean.Application.User.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMediator _mediator;

    public UserController(IUserService userService, IMediator mediator)
    {
        _userService = userService;
        _mediator = mediator;
    }

    // GET api/User/me
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var profile = await _userService.GetProfileAsync(userId, ct);
        if (profile == null) return NotFound();
        return Ok(profile);
    }
    //change password
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
    {
        command.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var success = await _mediator.Send(command);

        if (!success)
            return BadRequest(new { message = "Current password is incorrect" });

        return Ok(new { message = "Password updated successfully" });
    }

    // POST api/User/forgot-password
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command)
    {
        var success = await _mediator.Send(command);

        if (!success)
            return NotFound(new { message = "No account found with that email" });

        return Ok(new { message = "Password updated successfully" });
    }

    //Get all user
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers(CancellationToken ct)
    {
        var users = await _mediator.Send(new GetAllUsersQuery());

        return Ok(new
        {
            count = users.Count,
            data = users
        });
    }

    //toggle- block user
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> ToggleUser(int id)
    {
        var isBlocked = await _mediator.Send(new ToggleUserCommand(id));

        return Ok(new
        {
            message = isBlocked
                ? "User successfully blocked"
                : "User successfully unblocked"
        });
    }

    //delete user
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _mediator.Send(new DeleteUserCommand(id));

        return Ok(new
        {
            message = "User deleted successfully"
        });
    }
}
