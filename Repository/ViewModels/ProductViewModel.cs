using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Repository.ViewModels
{
    public class ProductViewModel
    {
        public string Name { get; set; }
        [Required(ErrorMessage = "The Short Description field is required.")]
        public string ShortDescription { get; set; }
        [Required(ErrorMessage = "The Detail Description field is required.")]
        public string LongDescription { get; set; }
        public DateTime ManufactureDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsOnSale { get; set; } = false;
        public Guid CateID { get; set; }
        public Guid StoreID { get; set; }
        //public List<IFormFile> Images { get; set; } // Upload tối đa 5 hình
        public IFormFile? MainImage { get; set; }  // Ảnh chính
        public List<IFormFile> GalleryImages { get; set; } = new List<IFormFile>();  // Ảnh phụ
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

        public string? ExistingMainImage { get; set; } // Chỉ ảnh chính
        public List<string>? ExistingGalleryImages { get; set; } = new(); // Chỉ ảnh phụ

    }
}

