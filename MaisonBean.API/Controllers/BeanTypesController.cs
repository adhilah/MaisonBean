using MaisonBean.Application.BeanTypes.Commands;
using MaisonBean.Application.BeanTypes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BeanTypesController : ControllerBase
{
    private readonly IMediator _mediator;

    public BeanTypesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
        => Ok(await _mediator.Send(new GetBeanTypesQuery()));

    //CREATE
    [HttpPost("bean/ad")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateBeanTypeCommand command)
    {
        var id = await _mediator.Send(command);

        return Ok(new
        {
            message = "Bean type add successfully",
            id = id
        });
    }

    //UPDATE
    [HttpPut("{id}update/ad")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateBeanTypeCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);

        return Ok(new
        {
            message = "Bean type updated successfully"
        });
    }

    //TOGGLE -BLOCK
    [HttpPatch("{id}/block/ad")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Toggle(int id)
    {
        var isBlocked = await _mediator.Send(new ToggleBeanTypeCommand(id));

        return Ok(new
        {
            message = isBlocked
                ? "Bean type successfully blocked"
                : "Bean type successfully unblocked"
        });
    }

    //DELETE
    [HttpDelete("{id}/ad")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteBeanTypeCommand(id));

        return Ok(new
        {
            message = "Bean type deleted successfully"
        });
    }
}