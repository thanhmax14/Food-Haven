using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Models;
using Repository.StoreFollower;

namespace BusinessLogic.Services.StoreFollowers
{
    public class StoreFollowersService : IStoreFollowersService
    {
        private readonly IStoreFollowersRepository _StoreFollowerRepository;

        public StoreFollowersService(IStoreFollowersRepository storeFollowersRepository)
        {
            _StoreFollowerRepository = storeFollowersRepository;
        }
          public IQueryable<StoreFollower> GetAll() => _StoreFollowerRepository.GetAll();

        public StoreFollower GetById(Guid id) => _StoreFollowerRepository.GetById(id);

        public async Task<StoreFollower> GetAsyncById(Guid id) => await _StoreFollowerRepository.GetAsyncById(id);

        public StoreFollower Find(Expression<Func<StoreFollower, bool>> match) => _StoreFollowerRepository.Find(match);

        public async Task<StoreFollower> FindAsync(Expression<Func<StoreFollower, bool>> match) => await _StoreFollowerRepository.FindAsync(match);

        public async Task AddAsync(StoreFollower entity) => await _StoreFollowerRepository.AddAsync(entity);

        public async Task UpdateAsync(StoreFollower entity) => await _StoreFollowerRepository.UpdateAsync(entity);

        public async Task DeleteAsync(StoreFollower entity) => await _StoreFollowerRepository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _StoreFollowerRepository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _StoreFollowerRepository.ExistsAsync(id);
        public int Count() => _StoreFollowerRepository.Count();

        public async Task<int> CountAsync() => await _StoreFollowerRepository.CountAsync();

        public async Task<IEnumerable<StoreFollower>> ListAsync() => await _StoreFollowerRepository.ListAsync();

        public async Task<IEnumerable<StoreFollower>> ListAsync(
            Expression<Func<StoreFollower, bool>> filter = null,
            Func<IQueryable<StoreFollower>, IOrderedQueryable<StoreFollower>> orderBy = null,
            Func<IQueryable<StoreFollower>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StoreFollower, object>> includeProperties = null) =>
            await _StoreFollowerRepository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _StoreFollowerRepository.SaveChangesAsync();
    }
}