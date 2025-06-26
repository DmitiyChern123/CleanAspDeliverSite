
using DiplomApplication.Domain.Entities;

namespace DiplomApplication.Web.Models
{
    public class ProfileViewModel
    {
        public User user { get; set; }
        public List<Order> orders { get; set; }
    }
}
