namespace DiplomApplication.Domain.Entities;

public partial class Cart
{
    public int Id { get; set; }

    public int ProductTypeId { get; set; }

    public int UserId { get; set; }

    public int Count { get; set; }

    public virtual ProductType ProductType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
