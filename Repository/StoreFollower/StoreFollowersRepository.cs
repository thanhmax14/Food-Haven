using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.DBContext;
using Repository.BaseRepository;

namespace Repository.StoreFollower
{
    public class StoreFollowersRepository : BaseRepository<Models.StoreFollower>, IStoreFollowersRepository
    {
        public StoreFollowersRepository(FoodHavenDbContext dbContext) : base(dbContext)
        {
        }
    }
}