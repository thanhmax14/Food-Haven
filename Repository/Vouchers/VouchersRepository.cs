using Models.DBContext;
using Repository.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Vouchers
{
    public class VouchersRepository:BaseRepository<Models.Voucher>, IVouchersRepository
    {
        public VouchersRepository(FoodHavenDbContext context) : base(context) { }
    }
}
