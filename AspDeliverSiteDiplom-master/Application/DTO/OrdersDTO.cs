using DiplomApplication.Domain.Entities;

namespace DiplomApplication.Application.DTO;

public class OrdersDTO
{
    
    public List<Order> Orders { get; set; }
    public List<StatusOrder> statuses { get; set; }
    public List<Courier> courses { get; set; }
}