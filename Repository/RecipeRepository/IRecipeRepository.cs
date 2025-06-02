using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.BaseRepository;

namespace Repository.RecipeRepository
{
    public interface IRecipeRepository : IBaseRepository<Models.Recipe>
    {
        
    }
}