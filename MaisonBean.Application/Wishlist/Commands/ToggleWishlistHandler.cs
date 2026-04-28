using MaisonBean.Application.Interfaces;
using MaisonBean.Application.Wishlist.Commands;
using MaisonBean.Domain.Entities;
using MediatR;

namespace MaisonBean.Application.Wishlist.Commands;
public class ToggleWishlistHandler
    : IRequestHandler<ToggleWishlistCommand, WishlistResult>
{
    private readonly IWishlistRepository _wishlist;
    private readonly IUnitOfWork _uow;

    public ToggleWishlistHandler(
        IWishlistRepository wishlist,
        IUnitOfWork uow)
    {
        _wishlist = wishlist;
        _uow = uow;
    }

    public async Task<WishlistResult> Handle(
        ToggleWishlistCommand request,
        CancellationToken ct)
    {
        var existing = await _wishlist
            .GetByUserAndProductAsync(request.UserId, request.ProductId, ct);

        if (existing != null)
        {
            _wishlist.Remove(existing);
            await _uow.SaveChangesAsync(ct);

            return new WishlistResult { IsAdded = false };
        }

        var item = new WishlistItem
        {
            UserId = request.UserId,
            ProductId = request.ProductId
        };

        await _wishlist.AddAsync(item, ct);
        await _uow.SaveChangesAsync(ct);

        return new WishlistResult { IsAdded = true };
    }
}