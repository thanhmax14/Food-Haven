using Models.DBContext;
using Repository.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ExpertRecipes
{
    public class ExpertRecipeRepository:BaseRepository<Models.ExpertRecipe>, IExpertRecipeRepository
    {
        public ExpertRecipeRepository(FoodHavenDbContext context):base(context)
        {
            
        }
    }
}
