using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class DiscountRequest
    {
        public string Code { get; set; }
        public decimal OrderTotal { get; set; } // Tổng đơn hàng thực tế
    }
}
