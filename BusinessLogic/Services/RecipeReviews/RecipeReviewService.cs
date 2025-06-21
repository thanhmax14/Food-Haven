using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Models;
using Repository.RecipeReviews;

namespace BusinessLogic.Services.RecipeReviewReviews
{
    public class RecipeReviewService : IRecipeReviewService
    {
        private readonly IRecipeReviewsRepository _recipeReviewsRepository;

        public RecipeReviewService(IRecipeReviewsRepository recipeReviewsRepository)
        {
            _recipeReviewsRepository = recipeReviewsRepository;
        }
        public IQueryable<RecipeReview> GetAll() => _recipeReviewsRepository.GetAll();

        public RecipeReview GetById(Guid id) => _recipeReviewsRepository.GetById(id);

        public async Task<RecipeReview> GetAsyncById(Guid id) => await _recipeReviewsRepository.GetAsyncById(id);

        public RecipeReview Find(Expression<Func<RecipeReview, bool>> match) => _recipeReviewsRepository.Find(match);

        public async Task<RecipeReview> FindAsync(Expression<Func<RecipeReview, bool>> match) => await _recipeReviewsRepository.FindAsync(match);

        public async Task AddAsync(RecipeReview entity) => await _recipeReviewsRepository.AddAsync(entity);

        public async Task UpdateAsync(RecipeReview entity) => await _recipeReviewsRepository.UpdateAsync(entity);

        public async Task DeleteAsync(RecipeReview entity) => await _recipeReviewsRepository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _recipeReviewsRepository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _recipeReviewsRepository.ExistsAsync(id);
        public int Count() => _recipeReviewsRepository.Count();

        public async Task<int> CountAsync() => await _recipeReviewsRepository.CountAsync();

        public async Task<IEnumerable<RecipeReview>> ListAsync() => await _recipeReviewsRepository.ListAsync();

        public async Task<IEnumerable<RecipeReview>> ListAsync(
            Expression<Func<RecipeReview, bool>> filter = null,
            Func<IQueryable<RecipeReview>, IOrderedQueryable<RecipeReview>> orderBy = null,
            Func<IQueryable<RecipeReview>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<RecipeReview, object>> includeProperties = null) =>
            await _recipeReviewsRepository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _recipeReviewsRepository.SaveChangesAsync();
    }
}