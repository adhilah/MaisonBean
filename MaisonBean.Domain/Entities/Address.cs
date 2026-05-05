using MaisonBean.Domain.Common;

namespace MaisonBean.Domain.Entities;
public class Address : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

    public string DeliveryAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public bool IsDefault { get; set; } = false;
}