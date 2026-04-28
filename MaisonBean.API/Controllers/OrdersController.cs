using MaisonBean.Application.Interfaces;
using MaisonBean.Application.Orders.Commands;
using MaisonBean.Application.Orders.Requests;
using MaisonBean.Application.Orders.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;
    private readonly IMediator _mediator;

    public OrderController(
     IOrderRepository orders,
     IMediator mediator,
     IUnitOfWork uow)
    {
        _orders = orders;
        _mediator = mediator;
        _uow = uow;
    }

    // ✅ GET logged-in user's orders
    [HttpGet]
    public async Task<IActionResult> GetOrders(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var orders = await _orders.GetByUserIdAsync(userId, ct);

        var result = OrderMapper.ToDtoList(orders);

        return Ok(result);
    }


    // ✅ PLACE SINGLE PRODUCT ORDER
    [HttpPost("single")]
    public async Task<IActionResult> PlaceSingleOrder(
     [FromBody] PlaceSingleOrderRequest request,
     CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var command = new PlaceSingleOrderCommand
        {
            UserId = int.Parse(userId),
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            IsCustomized = request.IsCustomized,
            BeanId = request.BeanId,
            MilkId = request.MilkId,
            DeliveryAddress = request.DeliveryAddress,
            City = request.City,
            Phone = request.Phone,
            PaymentMethod = request.PaymentMethod,
            UpiId = request.UpiId
        };

        var orderId = await _mediator.Send(command, ct);

        return Ok(new { message = "Order placed", orderId });
    }

    // ✅ PLACE ORDER (via MediatR)
    [HttpPost]
    public async Task<IActionResult> PlaceOrder(
    [FromBody] PlaceOrderRequest request,
    CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var command = new PlaceOrderCommand
        {
            UserId = int.Parse(userId),
            UserEmail = userEmail ?? "",
            DeliveryAddress = request.DeliveryAddress,
            City = request.City,
            Phone = request.Phone,
            PaymentMethod = request.PaymentMethod,
            UpiId = request.UpiId
        };

        var orderId = await _mediator.Send(command, ct);

        return Ok(new
        {
            message = "Order placed successfully",
            orderId
        });
    }

    // ✅ UPDATE STATUS (Admin only)
    [Authorize(Roles = "admin")]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] UpdateStatusRequest request,
        CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(id, ct);

        if (order == null)
            return NotFound();

        order.Status = request.Status.ToLower().Trim();

        _orders.Update(order);
        await _uow.SaveChangesAsync(ct);

        return Ok(new { message = "Status updated", order.Status });
    }

    // ✅ CANCEL ORDER (SECURE)
    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var order = await _orders.GetByIdAsync(id, ct);

        if (order == null)
            return NotFound();

        // 🔐 SECURITY CHECK
        if (order.UserId != userId)
            return Forbid();

        if (order.Status == "delivered")
            return BadRequest("Cannot cancel delivered order");

        order.Status = "cancelled";

        _orders.Update(order);
        await _uow.SaveChangesAsync(ct);

        return Ok(new { message = "Order cancelled" });
    }

    // ✅ DELETE ORDER (SECURE)
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var order = await _orders.GetByIdAsync(id, ct);

        if (order == null)
            return NotFound();

        // 🔐 SECURITY CHECK
        if (order.UserId != userId)
            return Forbid();

        _orders.Remove(order);
        await _uow.SaveChangesAsync(ct);

        return Ok(new { message = "Order deleted" });
    }
}