using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class IngredientTag
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();
        [StringLength(50)]
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public ICollection<RecipeIngredientTag> RecipeIngredientTags { get; set; } = new List<RecipeIngredientTag>();
    }
}
