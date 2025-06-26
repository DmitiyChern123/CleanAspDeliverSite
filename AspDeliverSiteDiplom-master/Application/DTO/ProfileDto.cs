using DiplomApplication.Domain.Entities;

namespace DiplomApplication.Application.DTO;

public class ProfileDto
{
    
    public User user { get; set; }
    public List<Order> orders { get; set; }
}