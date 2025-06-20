using AutoMapper;
using Models;
using Repository.MessageImages;
using Repository.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.MessageImages
{
    public class MessageImageService:IMessageImageService
    {
        private readonly IMessageImageRepository _repository;
        private readonly IMapper _mapper;

        public MessageImageService(IMessageImageRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IQueryable<Models.MessageImage> GetAll() => _repository.GetAll();

        public Models.MessageImage GetById(Guid id) => _repository.GetById(id);

        public async Task<Models.MessageImage> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public Models.MessageImage Find(Expression<Func<Models.MessageImage, bool>> match) => _repository.Find(match);

        public async Task<Models.MessageImage> FindAsync(Expression<Func<Models.MessageImage, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(Models.MessageImage entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(Models.MessageImage entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(Models.MessageImage entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<Models.MessageImage>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<Models.MessageImage>> ListAsync(
            Expression<Func<Models.MessageImage, bool>> filter = null,
            Func<IQueryable<Models.MessageImage>, IOrderedQueryable<Models.MessageImage>> orderBy = null,
            Func<IQueryable<Models.MessageImage>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Models.MessageImage, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
    }
}
