using Models;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessLogic.Services.TypeOfDishServices
{
    public interface ITypeOfDishService
    {
        IQueryable<TypeOfDish> GetAll();
        TypeOfDish GetById(Guid id);
        Task<TypeOfDish> GetAsyncById(Guid id);
        TypeOfDish Find(Expression<Func<TypeOfDish, bool>> match);
        Task<TypeOfDish> FindAsync(Expression<Func<TypeOfDish, bool>> match);
        Task AddAsync(TypeOfDish entity);
        Task UpdateAsync(TypeOfDish entity);
        Task DeleteAsync(TypeOfDish entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<TypeOfDish>> ListAsync();
        Task<IEnumerable<TypeOfDish>> ListAsync(
            Expression<Func<TypeOfDish, bool>> filter = null,
            Func<IQueryable<TypeOfDish>, IOrderedQueryable<TypeOfDish>> orderBy = null,
            Func<IQueryable<TypeOfDish>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<TypeOfDish, object>> includeProperties = null);
    }
}