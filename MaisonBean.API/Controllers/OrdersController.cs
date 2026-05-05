using MaisonBean.Application.Interfaces;
using MaisonBean.Application.Orders.Commands;
using MaisonBean.Application.Orders.DTOs;
using MaisonBean.Application.Orders.Requests;
using MaisonBean.Domain.Enums;
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

    //loged in user's Orders
    [HttpGet]
    public async Task<IActionResult> GetOrders(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var orders = await _orders.GetByUserIdAsync(userId, ct);

        return Ok(OrderMapper.ToDtoList(orders));
    }

    //post order
    [HttpPost]
    public async Task<IActionResult> PlaceOrder(
    [FromBody] PlaceOrderRequest request,
    CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out var parsedUserId))
                return Unauthorized();

            if (request.PaymentMethod?.ToLower() == "upi" && string.IsNullOrEmpty(request.UpiId))
                return BadRequest(new { message = "UPI ID is required" });

            var command = new PlaceOrderCommand
            {
                UserId = parsedUserId,
                AddressId = request.AddressId,
                PaymentMethod = request.PaymentMethod.ToLower(),
                UpiId = request.UpiId
            };

            var orderId = await _mediator.Send(command, ct);

            return Ok(new
            {
                message = "Order placed successfully",
                orderId
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("single")]
    public async Task<IActionResult> PlaceSingleOrder(
        [FromBody] PlaceSingleOrderRequest request,
        CancellationToken ct)
    {
        try
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
                AddressId = request.AddressId,
                PaymentMethod = request.PaymentMethod,
                UpiId = request.UpiId
            };

            var orderId = await _mediator.Send(command, ct);

            return Ok(new { message = "Order placed", orderId });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    //CANCEL ORDER (STATUS = cancelled)
    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var order = await _orders.GetByIdAsync(id, ct);

        if (order == null)
            return NotFound();

        if (order.UserId != userId)
            return Forbid();

        if (order.Status == OrderStatus.Delivered)
            return BadRequest("Cannot cancel delivered order");

        order.Cancel();

        _orders.Update(order);
        await _uow.SaveChangesAsync(ct);

        return Ok(new { message = "Order cancelled successfully" });
    }

    //DELETE ORDER (REMOVE FROM DB)
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var order = await _orders.GetByIdAsync(id, ct);

        if (order == null)
            return NotFound();

        if (order.UserId != userId)
            return Forbid();

        _orders.Remove(order);
        await _uow.SaveChangesAsync(ct);

        return Ok(new { message = "Order deleted permanently" });
    }

    //get all users's orders
    [Authorize(Roles = "Admin")]
    [HttpGet("all/ad")]
    public async Task<IActionResult> GetAllOrders(CancellationToken ct)
    {
        var orders = await _orders.GetAllAsync(ct);

        var result = OrderMapper.ToDtoList(orders);

        return Ok(result);
    }

    //update delivery status
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/status/ad")]
    public async Task<IActionResult> UpdateStatus(
    int id,
    [FromQuery] OrderStatus newStatus)   // creates dropdown
    {
        var command = new UpdateOrderStatusCommand
        {
            OrderId = id,
            NewStatus = newStatus
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}