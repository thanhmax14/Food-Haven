using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class GetSellerOrder
    {
        public int STT { get; set; }
        public string OrderTracking { get; set; }
        public string UserName { get; set; }
        public DateTime? OrderDate { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }  
        public string StatusPayment { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public string Desctiption { get; set; }
        public string DeliveryAddress { get; set; }
    }
}
