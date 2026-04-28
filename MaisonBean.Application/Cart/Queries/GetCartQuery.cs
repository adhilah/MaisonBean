using MaisonBean.Application.Interfaces;
using MediatR;

namespace MaisonBean.Application.Cart.Queries;

// Query
public record GetCartQuery(int UserId) : IRequest<CartResultDto>;


// Result DTO
public class CartResultDto
{
    public List<CartItemDto> Items { get; set; } = new();

    public int TotalQuantity => Items.Sum(i => i.Quantity);
    public decimal TotalPrice => Items.Sum(i => i.TotalPrice);
}


// Item DTO
public class CartItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }

    public string Name { get; set; } = default!;
    public string? Image { get; set; }
    public string? Category { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public decimal TotalPrice => UnitPrice * Quantity;

    public bool IsCustomized { get; set; }

    public int? BeanId { get; set; }
    public string? BeanName { get; set; }

    public int? MilkId { get; set; }
    public string? MilkName { get; set; }
}


// Handler
public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartResultDto>
{
    private readonly ICartRepository _cart;

    public GetCartQueryHandler(ICartRepository cart)
    {
        _cart = cart;
    }

    public async Task<CartResultDto> Handle(GetCartQuery request, CancellationToken ct)
    {
        var items = await _cart.GetByUserIdAsync(request.UserId, ct);

        return new CartResultDto
        {
            Items = items.Select(i => new CartItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,

                Name = i.ProductName,
                Image = i.ProductImage,
                Category = i.ProductCategory,

                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,

                // TotalPrice = i.TotalPrice,

                IsCustomized = i.IsCustomized,

                BeanId = i.BeanId,
                BeanName = i.Bean?.Name,

                MilkId = i.MilkId,
                MilkName = i.Milk?.Name,
            }).ToList()
        };
    }
}