using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BusinessLogic.Services.RecipeIngredientTagIngredientTagServices;
using Models;
using Repository.RecipeIngredientTags;

namespace BusinessLogic.Services.RecipeIngredientTagIngredientTagIngredientTagServices
{
    public class RecipeIngredientTagIngredientTagIngredientTagSerivce : IRecipeIngredientTagIngredientTagSerivce
    {
        private readonly IRecipeIngredientTagRepository __recipeIngredientTagRepository;

        public RecipeIngredientTagIngredientTagIngredientTagSerivce(IRecipeIngredientTagRepository recipeIngredientTagRepository)
        {
            __recipeIngredientTagRepository = recipeIngredientTagRepository;
        }

        public IQueryable<RecipeIngredientTag> GetAll() => __recipeIngredientTagRepository.GetAll();

        public RecipeIngredientTag GetById(Guid id) => __recipeIngredientTagRepository.GetById(id);

        public async Task<RecipeIngredientTag> GetAsyncById(Guid id) => await __recipeIngredientTagRepository.GetAsyncById(id);

        public RecipeIngredientTag Find(Expression<Func<RecipeIngredientTag, bool>> match) => __recipeIngredientTagRepository.Find(match);

        public async Task<RecipeIngredientTag> FindAsync(Expression<Func<RecipeIngredientTag, bool>> match) => await __recipeIngredientTagRepository.FindAsync(match);

        public async Task AddAsync(RecipeIngredientTag entity) => await __recipeIngredientTagRepository.AddAsync(entity);

        public async Task UpdateAsync(RecipeIngredientTag entity) => await __recipeIngredientTagRepository.UpdateAsync(entity);

        public async Task DeleteAsync(RecipeIngredientTag entity) => await __recipeIngredientTagRepository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await __recipeIngredientTagRepository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await __recipeIngredientTagRepository.ExistsAsync(id);
        public int Count() => __recipeIngredientTagRepository.Count();

        public async Task<int> CountAsync() => await __recipeIngredientTagRepository.CountAsync();

        public async Task<IEnumerable<RecipeIngredientTag>> ListAsync() => await __recipeIngredientTagRepository.ListAsync();

        public async Task<IEnumerable<RecipeIngredientTag>> ListAsync(
            Expression<Func<RecipeIngredientTag, bool>> filter = null,
            Func<IQueryable<RecipeIngredientTag>, IOrderedQueryable<RecipeIngredientTag>> orderBy = null,
            Func<IQueryable<RecipeIngredientTag>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<RecipeIngredientTag, object>> includeProperties = null) =>
            await __recipeIngredientTagRepository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await __recipeIngredientTagRepository.SaveChangesAsync();

    }
}