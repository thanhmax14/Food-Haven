using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class RecipeIngredientTag
    {
        public Guid RecipeID { get; set; }
        public Recipe Recipe { get; set; }

        public Guid IngredientTagID { get; set; }
        public IngredientTag IngredientTag { get; set; }
    }
}
