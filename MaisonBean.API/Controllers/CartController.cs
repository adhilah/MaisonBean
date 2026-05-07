
using MaisonBean.Application.Cart;
using MaisonBean.Application.Cart.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Authorize(Roles = "Customer")]
[Route("api/cart")]
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

    // ✅ GET CART
    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken ct)
    {
        var userId = GetUserId();
        var cart = await _mediator.Send(new GetCartQuery(userId), ct);
        return Ok(cart);
    }

    // ✅ ADD TO CART
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddToCartCommand cmd, CancellationToken ct)
    {
        cmd.UserId = GetUserId();

        var result = await _mediator.Send(cmd, ct);

        return Ok(new
        {
            message = "Item added to cart",
            cartItemId = result.CartItemId,
            total = result.Total
        });
    }

    // ✅ UPDATE QUANTITY
    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] UpdateCartItemCommand cmd, CancellationToken ct)
    {
        cmd.UserId = GetUserId();

        await _mediator.Send(cmd, ct);

        return Ok(new { message = "Cart updated" });
    }

    // ✅ REMOVE ITEM
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id, CancellationToken ct)
    {
        await _mediator.Send(new RemoveCartItemCommand(id, GetUserId()), ct);

        return Ok(new { message = "Item removed" });
    }

    // ✅ CLEAR CART
    [HttpDelete("clear")]
    public async Task<IActionResult> Clear(CancellationToken ct)
    {
        await _mediator.Send(new ClearCartCommand(GetUserId()), ct);

        return Ok(new { message = "Cart cleared" });
    }
}