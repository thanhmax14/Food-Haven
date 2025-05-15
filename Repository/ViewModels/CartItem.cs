using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class CartItem
    {
         public Guid ProductTypeId { get; set; } 
        public Guid ProductID { get; set; }
        public int quantity { get; set; } = 0;

    }
}
