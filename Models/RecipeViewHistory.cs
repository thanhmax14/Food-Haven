using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class RecipeViewHistory
    {
        public Guid ID { get; set; }= Guid.NewGuid();

        [ForeignKey("AppUser")]
        [StringLength(450)]
        public string UserID { get; set; }
        public virtual AppUser AppUser { get; set; }
        [ForeignKey("ExpertRecipe")]
        public Guid ExpertRecipeId { get; set; }
        public virtual ExpertRecipe ExpertRecipe { get; set; }

        public DateTime ViewedAt { get; set; } = DateTime.Now;
        [StringLength(1000)]
        public string MatchedIngredients { get; set; }
    }
}
