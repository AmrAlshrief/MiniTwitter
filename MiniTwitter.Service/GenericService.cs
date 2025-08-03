using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Core.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Service
{
    public class GenericService<T> : IGenericService<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;
        public GenericService(IGenericRepository<T> repository)
        {
            _repository = repository;
        }
        public async Task AddAsync(T entity)
        {
            await _repository.AddAsync(entity);
        }
        public async Task DeleteAsync(T entity)
        {
            await _repository.DeleteAsync(entity);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate) 
        {
            return await _repository.FindOneAsync(predicate);
        }

        public async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            return await _repository.FindOneAsync(predicate, includes);
        }
        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _repository.FindAllAsync(predicate);
        }
        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes) 
        {
            return await _repository.FindAllAsync(predicate, includes);
        }
        public async Task<T> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task UpdateAsync(T entity)
        {
            await _repository.UpdateAsync(entity);
        }
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) 
        {
            return await _repository.ExistsAsync(predicate);
        }

        public IQueryable<T> QueryAll(Expression<Func<T, bool>> predicate)
        {
            return _repository.QueryAll(predicate);
        }

        public IQueryable<T> QueryAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            return _repository.QueryAll(predicate, includes);
        }
    }
}
