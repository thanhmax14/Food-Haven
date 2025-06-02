using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.DBContext;
using Repository.BaseRepository;

namespace Repository.TypeOfDishRepositoties
{
    public class TypeOfDishRepository : BaseRepository<Models.TypeOfDish>, ITypeOfDishRepository
    {
          public TypeOfDishRepository(FoodHavenDbContext context) : base(context) { }
    }
}