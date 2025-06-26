using System.Linq.Expressions;
using DiplomApplication.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiplomApplication.Infrastructure.Repositories;

public class Repository<T> : IRepository<T>, IAsyncDisposable where T : class
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public T? GetById(int id) => _dbSet.Find(id);
    
    public IEnumerable<T> GetAll() => _dbSet.ToList();
    
    public void Add(T entity) => _dbSet.Add(entity);
    
    public void Update(T entity) => _context.Entry(entity).State = EntityState.Modified;
    
    public void Delete(int id)
    {
        var entity = _dbSet.Find(id);
        if (entity != null) _dbSet.Remove(entity);
    }
    
    public IEnumerable<T> Find(Expression<Func<T, bool>> predicate) => _dbSet.Where(predicate).ToList();

    public void Dispose()
    {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}