namespace MaisonBean.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public string? ProductCategory { get; set; }
        public decimal BasePrice { get; set; }
        public int Quantity { get; set; }
        public Guid? BeanId { get; set; }
        public string? BeanName { get; set; }
        public decimal BeanPriceAdd { get; set; }
        public Guid? MilkId { get; set; }
        public string? MilkName { get; set; }
        public decimal MilkPriceAdd { get; set; }
    }
}