using AutoMapper;
using Models;
using Repository.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.VoucherServices
{
    public class VoucherService  : IVoucherServices
    {
        private readonly IVouchersRepository _repository;
        private readonly IMapper _mapper;

        public VoucherService(IVouchersRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IQueryable<Voucher> GetAll() => _repository.GetAll();

        public Voucher GetById(Guid id) => _repository.GetById(id);

        public async Task<Voucher> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public Voucher Find(Expression<Func<Voucher, bool>> match) => _repository.Find(match);

        public async Task<Voucher> FindAsync(Expression<Func<Voucher, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(Voucher entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(Voucher entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(Voucher entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<Voucher>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<Voucher>> ListAsync(
            Expression<Func<Voucher, bool>> filter = null,
            Func<IQueryable<Voucher>, IOrderedQueryable<Voucher>> orderBy = null,
            Func<IQueryable<Voucher>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Voucher, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
    }
}
