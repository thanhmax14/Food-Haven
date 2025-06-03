using AutoMapper;
using Models;
using Repository.Complaints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Complaints
{
    public class ComplaintServices:IComplaintServices
    {
        private readonly IComplaintRepository _repository;
        private readonly IMapper _mapper;

        public ComplaintServices(IComplaintRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IQueryable<Complaint> GetAll() => _repository.GetAll();

        public Complaint GetById(Guid id) => _repository.GetById(id);

        public async Task<Complaint> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public Complaint Find(Expression<Func<Complaint, bool>> match) => _repository.Find(match);

        public async Task<Complaint> FindAsync(Expression<Func<Complaint, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(Complaint entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(Complaint entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(Complaint entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<Complaint>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<Complaint>> ListAsync(
            Expression<Func<Complaint, bool>> filter = null,
            Func<IQueryable<Complaint>, IOrderedQueryable<Complaint>> orderBy = null,
            Func<IQueryable<Complaint>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Complaint, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
    }
}
