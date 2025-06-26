using DiplomApplication.Application.Interfaces;
using DiplomApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiplomApplication.Infrastructure.Repositories;
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetByIdAsync(int id)
        => await _context.Users.FindAsync(id);

    public async Task<User> GetByLoginAsync(string name)
        => await _context.Users.FirstOrDefaultAsync(u => u.Login == name);
    
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task UpdateAsync(User user)
        => _context.Users.Update(user);
    
    public async Task<User> GetUserFromLoginAndPasswordHashAsync(string login,string passwordHash)
        => await _context.Users.FirstOrDefaultAsync(d => d.Login == login && d.Password == passwordHash);
    
    public async Task DeleteAsync(User user)
        => _context.Users.Remove(user);
    public async Task<List<Order>> GetOrdersByUserAsync(int userid) 
        => await _context.Orders.Include(d=>d.Status).Where(d => d.UserId == userid).ToListAsync();
}
