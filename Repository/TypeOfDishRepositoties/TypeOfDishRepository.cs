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
        public TypeOfDishRepository(FoodHavenDbContext context) : base(context)
        {
            _context = context;
        }

        private readonly FoodHavenDbContext _context;


        public async Task<bool> ToggletypeOfDishesIdTagStatus(Guid typeOfDishesId, bool isActive)
        {
            var typeOfDishes = await _context.TypeOfDish.FindAsync(typeOfDishesId);
            if (typeOfDishes == null) return false;

            typeOfDishes.IsActive = isActive;
            typeOfDishes.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

    }


}