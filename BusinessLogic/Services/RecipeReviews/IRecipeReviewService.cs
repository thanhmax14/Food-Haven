using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Models;

namespace BusinessLogic.Services.RecipeReviewReviews
{
    public interface IRecipeReviewService
    {
        IQueryable<RecipeReview> GetAll();
        RecipeReview GetById(Guid id);
        Task<RecipeReview> GetAsyncById(Guid id);
        RecipeReview Find(Expression<Func<RecipeReview, bool>> match);
        Task<RecipeReview> FindAsync(Expression<Func<RecipeReview, bool>> match);
        Task AddAsync(RecipeReview entity);
        Task UpdateAsync(RecipeReview entity);
        Task DeleteAsync(RecipeReview entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<RecipeReview>> ListAsync();
        Task<IEnumerable<RecipeReview>> ListAsync(
            Expression<Func<RecipeReview, bool>> filter = null,
            Func<IQueryable<RecipeReview>, IOrderedQueryable<RecipeReview>> orderBy = null,
            Func<IQueryable<RecipeReview>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<RecipeReview, object>> includeProperties = null);
    }
}