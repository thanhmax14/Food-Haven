using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Repository.BaseRepository;

namespace Repository.FavoriteRecipes
{
    public interface IFavoriteRecipeRepository : IBaseRepository<FavoriteRecipe>
    {

    }
}