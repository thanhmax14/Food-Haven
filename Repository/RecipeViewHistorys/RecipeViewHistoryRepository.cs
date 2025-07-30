using Models.DBContext;
using Repository.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.RecipeViewHistorys
{
    public class RecipeViewHistoryRepository:BaseRepository<Models.RecipeViewHistory>, IRecipeViewHistoryRepository
    {
        public RecipeViewHistoryRepository(FoodHavenDbContext context): base(context)
        {
            
        }
    }
}
