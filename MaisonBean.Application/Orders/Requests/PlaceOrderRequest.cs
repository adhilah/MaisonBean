using System.ComponentModel.DataAnnotations;

namespace MaisonBean.Application.Orders.Requests;

public class PlaceOrderRequest : IValidatableObject
{
    [Required(ErrorMessage = "Delivery address is required")]
    [MinLength(5, ErrorMessage = "Delivery address must be at least 5 characters")]
    public string DeliveryAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be 10 digits")]
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