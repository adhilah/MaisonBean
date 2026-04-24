using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WishlistController : ControllerBase
{
    private readonly IWishlistRepository _wishlist;
    private readonly IUnitOfWork _uow;

    public WishlistController(IWishlistRepository wishlist, IUnitOfWork uow)
    {
        _wishlist = wishlist;
        _uow = uow;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetWishlist(string userId, CancellationToken ct)
    {
        var items = await _wishlist.GetByUserIdAsync(userId, ct);
        return Ok(items);
    }

    [HttpPost("toggle")]
    public async Task<IActionResult> Toggle([FromBody] WishlistRequest request, CancellationToken ct)
    {
        var existing = await _wishlist.GetByUserAndProductAsync(request.UserId, request.ProductId, ct);

        if (existing != null)
        {
            _wishlist.Remove(existing);
            await _uow.SaveChangesAsync(ct);
            return Ok(new { message = "Removed from wishlist", wishlisted = false });
        }

        var item = new WishlistItem
        {
            UserId = request.UserId,
            ProductId = request.ProductId,
            Name = request.Name,
            Price = request.Price,
            Image = request.Image
        };

        await _wishlist.AddAsync(item, ct);
        await _uow.SaveChangesAsync(ct);
        return Ok(new { message = "Added to wishlist", wishlisted = true });
    }

    [HttpDelete("remove/{id}")]
    public async Task<IActionResult> Remove(Guid id, CancellationToken ct)
    {
        var item = await _wishlist.GetByIdAsync(id, ct);
        if (item == null) return NotFound();

        _wishlist.Remove(item);
        await _uow.SaveChangesAsync(ct);
        return Ok();
    }

    [HttpDelete("clear/{userId}")]
    public async Task<IActionResult> Clear(string userId, CancellationToken ct)
    {
        var items = await _wishlist.GetByUserIdAsync(userId, ct);
        foreach (var item in items)
            _wishlist.Remove(item);

        await _uow.SaveChangesAsync(ct);
        return Ok();
    }
}

public class WishlistRequest
{
    public string UserId { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Image { get; set; } = string.Empty;
}