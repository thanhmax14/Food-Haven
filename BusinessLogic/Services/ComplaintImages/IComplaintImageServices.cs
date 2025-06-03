using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.ComplaintImages
{
    public interface IComplaintImageServices
    {
        IQueryable<ComplaintImage> GetAll();
        ComplaintImage GetById(Guid id);
        Task<ComplaintImage> GetAsyncById(Guid id);
        ComplaintImage Find(Expression<Func<ComplaintImage, bool>> match);
        Task<ComplaintImage> FindAsync(Expression<Func<ComplaintImage, bool>> match);
        Task AddAsync(ComplaintImage entity);
        Task UpdateAsync(ComplaintImage entity);
        Task DeleteAsync(ComplaintImage entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<ComplaintImage>> ListAsync();
        Task<IEnumerable<ComplaintImage>> ListAsync(
            Expression<Func<ComplaintImage, bool>> filter = null,
            Func<IQueryable<ComplaintImage>, IOrderedQueryable<ComplaintImage>> orderBy = null,
            Func<IQueryable<ComplaintImage>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ComplaintImage, object>> includeProperties = null);
    }
}
