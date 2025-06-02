using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.DBContext;
using Repository.BaseRepository;

namespace Repository.RecipeRepository
{
    public class RecipeRepository : BaseRepository<Models.Recipe>, IRecipeRepository
    {
       public RecipeRepository(FoodHavenDbContext context) : base(context) { }
    }
}