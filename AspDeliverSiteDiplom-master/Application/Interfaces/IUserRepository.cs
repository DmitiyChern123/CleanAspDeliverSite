using DiplomApplication.Domain.Entities;

namespace DiplomApplication.Application.Interfaces;

public interface IUserRepository
{
    Task<User> GetByIdAsync(int id);
    Task<User> GetByLoginAsync(string name);
    
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task<User> GetUserFromLoginAndPasswordHashAsync(string login, string passwordHash);
    Task<List<Order>> GetOrdersByUserAsync(int userid);

}