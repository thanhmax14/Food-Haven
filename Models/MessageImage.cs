using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class MessageImage
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();

        [Required]
        public Guid MessageID { get; set; }

        [Required]
        [StringLength(255)]
        public string ImageUrl { get; set; }

        [ForeignKey(nameof(MessageID))]
        public virtual Message Message { get; set; }
    }
}
