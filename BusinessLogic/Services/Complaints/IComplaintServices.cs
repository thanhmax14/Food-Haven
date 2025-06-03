using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Complaints
{
    public interface IComplaintServices
    {
        IQueryable<Complaint> GetAll();
        Complaint GetById(Guid id);
        Task<Complaint> GetAsyncById(Guid id);
        Complaint Find(Expression<Func<Complaint, bool>> match);
        Task<Complaint> FindAsync(Expression<Func<Complaint, bool>> match);
        Task AddAsync(Complaint entity);
        Task UpdateAsync(Complaint entity);
        Task DeleteAsync(Complaint entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<Complaint>> ListAsync();
        Task<IEnumerable<Complaint>> ListAsync(
            Expression<Func<Complaint, bool>> filter = null,
            Func<IQueryable<Complaint>, IOrderedQueryable<Complaint>> orderBy = null,
            Func<IQueryable<Complaint>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Complaint, object>> includeProperties = null);
    }
}
