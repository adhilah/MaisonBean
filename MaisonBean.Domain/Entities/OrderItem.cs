using MaisonBean.Domain.Common;

namespace MaisonBean.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        //public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public string? ProductCategory { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int? BeanId { get; set; }
        public decimal BeanPriceAdd { get; set; }
        public int? MilkId { get; set; }
        public decimal MilkPriceAdd { get; set; }
    }
}