using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;

namespace Repository.ViewModels
{
    public class RecipeViewModels
    {
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
        public Guid CateID { get; set; }
        public Guid IngredientTagID { get; set; }
        public string ThumbnailImage { get; set; }
        public string TypeOfDishName { get; set; }

        public List<Categories> Categories { get; set; } = new List<Categories>();

        public string UserID { get; set; }
        public Guid TypeOfDishID { get; set; }
        public Guid IngredientTagsID { get; set; }

        public List<TypeOfDish> typeOfDishes { get; set; } = new List<TypeOfDish>();

        public List<IngredientTag> IngredientTags { get; set; } = new List<IngredientTag>();
        public List<Guid> SelectedIngredientTags { get; set; } = new List<Guid>();
        public ICollection<FavoriteRecipe> FavoriteRecipes { get; set; }
        public ICollection<RecipeReview> RecipeReviews { get; set; }

    }
}