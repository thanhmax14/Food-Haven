using Microsoft.EntityFrameworkCore;
using Models;
using Models.DBContext;
using NuGet.Protocol.Core.Types;
using Repository.IngredientTagRepositorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessLogic.Services.IngredientTagServices
{
    public class IngredientTagService : IIngredientTagService
    {
        private readonly IIngredientTagRepository _ingredientTagRepository;
        private readonly IngredientTagRepository _ingredientTagRepository1;
        private readonly FoodHavenDbContext _context;
        public IngredientTagService(IIngredientTagRepository ingredientTagRepository, IngredientTagRepository ingredientTagRepository1, FoodHavenDbContext context)
        {
            _ingredientTagRepository1 = ingredientTagRepository1;
            _ingredientTagRepository = ingredientTagRepository;
            _context = context;
        }
        public IQueryable<IngredientTag> GetAll() => _ingredientTagRepository.GetAll();

        public IngredientTag GetById(Guid id) => _ingredientTagRepository.GetById(id);

        public async Task<IngredientTag> GetAsyncById(Guid id) => await _ingredientTagRepository.GetAsyncById(id);

        public IngredientTag Find(Expression<Func<IngredientTag, bool>> match) => _ingredientTagRepository.Find(match);

        public async Task<IngredientTag> FindAsync(Expression<Func<IngredientTag, bool>> match) => await _ingredientTagRepository.FindAsync(match);

        public async Task AddAsync(IngredientTag entity) => await _ingredientTagRepository.AddAsync(entity);

        public async Task UpdateAsync(IngredientTag entity) => await _ingredientTagRepository.UpdateAsync(entity);

        public async Task DeleteAsync(IngredientTag entity) => await _ingredientTagRepository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _ingredientTagRepository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _ingredientTagRepository.ExistsAsync(id);
        public int Count() => _ingredientTagRepository.Count();

        public async Task<int> CountAsync() => await _ingredientTagRepository.CountAsync();

        public async Task<IEnumerable<IngredientTag>> ListAsync() => await _ingredientTagRepository.ListAsync();

        public async Task<IEnumerable<IngredientTag>> ListAsync(
            Expression<Func<IngredientTag, bool>> filter = null,
            Func<IQueryable<IngredientTag>, IOrderedQueryable<IngredientTag>> orderBy = null,
            Func<IQueryable<IngredientTag>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<IngredientTag, object>> includeProperties = null) =>
            await _ingredientTagRepository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _ingredientTagRepository.SaveChangesAsync();

        public async Task<bool> ToggleIngredientTagStatus(Guid IngredientTagId, bool isActive)
        {
            return await _ingredientTagRepository1.ToggleIngredientTagStatus(IngredientTagId, isActive);
        }
        public async Task<bool> ExistsAsync(string name)
        {
            return await _context.IngredientTag
                .AnyAsync(x => x.Name.ToLower() == name.Trim().ToLower());
        }
        public async Task<bool> ExistsAsync(string name, Guid excludeId)
        {
            return await _context.IngredientTag
                .AnyAsync(x => x.Name.ToLower() == name.Trim().ToLower() && x.ID != excludeId);
        }




    }
}