using MaisonBean.Application.Cart;
using MaisonBean.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartRepository _cart;
    private readonly IUnitOfWork _uow;
    private readonly IMediator _mediator;

    public CartController(ICartRepository cart, IUnitOfWork uow, IMediator mediator)
    {
        _cart = cart;
        _uow = uow;
        _mediator = mediator;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCart(string userId, CancellationToken ct)
    {
        var items = await _cart.GetByUserIdAsync(userId, ct);
        return Ok(new { items, totalQuantity = items.Sum(i => i.Quantity) });
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] AddToCartRequest request, CancellationToken ct)
    {
        var total = await _mediator.Send(new AddToCartCommand
        {
            UserId = request.UserId,
            ProductId = request.ProductId,
            IsCustomized = request.IsCustomized,
            BeanId = request.BeanId,
            MilkId = request.MilkId,
            Quantity = request.Quantity
        }, ct);
        return Ok(new { message = "Added to cart", total });
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] int quantity, CancellationToken ct)
    {
        var item = await _cart.GetByIdAsync(id, ct);
        if (item == null) return NotFound();

        if (quantity <= 0)
            _cart.RemoveItem(item);
        else
        {
            item.UpdateQuantity(quantity);
            _cart.Update(item);
        }

        await _uow.SaveChangesAsync(ct);
        return Ok();
    }

    [HttpDelete("remove/{id}")]
    public async Task<IActionResult> Remove(Guid id, CancellationToken ct)
    {
        var item = await _cart.GetByIdAsync(id, ct);
        if (item == null) return NotFound();
        _cart.RemoveItem(item);
        await _uow.SaveChangesAsync(ct);
        return Ok();
    }

    [HttpDelete("clear/{userId}")]
    public async Task<IActionResult> Clear(string userId, CancellationToken ct)
    {
        var items = await _cart.GetByUserIdAsync(userId, ct);
        foreach (var item in items)
            _cart.RemoveItem(item);
        await _uow.SaveChangesAsync(ct);
        return Ok();
    }
}

public class AddToCartRequest
{
    public string UserId { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public bool IsCustomized { get; set; }
    public Guid? BeanId { get; set; }
    public Guid? MilkId { get; set; }
    public int Quantity { get; set; } = 1;
}