using Models.DBContext;
using Repository.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Messages
{
    public class MessageRepository:BaseRepository<Models.Message>,IMessageRepository
    {
        public MessageRepository(FoodHavenDbContext context) : base(context) { }

    }
}
