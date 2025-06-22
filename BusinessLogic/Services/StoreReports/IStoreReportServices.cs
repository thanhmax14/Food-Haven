using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Models;

namespace BusinessLogic.Services.StoreReports
{
    public interface IStoreReportServices
    {
          IQueryable<StoreReport> GetAll();
        StoreReport GetById(Guid id);
        Task<StoreReport> GetAsyncById(Guid id);
        StoreReport Find(Expression<Func<StoreReport, bool>> match);
        Task<StoreReport> FindAsync(Expression<Func<StoreReport, bool>> match);
        Task AddAsync(StoreReport entity);
        Task UpdateAsync(StoreReport entity);
        Task DeleteAsync(StoreReport entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<StoreReport>> ListAsync();
        Task<IEnumerable<StoreReport>> ListAsync(
            Expression<Func<StoreReport, bool>> filter = null,
            Func<IQueryable<StoreReport>, IOrderedQueryable<StoreReport>> orderBy = null,
            Func<IQueryable<StoreReport>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StoreReport, object>> includeProperties = null);
    }
    }