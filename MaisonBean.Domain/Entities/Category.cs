using MaisonBean.Domain.Common;

namespace MaisonBean.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;

    private Category() { }

    public static Category Create(string name, string slug)
    {
        return new Category { Name = name, Slug = slug };
    }
}