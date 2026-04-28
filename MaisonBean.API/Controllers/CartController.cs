using MaisonBean.Application.Cart;
using MaisonBean.Application.Cart.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken ct)
    {
        var userId = GetUserId();
        var cart = await _mediator.Send(new GetCartQuery(userId), ct);
        return Ok(cart);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddToCartCommand cmd, CancellationToken ct)
    {
        cmd.UserId = GetUserId();
        var total = await _mediator.Send(cmd, ct);
        return Ok(new { message = "Added to cart", total });
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] int quantity, CancellationToken ct)
    {
        var userId = GetUserId();

        await _mediator.Send(new UpdateCartItemCommand
        {
            CartItemId = id,
            UserId = userId,
            Quantity = quantity
        }, ct);

        return Ok(new { message = "Cart updated" });
    }

    [HttpDelete("remove/{id}")]
    public async Task<IActionResult> Remove(int id, CancellationToken ct)
    {
        var userId = GetUserId();

        await _mediator.Send(new RemoveCartItemCommand(id, userId), ct);

        return Ok(new { message = "Item removed" });
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> Clear(CancellationToken ct)
    {
        await _mediator.Send(new ClearCartCommand(GetUserId()), ct);
        return Ok(new { message = "Cart cleared" });
    }
}