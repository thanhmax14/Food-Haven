using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.DBContext;
using Repository.BaseRepository;

namespace Repository.StoreReport
{
    public class StoreReportRepository : BaseRepository<Models.StoreReport>, IStoreReportRepository
    {
        public StoreReportRepository(FoodHavenDbContext context) : base(context) { }

    }
}