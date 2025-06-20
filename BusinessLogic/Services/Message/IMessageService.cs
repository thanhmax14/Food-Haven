using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Message
{
    public interface IMessageService
    {
        IQueryable<Models.Message> GetAll();
        Models.Message GetById(Guid id);
        Task<Models.Message> GetAsyncById(Guid id);
        Models.Message Find(Expression<Func<Models.Message, bool>> match);
        Task<Models.Message> FindAsync(Expression<Func<Models.Message, bool>> match);
        Task AddAsync(Models.Message entity);
        Task UpdateAsync(Models.Message entity);
        Task DeleteAsync(Models.Message entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<Models.Message>> ListAsync();
        Task<IEnumerable<Models.Message>> ListAsync(
            Expression<Func<Models.Message, bool>> filter = null,
            Func<IQueryable<Models.Message>, IOrderedQueryable<Models.Message>> orderBy = null,
            Func<IQueryable<Models.Message>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Models.Message, object>> includeProperties = null);
    }
}
