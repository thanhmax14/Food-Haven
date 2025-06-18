using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
using X.PagedList;

namespace Repository.ViewModels
{
    public class MyViewRecipePageViewModel
    {
        public IPagedList<RecipeViewModels> Recipes { get; set; }
        public List<Categories> Categories { get; set; }
        public List<TypeOfDish> TypeOfDishes { get; set; }
        public List<IngredientTag> IngredientTags { get; set; } = new List<IngredientTag>();
        public List<Guid> SelectedIngredientTags { get; set; } = new List<Guid>();


    }
}