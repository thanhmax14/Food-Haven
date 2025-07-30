using AutoMapper;
using Models;
using Repository.RecipeViewHistorys;
using Repository.Wishlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.RecipeViewHistorys
{
    public class RecipeViewHistoryServices: IRecipeViewHistoryServices
    {
        private readonly IRecipeViewHistoryRepository _repository;
        private readonly IMapper _mapper;

        public RecipeViewHistoryServices(IRecipeViewHistoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IQueryable<RecipeViewHistory> GetAll() => _repository.GetAll();

        public RecipeViewHistory GetById(Guid id) => _repository.GetById(id);

        public async Task<RecipeViewHistory> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public RecipeViewHistory Find(Expression<Func<RecipeViewHistory, bool>> match) => _repository.Find(match);

        public async Task<RecipeViewHistory> FindAsync(Expression<Func<RecipeViewHistory, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(RecipeViewHistory entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(RecipeViewHistory entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(RecipeViewHistory entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<RecipeViewHistory>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<RecipeViewHistory>> ListAsync(
            Expression<Func<RecipeViewHistory, bool>> filter = null,
            Func<IQueryable<RecipeViewHistory>, IOrderedQueryable<RecipeViewHistory>> orderBy = null,
            Func<IQueryable<RecipeViewHistory>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<RecipeViewHistory, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
    }
}
