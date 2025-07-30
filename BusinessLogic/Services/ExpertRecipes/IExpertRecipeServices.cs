using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.ExpertRecipes
{
    public interface IExpertRecipeServices
    {
        IQueryable<ExpertRecipe> GetAll();
        ExpertRecipe GetById(Guid id);
        Task<ExpertRecipe> GetAsyncById(Guid id);
        ExpertRecipe Find(Expression<Func<ExpertRecipe, bool>> match);
        Task<ExpertRecipe> FindAsync(Expression<Func<ExpertRecipe, bool>> match);
        Task AddAsync(ExpertRecipe entity);
        Task UpdateAsync(ExpertRecipe entity);
        Task DeleteAsync(ExpertRecipe entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<ExpertRecipe>> ListAsync();
        Task<IEnumerable<ExpertRecipe>> ListAsync(
            Expression<Func<ExpertRecipe, bool>> filter = null,
            Func<IQueryable<ExpertRecipe>, IOrderedQueryable<ExpertRecipe>> orderBy = null,
            Func<IQueryable<ExpertRecipe>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ExpertRecipe, object>> includeProperties = null);
    }
}
