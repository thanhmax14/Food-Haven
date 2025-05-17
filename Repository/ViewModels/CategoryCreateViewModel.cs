using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class CategoryCreateViewModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Commission must be between 0% and 100%!")]
        public float Commission { get; set; } // Hoa hồng %

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Display order must be greater than 0!")]
        public int Number { get; set; }
        public IFormFile Image { get; set; } // Thêm dòng này
    }
}
