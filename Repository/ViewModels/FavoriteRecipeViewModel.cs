using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class FavoriteRecipeViewModel
    {
        public Guid ID { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public Guid RecipeID { get; set; }
        public string UserID { get; set; }
        public string ThumbnailImage { get; set; }
        public string DifficultyLevel { get; set; }

        public string Title { get; set; }

        public string ShortDescriptions { get; set; }
        public Guid TypeOfDishID { get; set; }
        public string status { get; set; } = "";
        public string TypeOfDishName { get; set; } // để hiển thị tên loại món


    }
}