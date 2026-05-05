using MaisonBean.Application.Interfaces;
using MaisonBean.Application.Payments.Interfaces;
using MaisonBean.Domain.Enums;
using MediatR;
using System;

namespace MaisonBean.Application.Payments.Commands;

public record CreateOrderCommand(int OrderId) : IRequest<object>;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, object>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderHandler(
        IOrderRepository orderRepository,
        IPaymentService paymentService,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _paymentService = paymentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<object> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
            throw new Exception("Order not found");

        var razorpay = await _paymentService.CreateOrderAsync(order.Total);

        order.RazorpayOrderId = razorpay.orderId;

        await _unitOfWork.SaveChangesAsync(); // ✅ IMPORTANT

        return new
        {
            orderId = order.Id,
            razorpayOrderId = razorpay.orderId,
            amount = razorpay.amount
        };
    }
}