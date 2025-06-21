using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.DBContext;
using Repository.BaseRepository;

namespace Repository.RecipeReviews
{
    public class RecipeReviewsRepository : BaseRepository<Models.RecipeReview>, IRecipeReviewsRepository
    {
        public RecipeReviewsRepository(FoodHavenDbContext context) : base(context) { }

    }
}