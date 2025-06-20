using AutoMapper;
using Models;
using Repository.MessageImages;
using Repository.Messages;
using Repository.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Message
{
    public class MessageServices:IMessageService
    {
        private readonly IMessageRepository _repository;
        private readonly IMapper _mapper;

        public MessageServices(IMessageRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IQueryable<Models.Message> GetAll() => _repository.GetAll();

        public Models.Message GetById(Guid id) => _repository.GetById(id);

        public async Task<Models.Message> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public Models.Message Find(Expression<Func<Models.Message, bool>> match) => _repository.Find(match);

        public async Task<Models.Message> FindAsync(Expression<Func<Models.Message, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(Models.Message entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(Models.Message entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(Models.Message entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<Models.Message>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<Models.Message>> ListAsync(
            Expression<Func<Models.Message, bool>> filter = null,
            Func<IQueryable<Models.Message>, IOrderedQueryable<Models.Message>> orderBy = null,
            Func<IQueryable<Models.Message>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Models.Message, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
    }
}
