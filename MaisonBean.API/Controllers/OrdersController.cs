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

    // ✅ PLACE ORDER (Cart → Order)
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

    // ✅ PLACE SINGLE PRODUCT ORDER (Buy Now)
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
}