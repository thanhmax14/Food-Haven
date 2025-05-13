﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class StoreDetails
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();

        [ForeignKey("AppUser")]
        public string? UserID { get; set; }

        public string Name { get; set; } = default!;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public string? LongDescriptions { get; set; }
        public string? ShortDescriptions { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? ImageUrl { get; set; }
        public string? Status { get; set; }
        public bool IsActive { get; set; } = false;


        public virtual AppUser AppUser { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
