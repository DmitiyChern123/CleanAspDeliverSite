using DiplomApplication.Application.Interfaces;
using DiplomApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiplomApplication.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
      private readonly AppDbContext _context;

      public OrderRepository(AppDbContext context)
      {
            _context = context;
      }

      public async Task AddAsync(Order order)
            => _context.Orders.AddAsync(order);

      public async Task UpdateAsync(Order order)
            => _context.Orders.Update(order);

      public async Task DeleteAsync(Order order)
            => _context.Orders.Remove(order);
      
      
      public async Task AddAsync(CartInOrder cartInOrder)
            => _context.CartInOrders.AddAsync(cartInOrder);

      public async Task UpdateAsync(CartInOrder cartInOrder)
            => _context.CartInOrders.Update(cartInOrder);

      public async Task DeleteAsync(CartInOrder cartInOrder)
            => _context.CartInOrders.Remove(cartInOrder);
     public async Task<List<Order>> GetFullOrdersInfoAsync()
            => await _context.Orders.Include (o => o.Status)
                  .Include(o => o.User)
                  .Include(o => o.Courier)
                  .ToListAsync();

     public async Task<List<StatusOrder>> GetStatusOrdersAsync()
           => await _context.StatusOrders.ToListAsync();

     public async Task<List<Courier>> GetCouriersAsync()
           => await _context.Couriers.Include(c => c.Orders).ToListAsync();

}