using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Models;
using Repository.FavoriteRecipes;

namespace BusinessLogic.Services.FavoriteFavoriteRecipes
{
    public class FavoriteRecipeService : IFavoriteRecipeService
    {
        private readonly IFavoriteRecipeRepository _repository;

        public FavoriteRecipeService(IFavoriteRecipeRepository repository)
        {
            _repository = repository;
        }
        public IQueryable<FavoriteRecipe> GetAll() => _repository.GetAll();

        public FavoriteRecipe GetById(Guid id) => _repository.GetById(id);

        public async Task<FavoriteRecipe> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public FavoriteRecipe Find(Expression<Func<FavoriteRecipe, bool>> match) => _repository.Find(match);

        public async Task<FavoriteRecipe> FindAsync(Expression<Func<FavoriteRecipe, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(FavoriteRecipe entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(FavoriteRecipe entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(FavoriteRecipe entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<FavoriteRecipe>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<FavoriteRecipe>> ListAsync(
            Expression<Func<FavoriteRecipe, bool>> filter = null,
            Func<IQueryable<FavoriteRecipe>, IOrderedQueryable<FavoriteRecipe>> orderBy = null,
            Func<IQueryable<FavoriteRecipe>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<FavoriteRecipe, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
    }
}