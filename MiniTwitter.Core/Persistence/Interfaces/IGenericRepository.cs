using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Core.Persistence.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        IQueryable<T> QueryAll(Expression<Func<T, bool>> predicate);
        IQueryable<T> QueryAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    }
}
