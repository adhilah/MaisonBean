using System.ComponentModel.DataAnnotations;

namespace MaisonBean.Application.Orders.Requests;

public class PlaceSingleOrderRequest : IValidatableObject
{
    [Required(ErrorMessage = "ProductId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "ProductId must be greater than 0")]
    public int ProductId { get; set; }

    [Range(1, 10, ErrorMessage = "Quantity must be between 1 and 10")]
    public int Quantity { get; set; } = 1;

    public bool IsCustomized { get; set; }

    public int? BeanId { get; set; }
    public int? MilkId { get; set; }

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

    // 🔥 Conditional + Business Validation
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var method = PaymentMethod?.Trim().ToLower();

        // 🔹 Customization validation
        if (IsCustomized)
        {
            if (!BeanId.HasValue || BeanId <= 0)
            {
                yield return new ValidationResult(
                    "BeanId is required when customization is enabled",
                    new[] { nameof(BeanId) });
            }

            if (!MilkId.HasValue || MilkId <= 0)
            {
                yield return new ValidationResult(
                    "MilkId is required when customization is enabled",
                    new[] { nameof(MilkId) });
            }
        }
        else
        {
            if (BeanId.HasValue || MilkId.HasValue)
            {
                yield return new ValidationResult(
                    "BeanId and MilkId must be null when customization is disabled",
                    new[] { nameof(BeanId), nameof(MilkId) });
            }
        }

        // 🔹 Payment validation
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