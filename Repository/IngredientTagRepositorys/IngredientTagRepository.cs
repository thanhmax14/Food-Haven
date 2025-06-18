using Microsoft.EntityFrameworkCore;
using Models;
using Models.DBContext;
using Repository.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.IngredientTagRepositorys
{
    public class IngredientTagRepository : BaseRepository<Models.IngredientTag>, IIngredientTagRepository
    {
        public IngredientTagRepository(FoodHavenDbContext context) : base(context)
        {
            _context = context;
        }

        private readonly FoodHavenDbContext _context;


        public async Task<bool> ToggleIngredientTagStatus(Guid IngredientTagId, bool isActive)
        {
            var IngredientTag = await _context.IngredientTag.FindAsync(IngredientTagId);
            if (IngredientTag == null) return false;

            IngredientTag.IsActive = isActive;
            IngredientTag.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
      


    }
}