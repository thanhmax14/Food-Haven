using AutoMapper;
using Models;
using Repository.Complaints;
using Repository.ComplaintImages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.ComplaintImages
{
    public class ComplaintImageServices:IComplaintImageServices
    {
        private readonly IComplaintImageRepository _repository;
        private readonly IMapper _mapper;

        public ComplaintImageServices(IComplaintImageRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IQueryable<ComplaintImage> GetAll() => _repository.GetAll();

        public ComplaintImage GetById(Guid id) => _repository.GetById(id);

        public async Task<ComplaintImage> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public ComplaintImage Find(Expression<Func<ComplaintImage, bool>> match) => _repository.Find(match);

        public async Task<ComplaintImage> FindAsync(Expression<Func<ComplaintImage, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(ComplaintImage entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(ComplaintImage entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(ComplaintImage entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<ComplaintImage>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<ComplaintImage>> ListAsync(
            Expression<Func<ComplaintImage, bool>> filter = null,
            Func<IQueryable<ComplaintImage>, IOrderedQueryable<ComplaintImage>> orderBy = null,
            Func<IQueryable<ComplaintImage>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ComplaintImage, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
    }
}
