using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Models;

namespace BusinessLogic.Services.IngredientTagServices
{
    public interface IIngredientTagService
    {
         IQueryable<IngredientTag> GetAll();
        IngredientTag GetById(Guid id);
        Task<IngredientTag> GetAsyncById(Guid id);
        IngredientTag Find(Expression<Func<IngredientTag, bool>> match);
        Task<IngredientTag> FindAsync(Expression<Func<IngredientTag, bool>> match);
        Task AddAsync(IngredientTag entity);
        Task UpdateAsync(IngredientTag entity);
        Task DeleteAsync(IngredientTag entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<IngredientTag>> ListAsync();
        Task<IEnumerable<IngredientTag>> ListAsync(
            Expression<Func<IngredientTag, bool>> filter = null,
            Func<IQueryable<IngredientTag>, IOrderedQueryable<IngredientTag>> orderBy = null,
            Func<IQueryable<IngredientTag>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<IngredientTag, object>> includeProperties = null);


        Task<bool> ToggleIngredientTagStatus(Guid categoryId, bool isActive);


    }
}