using MaisonBean.Application.Auth.Commands;
using MaisonBean.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MaisonBean.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    //register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand cmd)
    {

        if (!ModelState.IsValid)
            return BadRequest(new
            {
                success = false,
                errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage)
                    )
            });

        var result = await _mediator.Send(cmd);

        if (!result.Success)
            return Conflict(new
            {
                success = false,
                message = result.Message
            });

        return StatusCode(201, new
        {
            success = true,
            message = result.Message
        });
    }

    // login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand cmd)
    {

        if (!ModelState.IsValid)
            return BadRequest(new
            {
                success = false,
                errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage)
                    )
            });


        var result = await _mediator.Send(cmd);

        if (!result.Success)
            return Unauthorized(new
            {
                success = false,
                message = result.Message
            });

        return Ok(new
        {
            success = true,
            message = "Login successful",
            token = result.Token,
            refreshToken = result.RefreshToken,
            user = result.User
        });
    }


    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new
            {
                success = false,
                message = "User not authenticated"
            });

        var result = await _mediator.Send(new LogoutCommand
        {
            UserId = int.Parse(userId)
        }, ct);

        if (!result)
            return BadRequest(new
            {
                success = false,
                message = "Logout failed"
            });

        return Ok(new
        {
            success = true,
            message = "Logged out successfully"
        });
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenRequest request)
    {
        var result = await _mediator.Send(new RefreshTokenCommand
        {
            Token = request.Token,
            RefreshToken = request.RefreshToken
        });

        if (!result.Success)
            return Unauthorized(new
            {
                success = false,
                message = result.Message
            });

        return Ok(new
        {
            success = true,
            token = result.Token,
            refreshToken = result.RefreshToken
        });
    }
}