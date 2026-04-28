using MaisonBean.Application.Auth.Commands;
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

    // POST /api/auth/login
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
            user = result.User
        });
    }
}