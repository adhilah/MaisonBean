using System.ComponentModel.DataAnnotations;

namespace MaisonBean.Application.Orders.Requests;

public class PlaceOrderRequest : IValidatableObject
{
    public int AddressId { get; set; }
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Payment method is required")]
    public string PaymentMethod { get; set; } = string.Empty;

    public string? UpiId { get; set; }

    // 🔥 Conditional validation
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Normalize for safety
        var method = PaymentMethod?.Trim().ToLower();

        if (method == "upi")
        {
            if (string.IsNullOrWhiteSpace(UpiId))
            {
                yield return new ValidationResult(
                    "UPI ID is required for UPI payment",
                    new[] { nameof(UpiId) });
            }
        }

        if (method == "cod")
        {
            if (!string.IsNullOrEmpty(UpiId))
            {
                yield return new ValidationResult(
                    "UPI ID should not be provided for Cash on Delivery",
                    new[] { nameof(UpiId) });
            }
        }
    }
}