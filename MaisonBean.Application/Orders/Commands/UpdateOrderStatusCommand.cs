using MaisonBean.Application.Interfaces;
using MaisonBean.Domain.Enums;
using MediatR;

namespace MaisonBean.Application.Orders.Commands;

public class UpdateOrderStatusCommand : IRequest<UpdateOrderStatusResult>
{
    public int OrderId { get; set; }
    public OrderStatus NewStatus { get; set; }
}
public class UpdateOrderStatusResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Status { get; set; }
}

public class UpdateOrderStatusCommandHandler
    : IRequestHandler<UpdateOrderStatusCommand, UpdateOrderStatusResult>
{
    private readonly IOrderRepository _repo;
    private readonly IUnitOfWork _uow;

    public UpdateOrderStatusCommandHandler(IOrderRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<UpdateOrderStatusResult> Handle(UpdateOrderStatusCommand request, CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(request.OrderId, ct);

        if (order == null)
        {
            return new UpdateOrderStatusResult
            {
                Success = false,
                Message = "Order not found"
            };
        }

        try
        {
            order.UpdateStatus(request.NewStatus);

            _repo.Update(order);
            await _uow.SaveChangesAsync(ct);

            return new UpdateOrderStatusResult
            {
                Success = true,
                Message = $"Order moved to {request.NewStatus}",
                Status = request.NewStatus.ToString()
            };
        }
        catch (Exception ex)
        {
            return new UpdateOrderStatusResult
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
}