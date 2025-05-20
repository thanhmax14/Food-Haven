using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class CartViewModels
    {
        public Guid CartID { get; set; }
        public Guid ProductID { get; set; }
        public Guid ProductTypeID { get; set; }
        public int quantity { get; set; } = 0;
        public string? img { get; set; }
        public string? ProductName { get; set; }
        public string? ProductTyName { get; set; }
        public float vote { get; set; }
        public decimal price { get; set; } = 0;
        public decimal Subtotal { get; set; }
        public int Stock { get; set; }
        public int Rating { get; set; } = 5;

        public string UserID { get; set; }

    }
}
