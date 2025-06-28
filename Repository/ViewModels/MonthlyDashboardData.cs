using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{

    public class MonthlyDashboardData
    {
        public MonthlyDashboardSummary Summary { get; set; }
        public Dictionary<string, int> Orders { get; set; }
        public Dictionary<string, decimal> Earnings { get; set; }
        public Dictionary<string, int> Refunds { get; set; }
        public Dictionary<string, int> NewCustomers { get; set; }
    }
}
