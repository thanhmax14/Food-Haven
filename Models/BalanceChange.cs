using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class BalanceChange
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();
        public decimal MoneyBeforeChange { get; set; } = 0;
        public decimal MoneyChange { get; set; } = 0;
        public decimal MoneyAfterChange { get; set; } = 0;
        public DateTime? StartTime { get; set; }
        public DateTime? DueTime { get; set; }
        [StringLength(200)]
        public string? Description { get; set; }
        [ForeignKey("AppUser")]
        [StringLength(450)]
        public string UserID { get; set; }
        [StringLength(20)]
        public string Status { get; set; }
        [StringLength(20)]
        public string Method { get; set; }
        public bool Display { get; set; } = true;
        public int? OrderCode { get; set; }
        public bool IsComplete { get; set; } = false;
        public bool CheckDone { get; set; } = false;
        [StringLength(20)]
        public string? RejectNote { get; set; }
        public virtual AppUser AppUser { get; set; }
    }
}
