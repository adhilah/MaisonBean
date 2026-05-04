using MaisonBean.Application.Interfaces;
using MediatR;

namespace MaisonBean.Application.Products.Commands;
public record ToggleProductCommand(int Id) : IRequest<bool>;

public class ToggleProductCommandHandler
    : IRequestHandler<ToggleProductCommand, bool>
{
    private readonly IProductRepository _repo;
    private readonly IUnitOfWork _uow;

    public ToggleProductCommandHandler(IProductRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<bool> Handle(ToggleProductCommand request, CancellationToken ct)
    {
        var product = await _repo.GetByIdAsync(request.Id, ct);

        if (product == null)
            return false;
        //throw new ArgumentException("Product not found");

        //toggle
        product.ToggleBlock();

        _repo.Update(product);
        await _uow.SaveChangesAsync(ct);

        //return current state
        return product.IsBlocked;
    }
}