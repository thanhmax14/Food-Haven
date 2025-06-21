using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class RecipeReviewViewModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        [StringLength(500)]
        public string Comment { get; set; }
        [StringLength(500)]
        public string? Reply { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ReplyDate { get; set; }
        public int Rating { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public string UserID { get; set; }
        public Guid RecipeID { get; set; }

    }
}