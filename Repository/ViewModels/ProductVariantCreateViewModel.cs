using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ProductVariantCreateViewModel
    {
        public Guid ID { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        public string Size { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0.")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Original Price must be greater than or equal to 0.")]
        public decimal OriginalPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than or equal to 0.")]
        public int Stock { get; set; }

        [DataType(DataType.Date)]
        public DateTime ManufactureDate { get; set; }

        public Guid ProductID { get; set; }
    }
}
