using Models.DBContext;
using Repository.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Complaints
{
    public class ComplaintRepository:BaseRepository<Models.Complaint>, IComplaintRepository
    {
        public ComplaintRepository(FoodHavenDbContext context) : base(context) { }
    }
}
