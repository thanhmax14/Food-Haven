using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Models;

namespace BusinessLogic.Services.RecipeServices
{
    public interface IRecipeService
    {
        IQueryable<Recipe> GetAll();
        Recipe GetById(Guid id);
        Task<Recipe> GetAsyncById(Guid id);
        Recipe Find(Expression<Func<Recipe, bool>> match);
        Task<Recipe> FindAsync(Expression<Func<Recipe, bool>> match);
        Task AddAsync(Recipe entity);
        Task UpdateAsync(Recipe entity);
        Task DeleteAsync(Recipe entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<Recipe>> ListAsync();
        Task<IEnumerable<Recipe>> ListAsync(
            Expression<Func<Recipe, bool>> filter = null,
            Func<IQueryable<Recipe>, IOrderedQueryable<Recipe>> orderBy = null,
            Func<IQueryable<Recipe>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Recipe, object>> includeProperties = null);

    }
}