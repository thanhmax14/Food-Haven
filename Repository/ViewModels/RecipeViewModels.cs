using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Microsoft.AspNetCore.Http;

namespace Repository.ViewModels
{
    public class RecipeViewModels
    {
        public Guid ID { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Username { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string CookingStep { get; set; }
        public string ShortDescriptions { get; set; }
        [Required(ErrorMessage = "Short description is required")]
        [StringLength(250, ErrorMessage = "Short description cannot exceed 250 characters")]
        public string PreparationTime { get; set; }
        [Required(ErrorMessage = "Cooking steps are required")]

        public string CookTime { get; set; }
        public string TotalTime { get; set; }
        [Required(ErrorMessage = "Please select difficulty level")]

        public string DifficultyLevel { get; set; }
        [Required(ErrorMessage = "Ingredients are required")]
        public string Ingredient { get; set; }
        [Required(ErrorMessage = "Servings is required")]
        public string Servings { get; set; }
        public int RecipesMadeItCount { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; } = false;
        [Required(ErrorMessage = "Please select a category")]

        public Guid CateID { get; set; }
        public Guid IngredientTagID { get; set; }
        [Required(ErrorMessage = "Thumbnail image is required")]
        public string ThumbnailImage { get; set; }
        public string TypeOfDishName { get; set; }
        public string? RejectNote { get; set; }

        public List<Categories> Categories { get; set; } = new List<Categories>();

        public string UserID { get; set; }
        [Required(ErrorMessage = "Please select a type of dish")]

        public Guid TypeOfDishID { get; set; }
        public Guid IngredientTagsID { get; set; }
        public string status { get; set; } = "";
        public string ImageUrl { get; set; } 

        public List<TypeOfDish> typeOfDishes { get; set; } = new List<TypeOfDish>();
        public List<TypeOfDishViewModel> typeOfDishes1 { get; set; } = new List<TypeOfDishViewModel>();

        public List<IngredientTag> IngredientTags { get; set; } = new List<IngredientTag>();
        public List<Guid> SelectedIngredientTags { get; set; } = new List<Guid>();
        public ICollection<FavoriteRecipe> FavoriteRecipes { get; set; }
        public ICollection<RecipeReview> RecipeReviews { get; set; }
        public IEnumerable<RecipeReviewViewModel> RecipeReviewViewModels { get; set; } = new List<RecipeReviewViewModel>();
        public string? RecipeOwnerUserName { get; set; }
        public bool IsFavorite { get; set; } // mới thêm
        public int ReviewCount { get; set; }  // Thêm mới
        public double AverageRating { get; set; }

    }



}