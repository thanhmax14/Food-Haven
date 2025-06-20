using Models.DBContext;
using Repository.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MessageImages
{
    public class MessageImageRepository:BaseRepository<Models.MessageImage>,IMessageImageRepository
    {
        public MessageImageRepository(FoodHavenDbContext foodHavenDbContext) : base(foodHavenDbContext) { }
    }
}
