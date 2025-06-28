using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Categories
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();
        public string? ImageUrl { get; set; }
        [StringLength(10)]
        public string DisplayOrder { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [Range(0, 100)]
        public float Commission { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Recipe> Recipes { get; set; }
    }
}
