using MaisonBean.Application.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

    // POST /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        if (!result.Success)
            return Conflict(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        if (!result.Success)
            return Unauthorized(new { message = result.Message });

        return Ok(new
        {
            token = result.Token,
            user = result.User
        });
    }
}