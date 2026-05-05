namespace MaisonBean.Application.Payments.Interfaces;
public interface IPaymentService
{
    Task<(string orderId, int amount, string currency)> CreateOrderAsync(decimal amount);
    Task<bool> VerifyPaymentAsync(string orderId, string paymentId, string signature);
}