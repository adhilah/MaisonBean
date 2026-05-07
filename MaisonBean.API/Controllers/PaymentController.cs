using MaisonBean.Application.Payments.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/payment")]
[Authorize(Roles = "Customer")]
public class PaymentController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    //CREATE using OrderId

    [HttpPost("create/{orderId}")]
    public async Task<IActionResult> Create(int orderId)
    {
        var result = await _mediator.Send(new CreateOrderCommand(orderId));
        return Ok(result);
    }

    //VERIFY
    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerifyPaymentCommand command)
    {
        var isValid = await _mediator.Send(command);

        if (!isValid)
            return BadRequest("Invalid payment");

        return Ok("Payment successful");
    }
}