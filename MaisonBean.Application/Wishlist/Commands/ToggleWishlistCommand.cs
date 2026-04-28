using MediatR;
using System.Text.Json.Serialization;
namespace MaisonBean.Application.Wishlist.Commands;
public class ToggleWishlistCommand : IRequest<WishlistResult>
{
    public int ProductId { get; set; }

    [JsonIgnore] 
    public string UserId { get; set; }
}