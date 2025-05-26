using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class OrderViewModel
    {

        public int stt { get; set; }
        public string OrderTracking { get; set; }
        public int  Quantity { get; set; }
        public string PaymentMethod { get; set; }
        public string StatusPayment { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public decimal Total { get; set; }
        public Guid OrderId { get; set; }
        public string  Desctiption { get; set; }
        public string DeliveryAddress { get; set; }


    }
}
