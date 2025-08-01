using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ExpertRecipe
    {
        public Guid ID { get; set; }=Guid.NewGuid();
        [StringLength(200)]
        public string Title { get; set; }
        [StringLength(1500)]
        public string Ingredients { get; set; } // JSON string or comma-separated
        [StringLength(2500)]
        public string Directions { get; set; }
        [StringLength(100)]
        public string Link { get; set; }
        [StringLength(100)]
        public string Source { get; set; }
        [StringLength(1000)]
        public string NER { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        // Navigation
        public ICollection<RecipeViewHistory> ViewHistories { get; set; }
    }
}
