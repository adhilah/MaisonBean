namespace MaisonBean.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string? UpiId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = "pending";
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public static Order Create(string userId, string paymentMethod, decimal shipping) =>
            new Order
            {
                UserId = userId,
                PaymentMethod = paymentMethod,
                Shipping = shipping,
            };

        public void AddItem(Product product, int quantity)
        {
            Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductImage = product.Image,
                BasePrice = product.Price,
                Quantity = quantity,
            });
            Subtotal = Items.Sum(i => i.BasePrice * i.Quantity);
            Total = Subtotal + Shipping;
        }
    }
}