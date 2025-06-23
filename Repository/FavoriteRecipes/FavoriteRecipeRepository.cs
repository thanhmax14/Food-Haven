using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Models.DBContext;
using Repository.BaseRepository;

namespace Repository.FavoriteRecipes
{
    public class FavoriteRecipeRepository : BaseRepository<FavoriteRecipe>, IFavoriteRecipeRepository
    {
        public FavoriteRecipeRepository(FoodHavenDbContext context) : base(context) { }

    }
}