using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.VoucherServices
{
    public interface IVoucherServices
    {
        IQueryable<Voucher> GetAll();
        Voucher GetById(Guid id);
        Task<Voucher> GetAsyncById(Guid id);
        Voucher Find(Expression<Func<Voucher, bool>> match);
        Task<Voucher> FindAsync(Expression<Func<Voucher, bool>> match);
        Task AddAsync(Voucher entity);
        Task UpdateAsync(Voucher entity);
        Task DeleteAsync(Voucher entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<Voucher>> ListAsync();
        Task<IEnumerable<Voucher>> ListAsync(
            Expression<Func<Voucher, bool>> filter = null,
            Func<IQueryable<Voucher>, IOrderedQueryable<Voucher>> orderBy = null,
            Func<IQueryable<Voucher>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Voucher, object>> includeProperties = null);
    }
}
