using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class TypeOfDishViewModel
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public int RecipeCount { get; set; }

        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}
