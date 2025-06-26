namespace DiplomApplication.Domain.Entities;

public partial class CartInOrder
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int OrderId { get; set; }

    public int Count { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual ProductType Product { get; set; } = null!;
}
