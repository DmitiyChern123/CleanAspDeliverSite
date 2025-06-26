namespace DiplomApplication.Domain.Entities;

public partial class Product
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Opis { get; set; }

    public int? Price { get; set; }

    public int? IdCategory { get; set; }

    public string? Img { get; set; }

    public string? Grams { get; set; }

    public string? Calories { get; set; }

    public bool IsHidden { get; set; }

    public virtual Category? IdCategoryNavigation { get; set; }

    public virtual ICollection<ProductType> ProductTypes { get; set; } = new List<ProductType>();
}
