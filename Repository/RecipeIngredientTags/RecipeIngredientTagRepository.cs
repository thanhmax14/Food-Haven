using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.DBContext;
using Repository.BaseRepository;

namespace Repository.RecipeIngredientTags
{
    public class RecipeIngredientTagRepository : BaseRepository<Models.RecipeIngredientTag>, IRecipeIngredientTagRepository
    {
        public RecipeIngredientTagRepository(FoodHavenDbContext context) : base(context) { }
    }



}