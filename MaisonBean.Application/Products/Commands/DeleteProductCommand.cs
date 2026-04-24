using MaisonBean.Application.Interfaces;
using MediatR;

namespace MaisonBean.Application.Products.Commands;

public class DeleteProductCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _productRepo;
    private readonly IUnitOfWork _uow;

    public DeleteProductCommandHandler(IProductRepository productRepo, IUnitOfWork uow)
    {
        _productRepo = productRepo;
        _uow = uow;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.Id);
        if (product == null) return false;

        product.Deactivate();           // soft delete
        _productRepo.Update(product);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}