using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class StoreDetails
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();

        [ForeignKey("AppUser")]
        [StringLength(450)]
        public string? UserID { get; set; }
        [StringLength(250)]
        public string Name { get; set; } = default!;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public string? LongDescriptions { get; set; }
        [StringLength(500)]
        public string? ShortDescriptions { get; set; }
        [StringLength(500)]
        public string? Address { get; set; }
        [StringLength(12)]
        public string? Phone { get; set; }
        public string? ImageUrl { get; set; }
        [StringLength(10)]
        public string? Status { get; set; }
        public bool IsActive { get; set; } = false;

        public string? RejectNote { get; set; }
        public virtual AppUser AppUser { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Voucher> Vouchers { get; set; }
    }
}
