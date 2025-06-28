using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class MonthlyDashboardSummary
    {
        public int Orders { get; set; }
        public decimal Earnings { get; set; }
        public int Refunds { get; set; }
        public int NewCustomers { get; set; }
    }
}
