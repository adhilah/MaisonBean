using MaisonBean.Application.Interfaces;
using MaisonBean.Application.Payments.Interfaces;
using MaisonBean.Domain.Enums;
using MediatR;


namespace MaisonBean.Application.Payments.Commands;

public record VerifyPaymentCommand(
    int OrderId,
    string RazorpayOrderId,
    string RazorpayPaymentId,
    string RazorpaySignature
) : IRequest<bool>;


//handler
public class VerifyPaymentHandler : IRequestHandler<VerifyPaymentCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyPaymentHandler(
        IOrderRepository orderRepository,
        IPaymentService paymentService,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _paymentService = paymentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(VerifyPaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
            throw new Exception("Order not found");

        var isValid = await _paymentService.VerifyPaymentAsync(
            request.RazorpayOrderId,
            request.RazorpayPaymentId,
            request.RazorpaySignature
        );

        if (!isValid)
            return false;

        order.RazorpayPaymentId = request.RazorpayPaymentId;
        order.PaidAt = DateTime.UtcNow;

        order.UpdateStatus(OrderStatus.Processing);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}