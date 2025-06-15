using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class OrderDetail
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();
        public decimal ProductPrice { get; set; } = 0;
        public decimal TotalPrice { get; set; } = 0;
        public int Quantity { get; set; } = 0;
        [StringLength(10)]
        public string? Status { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsFeedback { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        [ForeignKey("Order")]
        public Guid OrderID { get; set; }
        public virtual Order Order { get; set; }
        [ForeignKey("ProductTypes")]
        public Guid ProductTypesID { get; set; }
        public virtual ProductTypes ProductTypes { get; set; }
        public ICollection<Complaint> Complaints { get; set; }

    }
}
