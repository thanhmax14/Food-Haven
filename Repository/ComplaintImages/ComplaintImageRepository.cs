using Models.DBContext;
using Repository.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ComplaintImages
{
    public class ComplaintImageRepository:BaseRepository<Models.ComplaintImage>, IComplaintImageRepository
    {
        public ComplaintImageRepository(FoodHavenDbContext context) : base(context) { }
    }
}
