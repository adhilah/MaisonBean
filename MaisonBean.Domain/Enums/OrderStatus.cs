using System.ComponentModel;

namespace MaisonBean.Domain.Enums;

public enum OrderStatus
{
    Pending,
    Processing,
    Shipping,
    OutForDelivery,
    Delivered,
    Cancelled
}

//public enum PaymentMethod
//{
//    Card = 1,
//    UPI,
//    COD
//}