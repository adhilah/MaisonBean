namespace MaisonBean.Application.BeanTypes.DTOs;

public class BeanTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceAdd { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsBlocked { get; set; }
}