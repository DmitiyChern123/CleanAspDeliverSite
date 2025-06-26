namespace DiplomApplication.Domain.Entities;

public partial class Courier
{
    public int Id { get; set; }

    public string Fio { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
