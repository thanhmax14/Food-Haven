using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Models;
using Repository.TypeOfDishRepositoties;

namespace BusinessLogic.Services.TypeOfDishServices
{
    public class TypeOfDishService : ITypeOfDishService
    {
        private readonly ITypeOfDishRepository _typeOfDishRepository;

        public TypeOfDishService(ITypeOfDishRepository typeOfDishRepository)
        {
            _typeOfDishRepository = typeOfDishRepository;
        }

        public IQueryable<TypeOfDish> GetAll() => _typeOfDishRepository.GetAll();

        public TypeOfDish GetById(Guid id) => _typeOfDishRepository.GetById(id);

        public async Task<TypeOfDish> GetAsyncById(Guid id) => await _typeOfDishRepository.GetAsyncById(id);

        public TypeOfDish Find(Expression<Func<TypeOfDish, bool>> match) => _typeOfDishRepository.Find(match);

        public async Task<TypeOfDish> FindAsync(Expression<Func<TypeOfDish, bool>> match) => await _typeOfDishRepository.FindAsync(match);

        public async Task AddAsync(TypeOfDish entity) => await _typeOfDishRepository.AddAsync(entity);

        public async Task UpdateAsync(TypeOfDish entity) => await _typeOfDishRepository.UpdateAsync(entity);

        public async Task DeleteAsync(TypeOfDish entity) => await _typeOfDishRepository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _typeOfDishRepository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _typeOfDishRepository.ExistsAsync(id);
        public int Count() => _typeOfDishRepository.Count();

        public async Task<int> CountAsync() => await _typeOfDishRepository.CountAsync();

        public async Task<IEnumerable<TypeOfDish>> ListAsync() => await _typeOfDishRepository.ListAsync();

        public async Task<IEnumerable<TypeOfDish>> ListAsync(
            Expression<Func<TypeOfDish, bool>> filter = null,
            Func<IQueryable<TypeOfDish>, IOrderedQueryable<TypeOfDish>> orderBy = null,
            Func<IQueryable<TypeOfDish>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<TypeOfDish, object>> includeProperties = null) =>
            await _typeOfDishRepository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _typeOfDishRepository.SaveChangesAsync();
    }
}