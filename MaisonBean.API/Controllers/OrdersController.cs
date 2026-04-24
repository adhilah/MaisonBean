using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;

    public OrderController(IOrderRepository orders, IUnitOfWork uow)
    {
        _orders = orders;
        _uow = uow;
    }

    // GET api/Order/{userId}
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetOrders(string userId, CancellationToken ct)
    {
        var orders = await _orders.GetByUserIdAsync(userId, ct);
        return Ok(orders);
    }

    // GET api/Order/detail/{id}
    [HttpGet("detail/{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(id, ct);
        if (order == null) return NotFound();
        return Ok(order);
    }

    // POST api/Order
    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request, CancellationToken ct)
    {
        var order = new Order
        {
            UserId = request.UserId,
            UserEmail = request.UserEmail,
            PaymentMethod = request.PaymentMethod,
            UpiId = request.UpiId,
            Subtotal = request.Subtotal,
            Shipping = request.Shipping,
            Total = request.Total,
            Status = "pending",
            Items = request.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductImage = i.ProductImage,
                ProductCategory = i.ProductCategory,
                BasePrice = i.BasePrice,
                Quantity = i.Quantity,
                BeanId = i.BeanId,
                BeanName = i.BeanName,
                BeanPriceAdd = i.BeanPriceAdd,
                MilkId = i.MilkId,
                MilkName = i.MilkName,
                MilkPriceAdd = i.MilkPriceAdd,
            }).ToList()
        };

        await _orders.AddAsync(order, ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(new { message = "Order placed successfully", orderId = order.Id });
    }

    // PATCH api/Order/{id}/status
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(id, ct);
        if (order == null) return NotFound();

        order.Status = request.Status.ToLower().Trim();
        _orders.Update(order);
        await _uow.SaveChangesAsync(ct);

        return Ok(new { message = "Status updated", status = order.Status });
    }

    // PATCH api/Order/{id}/cancel
    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(id, ct);
        if (order == null) return NotFound();
        if (order.Status == "delivered")
            return BadRequest(new { message = "Cannot cancel a delivered order" });

        order.Status = "cancelled";
        _orders.Update(order);
        await _uow.SaveChangesAsync(ct);

        return Ok(new { message = "Order cancelled" });
    }

    // DELETE api/Order/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(id, ct);
        if (order == null) return NotFound();

        _orders.Remove(order);
        await _uow.SaveChangesAsync(ct);

        return Ok(new { message = "Order deleted" });
    }
}

public class PlaceOrderRequest
{
    public string UserId { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string? UpiId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Shipping { get; set; }
    public decimal Total { get; set; }
    public List<OrderItemRequest> Items { get; set; } = new();
}

public class OrderItemRequest
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public string? ProductCategory { get; set; }
    public decimal BasePrice { get; set; }
    public int Quantity { get; set; }
    public Guid? BeanId { get; set; }
    public string? BeanName { get; set; }
    public decimal BeanPriceAdd { get; set; }
    public Guid? MilkId { get; set; }
    public string? MilkName { get; set; }
    public decimal MilkPriceAdd { get; set; }
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
}