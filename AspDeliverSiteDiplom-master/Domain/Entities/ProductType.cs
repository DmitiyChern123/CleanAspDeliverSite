namespace DiplomApplication.Domain.Entities;

public partial class ProductType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Price { get; set; }

    public int ProductId { get; set; }

    public bool IsDelated { get; set; }

    public virtual ICollection<CartInOrder> CartInOrders { get; set; } = new List<CartInOrder>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
}
