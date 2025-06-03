using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ComplaintImage
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();

        [Required]
        public string ImageUrl { get; set; }

        [ForeignKey("Complaint")]
        public Guid ComplaintID { get; set; }
        public virtual Complaint Complaint { get; set; }
    }
}
