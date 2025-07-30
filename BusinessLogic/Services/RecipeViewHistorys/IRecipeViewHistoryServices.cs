using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.RecipeViewHistorys
{
    public interface IRecipeViewHistoryServices
    {
         IQueryable<RecipeViewHistory> GetAll();
        RecipeViewHistory GetById(Guid id);
        Task<RecipeViewHistory> GetAsyncById(Guid id);
        RecipeViewHistory Find(Expression<Func<RecipeViewHistory, bool>> match);
        Task<RecipeViewHistory> FindAsync(Expression<Func<RecipeViewHistory, bool>> match);
        Task AddAsync(RecipeViewHistory entity);
        Task UpdateAsync(RecipeViewHistory entity);
        Task DeleteAsync(RecipeViewHistory entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<RecipeViewHistory>> ListAsync();
        Task<IEnumerable<RecipeViewHistory>> ListAsync(
            Expression<Func<RecipeViewHistory, bool>> filter = null,
            Func<IQueryable<RecipeViewHistory>, IOrderedQueryable<RecipeViewHistory>> orderBy = null,
            Func<IQueryable<RecipeViewHistory>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<RecipeViewHistory, object>> includeProperties = null);
    }
}
