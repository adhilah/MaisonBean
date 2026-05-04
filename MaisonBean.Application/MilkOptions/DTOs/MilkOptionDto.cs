namespace MaisonBean.Application.MilkOptions.DTOs;

public class MilkOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceAdd { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Calories { get; set; }
    public bool IsBlocked { get; set; }
}