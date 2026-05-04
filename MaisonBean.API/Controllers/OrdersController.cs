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

    //Get All Order
    [HttpGet]
    public async Task<IActionResult> GetOrders(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var orders = await _orders.GetByUserIdAsync(userId, ct);

        return Ok(OrderMapper.ToDtoList(orders));
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder(
        [FromBody] PlaceOrderRequest request,
        CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var command = new PlaceOrderCommand
            {
                UserId = int.Parse(userId),
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
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message }); // ✅ 400
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Something went wrong" });
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
                DeliveryAddress = request.DeliveryAddress,
                City = request.City,
                Phone = request.Phone,
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

    ////update status
    //[Authorize(Roles = "Admin")]
    //[HttpPatch("{id}/status")]
    //public async Task<IActionResult> UpdateStatus(
    //int id,
    //[FromBody] UpdateOrderStatusCommand command,
    //CancellationToken ct)
    //{
    //    if (id != command.OrderId)
    //        return BadRequest("Order ID mismatch");

    //    var result = await _mediator.Send(command, ct);

    //    if (!result.Success)
    //        return BadRequest(new { message = result.Message });

    //    return Ok(new
    //    {
    //        message = result.Message,
    //        status = result.Status
    //    });
    //}



    [Authorize(Roles = "Admin")]
    [HttpGet("{id}/status-options")]
    private List<string> GetNextStatuses(OrderStatus current)
    {
        return current switch
        {
            OrderStatus.Pending => new() { "Processing" },
            OrderStatus.Processing => new() { "Shipping" },
            OrderStatus.Shipping => new() { "OutForDelivery" },
            OrderStatus.OutForDelivery => new() { "Delivered" },
            _ => new List<string>()
        };
    }
}