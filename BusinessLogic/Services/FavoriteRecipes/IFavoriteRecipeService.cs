using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Models;

namespace BusinessLogic.Services.FavoriteFavoriteRecipes
{
    public interface IFavoriteRecipeService
    {     IQueryable<FavoriteRecipe> GetAll();
        FavoriteRecipe GetById(Guid id);
        Task<FavoriteRecipe> GetAsyncById(Guid id);
        FavoriteRecipe Find(Expression<Func<FavoriteRecipe, bool>> match);
        Task<FavoriteRecipe> FindAsync(Expression<Func<FavoriteRecipe, bool>> match);
        Task AddAsync(FavoriteRecipe entity);
        Task UpdateAsync(FavoriteRecipe entity);
        Task DeleteAsync(FavoriteRecipe entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<FavoriteRecipe>> ListAsync();
        Task<IEnumerable<FavoriteRecipe>> ListAsync(
            Expression<Func<FavoriteRecipe, bool>> filter = null,
            Func<IQueryable<FavoriteRecipe>, IOrderedQueryable<FavoriteRecipe>> orderBy = null,
            Func<IQueryable<FavoriteRecipe>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<FavoriteRecipe, object>> includeProperties = null);
    }
}