using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using MaisonBean.Application.Payments.Interfaces;
using Razorpay.Api;

namespace MaisonBean.Infrastructure.Payments
{
    public class RazorpayService : IPaymentService
    {
        private readonly IConfiguration _config;

        public RazorpayService(IConfiguration config)
        {
            _config = config;
        }

        public Task<(string orderId, int amount, string currency)> CreateOrderAsync(decimal amount)
        {
            var client = new RazorpayClient(
                _config["Razorpay:KeyId"],
                _config["Razorpay:KeySecret"]
            );

            var options = new Dictionary<string, object>
    {
        { "amount", (int)(amount * 100) },
        { "currency", "INR" },
        { "receipt", Guid.NewGuid().ToString() }
    };

            Razorpay.Api.Order razorpayOrder = client.Order.Create(options);

            string orderId = razorpayOrder["id"].ToString();
            int orderAmount = Convert.ToInt32(razorpayOrder["amount"]);
            string currency = razorpayOrder["currency"].ToString();

            return Task.FromResult((orderId, orderAmount, currency));
        }

        public Task<bool> VerifyPaymentAsync(string orderId, string paymentId, string signature)
        {
            var secret = _config["Razorpay:KeySecret"]
                ?? throw new Exception("Razorpay secret missing");

            var payload = $"{orderId}|{paymentId}";
            var secretBytes = Encoding.UTF8.GetBytes(secret);

            using var hmac = new HMACSHA256(secretBytes);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var generatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return Task.FromResult(generatedSignature == signature);
        }
    }
}