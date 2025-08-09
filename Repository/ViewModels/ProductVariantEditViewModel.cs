using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ProductVariantEditViewModel
    {
        public Guid ID { get; set; } // Chuyển sang kiểu GUID
        public Guid ProductID { get; set; } // Chuyển sang kiểu GUID
        [Required(ErrorMessage = "The Name field is required.")]
        public string Size { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Value must be greater than or equal to 0.")]
        public decimal Price { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Value must be greater than or equal to 0.")]
        public decimal? OriginalPrice { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Value must be greater than or equal to 0.")]
        public int Stock { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
