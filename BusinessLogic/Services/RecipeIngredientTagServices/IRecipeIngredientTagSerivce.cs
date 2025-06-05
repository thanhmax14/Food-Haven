using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Models;

namespace BusinessLogic.Services.RecipeIngredientTagIngredientTagServices
{
    public interface IRecipeIngredientTagIngredientTagSerivce
    {
        IQueryable<RecipeIngredientTag> GetAll();
        RecipeIngredientTag GetById(Guid id);
        Task<RecipeIngredientTag> GetAsyncById(Guid id);
        RecipeIngredientTag Find(Expression<Func<RecipeIngredientTag, bool>> match);
        Task<RecipeIngredientTag> FindAsync(Expression<Func<RecipeIngredientTag, bool>> match);
        Task AddAsync(RecipeIngredientTag entity);
        Task UpdateAsync(RecipeIngredientTag entity);
        Task DeleteAsync(RecipeIngredientTag entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<RecipeIngredientTag>> ListAsync();
        Task<IEnumerable<RecipeIngredientTag>> ListAsync(
            Expression<Func<RecipeIngredientTag, bool>> filter = null,
            Func<IQueryable<RecipeIngredientTag>, IOrderedQueryable<RecipeIngredientTag>> orderBy = null,
            Func<IQueryable<RecipeIngredientTag>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<RecipeIngredientTag, object>> includeProperties = null);
    }
}