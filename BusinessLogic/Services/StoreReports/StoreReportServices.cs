using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Models;
using Repository.StoreReport;

namespace BusinessLogic.Services.StoreReports
{
    public class StoreReportServices : IStoreReportServices
    {
        private readonly IStoreReportRepository _repository;

        public StoreReportServices(IStoreReportRepository repository)
        {
            _repository = repository;
        }

        public IQueryable<StoreReport> GetAll() => _repository.GetAll();

        public StoreReport GetById(Guid id) => _repository.GetById(id);

        public async Task<StoreReport> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public StoreReport Find(Expression<Func<StoreReport, bool>> match) => _repository.Find(match);

        public async Task<StoreReport> FindAsync(Expression<Func<StoreReport, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(StoreReport entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(StoreReport entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(StoreReport entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<StoreReport>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<StoreReport>> ListAsync(
            Expression<Func<StoreReport, bool>> filter = null,
            Func<IQueryable<StoreReport>, IOrderedQueryable<StoreReport>> orderBy = null,
            Func<IQueryable<StoreReport>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StoreReport, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
    }
        
    
}