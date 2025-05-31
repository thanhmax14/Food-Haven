using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Recipe
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string CookingStep { get; set; }
        public string ShortDescriptions { get; set; }
        public string PreparationTime { get; set; }
        public string CookTime { get; set; }
        public string TotalTime { get; set; }
        public string DifficultyLevel { get; set; }
        public string Ingredient { get; set; }
        public string Servings { get; set; }
        public int RecipesMadeItCount { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; } = false;

        [ForeignKey("Categories")]
        public Guid CateID { get; set; }
        public virtual Categories Categories { get; set; }

        [ForeignKey("AppUser")]
        public string UserID { get; set; }
        public virtual AppUser AppUser { get; set; }

        [ForeignKey("TypeOfDish")]
        public Guid TypeOfDishID { get; set; }
        public virtual TypeOfDish TypeOfDish { get; set; }

        // ✅ Many-to-Many with IngredientTag
        public ICollection<IngredientTag> IngredientTags { get; set; } = new List<IngredientTag>();

        public ICollection<FavoriteRecipe> FavoriteRecipes { get; set; }
        public ICollection<RecipeReview> RecipeReviews { get; set; }
    }
}
