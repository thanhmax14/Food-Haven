using AutoMapper;
using Models;
using Repository.ExpertRecipes;
using Repository.RecipeViewHistorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.ExpertRecipes
{
    public class ExpertRecipeServices: IExpertRecipeServices
    {
        private readonly IExpertRecipeRepository _repository;
        private readonly IMapper _mapper;

        public ExpertRecipeServices(IExpertRecipeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IQueryable<ExpertRecipe> GetAll() => _repository.GetAll();

        public ExpertRecipe GetById(Guid id) => _repository.GetById(id);

        public async Task<ExpertRecipe> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public ExpertRecipe Find(Expression<Func<ExpertRecipe, bool>> match) => _repository.Find(match);

        public async Task<ExpertRecipe> FindAsync(Expression<Func<ExpertRecipe, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(ExpertRecipe entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(ExpertRecipe entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(ExpertRecipe entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<ExpertRecipe>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<ExpertRecipe>> ListAsync(
            Expression<Func<ExpertRecipe, bool>> filter = null,
            Func<IQueryable<ExpertRecipe>, IOrderedQueryable<ExpertRecipe>> orderBy = null,
            Func<IQueryable<ExpertRecipe>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ExpertRecipe, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
    }
}
