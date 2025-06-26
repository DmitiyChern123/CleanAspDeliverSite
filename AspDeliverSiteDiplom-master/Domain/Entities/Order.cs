namespace DiplomApplication.Domain.Entities;

public partial class Order
{
    public int Id { get; set; }

    public string Adress { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string PayType { get; set; } = null!;

    public int UserId { get; set; }

    public int StatusId { get; set; }

    public int? CourierId { get; set; }

    public bool IsNeedDevices { get; set; }

    public DateTime Date { get; set; }

    public int Sum { get; set; }

    public virtual ICollection<CartInOrder> CartInOrders { get; set; } = new List<CartInOrder>();

    public virtual Courier? Courier { get; set; }

    public virtual StatusOrder Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
