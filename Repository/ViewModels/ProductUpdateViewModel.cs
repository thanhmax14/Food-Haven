using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repository.ViewModels
{
    public class ProductUpdateViewModel
    {
        public Guid ProductID { get; set; }

        public string Name { get; set; }

        public string ShortDescription { get; set; }
        [Required(ErrorMessage = "The Long Description field is required.")]
        public string LongDescription { get; set; }

        public DateTime ManufactureDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsOnSale { get; set; } = false;

        [Required]
        public Guid CateID { get; set; }

        [Required]
        public Guid StoreID { get; set; }

        //public List<IFormFile> NewImages { get; set; } = new(); // Nếu vẫn muốn dùng, không sao

        // ✅ Bổ sung mới để chia rõ ảnh
        public IFormFile? MainImage { get; set; }
        public List<IFormFile> GalleryImages { get; set; } = new();

        public List<string> ExistingImages { get; set; } = new(); // ảnh cũ
        public List<string> RemoveImageUrls { get; set; } = new(); // ảnh cần xóa

        public List<SelectListItem> Categories { get; set; } = new();

        public List<string> NewImageUrls { get; set; } = new(); // có thể giữ nếu cần lưu đường dẫn mới
        public string? ExistingMainImage { get; set; } // đường dẫn ảnh chính hiện tại
    }
}
