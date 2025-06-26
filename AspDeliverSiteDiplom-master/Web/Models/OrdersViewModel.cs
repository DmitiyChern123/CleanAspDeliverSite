
using DiplomApplication.Domain.Entities;

namespace DiplomApplication.Web.Models
{
    public class OrdersViewModel
    {
        public List<Order> Orders { get; set; }
        public List<StatusOrder> statuses { get; set; }
        public List<Courier> courses { get; set; }
    }
}
