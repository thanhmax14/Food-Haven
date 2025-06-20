using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Message
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(450)]
        public string FromUserId { get; set; }

        [Required]
        [StringLength(450)]
        public string ToUserId { get; set; }
        [StringLength(500)]
        public string? MessageText { get; set; }

        public DateTime SentAt { get; set; } = DateTime.Now;

        // Tự liên kết (reply)
        public Guid? RepliedToMessageId { get; set; }

        public bool HasDropDown { get; set; } = true;
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        // Navigation
        [ForeignKey(nameof(FromUserId))]
        public virtual AppUser FromUser { get; set; }

        [ForeignKey(nameof(ToUserId))]
        public virtual AppUser ToUser { get; set; }

        [ForeignKey(nameof(RepliedToMessageId))]
        public virtual Message RepliedTo { get; set; }

        public virtual ICollection<Message> Replies { get; set; } = new List<Message>();
        public virtual ICollection<MessageImage> Images { get; set; } = new List<MessageImage>();
    }
}
