using DiplomApplication.Domain.Entities;

namespace DiplomApplication.Application.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(Order order);
    
    Task AddAsync(CartInOrder cartInOrder);
    Task UpdateAsync(CartInOrder cartInOrder);
    Task DeleteAsync(CartInOrder cartInOrder);
    Task<List<Order>> GetFullOrdersInfoAsync();
    Task<List<StatusOrder>> GetStatusOrdersAsync();
    Task<List<Courier>> GetCouriersAsync();
}