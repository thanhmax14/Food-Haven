using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Models;
using Repository.RecipeRepository;

namespace BusinessLogic.Services.RecipeServices
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;

        public RecipeService(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }
        public IQueryable<Recipe> GetAll() => _recipeRepository.GetAll();

        public Recipe GetById(Guid id) => _recipeRepository.GetById(id);

        public async Task<Recipe> GetAsyncById(Guid id) => await _recipeRepository.GetAsyncById(id);

        public Recipe Find(Expression<Func<Recipe, bool>> match) => _recipeRepository.Find(match);

        public async Task<Recipe> FindAsync(Expression<Func<Recipe, bool>> match) => await _recipeRepository.FindAsync(match);

        public async Task AddAsync(Recipe entity) => await _recipeRepository.AddAsync(entity);

        public async Task UpdateAsync(Recipe entity) => await _recipeRepository.UpdateAsync(entity);

        public async Task DeleteAsync(Recipe entity) => await _recipeRepository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _recipeRepository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _recipeRepository.ExistsAsync(id);
        public int Count() => _recipeRepository.Count();

        public async Task<int> CountAsync() => await _recipeRepository.CountAsync();

        public async Task<IEnumerable<Recipe>> ListAsync() => await _recipeRepository.ListAsync();

        public async Task<IEnumerable<Recipe>> ListAsync(
            Expression<Func<Recipe, bool>> filter = null,
            Func<IQueryable<Recipe>, IOrderedQueryable<Recipe>> orderBy = null,
            Func<IQueryable<Recipe>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Recipe, object>> includeProperties = null) =>
            await _recipeRepository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _recipeRepository.SaveChangesAsync();
    }
}