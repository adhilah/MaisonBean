using System.ComponentModel.DataAnnotations;

namespace MaisonBean.Application.Addresses.Requests;

public class UpdateAddressRequest
{
    [Required(ErrorMessage = "Delivery address is required")]
    [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
    public string DeliveryAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required")]
    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    [StringLength(15, MinimumLength = 10,
        ErrorMessage = "Phone number must be between 10 and 15 digits")]
    public string Phone { get; set; } = string.Empty;
}