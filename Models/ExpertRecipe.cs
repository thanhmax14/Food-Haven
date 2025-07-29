using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ExpertRecipe
    {
        public Guid ID { get; set; }=Guid.NewGuid();

        public string Title { get; set; }
        public string Ingredients { get; set; } // JSON string or comma-separated
        public string Directions { get; set; }
        public string Link { get; set; }
        public string Source { get; set; }
        public string NER { get; set; }

        // Navigation
        public ICollection<RecipeViewHistory> ViewHistories { get; set; }
    }
}
