using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Voucher
    {
        [Key]
        public Guid ID { get; set; }
        [StringLength(20)]
        public string Code { get; set; }
        public decimal DiscountAmount { get; set; }
        [StringLength(10)]
        public string DiscountType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int MaxUsage { get; set; }
        public int CurrentUsage { get; set; }
        public decimal MinOrderValue { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsGlobal { get; set; } = false;
        public int? UsagePerUser { get; set; }
        [StringLength(300)]
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public ICollection<Order> Orders { get; set; }
        [ForeignKey("StoreDetails")]
        public Guid? StoreID { get; set; }
        public StoreDetails StoreDetails { get; set; }


    }
}
