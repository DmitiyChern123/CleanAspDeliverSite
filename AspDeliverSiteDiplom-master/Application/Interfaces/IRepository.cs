using System.Linq.Expressions;

namespace DiplomApplication.Application.Interfaces;

public interface IRepository<T> : IDisposable where T : class
{
    T? GetById(int id);
    IEnumerable<T> GetAll();
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
    IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
}