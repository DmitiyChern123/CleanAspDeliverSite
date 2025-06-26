namespace DiplomApplication.Domain.Entities;

public partial class Category
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Img { get; set; }

    public string? Fullimg { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
