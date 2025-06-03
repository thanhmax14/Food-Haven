using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.DBContext;
using Repository.BaseRepository;

namespace Repository.IngredientTagRepositorys
{
    public class IngredientTagRepository : BaseRepository<Models.IngredientTag>, IIngredientTagRepository
    {
        public IngredientTagRepository(FoodHavenDbContext context) : base(context) { }
    }
}