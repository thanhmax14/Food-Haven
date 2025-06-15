using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Order
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();
        [StringLength(10)]
        public string OrderTracking { get; set; } ="";
        [StringLength(10)]
        public string OrderCode { get; set; }
        [StringLength(10)]
        public string Status { get; set; }
        public bool IsActive { get; set; } = true;
        public int Quantity { get; set; } = 0;
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public string Description { get; set; } = "";
        [StringLength(500)]
        public string Note { get; set; } = "";
        public string DeliveryAddress { get; set; } = "";
        [ForeignKey("AppUser")]
        [StringLength(450)]
        public string UserID { get; set; }
        public virtual AppUser AppUser { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        [ForeignKey("Voucher")]
        public Guid? VoucherID { get; set; }
        public virtual Voucher? Voucher { get; set; }
    }
}
