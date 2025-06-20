using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.MessageImages
{
    public interface IMessageImageService
    {
        IQueryable<Models.MessageImage> GetAll();
        Models.MessageImage GetById(Guid id);
        Task<Models.MessageImage> GetAsyncById(Guid id);
        Models.MessageImage Find(Expression<Func<Models.MessageImage, bool>> match);
        Task<Models.MessageImage> FindAsync(Expression<Func<Models.MessageImage, bool>> match);
        Task AddAsync(Models.MessageImage entity);
        Task UpdateAsync(Models.MessageImage entity);
        Task DeleteAsync(Models.MessageImage entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<Models.MessageImage>> ListAsync();
        Task<IEnumerable<Models.MessageImage>> ListAsync(
            Expression<Func<Models.MessageImage, bool>> filter = null,
            Func<IQueryable<Models.MessageImage>, IOrderedQueryable<Models.MessageImage>> orderBy = null,
            Func<IQueryable<Models.MessageImage>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Models.MessageImage, object>> includeProperties = null);
    }
}
