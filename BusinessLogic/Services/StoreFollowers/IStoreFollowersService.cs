using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Models;

namespace BusinessLogic.Services.StoreFollowers
{
    public interface IStoreFollowersService
    {
           IQueryable<StoreFollower> GetAll();
        StoreFollower GetById(Guid id);
        Task<StoreFollower> GetAsyncById(Guid id);
        StoreFollower Find(Expression<Func<StoreFollower, bool>> match);
        Task<StoreFollower> FindAsync(Expression<Func<StoreFollower, bool>> match);
        Task AddAsync(StoreFollower entity);
        Task UpdateAsync(StoreFollower entity);
        Task DeleteAsync(StoreFollower entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<StoreFollower>> ListAsync();
        Task<IEnumerable<StoreFollower>> ListAsync(
            Expression<Func<StoreFollower, bool>> filter = null,
            Func<IQueryable<StoreFollower>, IOrderedQueryable<StoreFollower>> orderBy = null,
            Func<IQueryable<StoreFollower>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StoreFollower, object>> includeProperties = null);

    }
}