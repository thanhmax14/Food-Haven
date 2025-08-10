using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Repository.ViewModels
{
    public class StoreViewModel
    {
        public Guid ID { get; set; }
        [Required(ErrorMessage = "The Store Name field is required.")]
        [StringLength(100, ErrorMessage = "Store name must not exceed 500 characters.")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "The Short Description field is required.")]
        [StringLength(100, ErrorMessage = "Short description must not exceed 1000 characters.")]
        public string ShortDescriptions { get; set; }

        [Required(ErrorMessage = "The Detail Description field is required.")]
        [StringLength(2000, ErrorMessage = "Long description must not exceed 10000 characters.")]
        public string LongDescriptions { get; set; }

        [Required(ErrorMessage = "The Address field is required.")]
        [StringLength(100, ErrorMessage = "Address must not exceed 500 characters.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "The Phone Number field is required.")]
        [RegularExpression(@"^(0\d{9}|\+84\d{9})$",
            ErrorMessage = "Phone number must be in the format: 0xxxxxxxxx or +84xxxxxxxxx")]
        public string Phone { get; set; }
        public string? Img { get; set; }  // Hình ảnh cửa hàng
        //[Required(ErrorMessage = "The Store Image field is required.")]
        public IFormFile? ImgFile { get; set; }     // <— file upload nằm ở đây
        public string? Status { get; set; } = "Pending"; // Trạng thái mặc định
        public bool IsActive { get; set; } = true; // Mặc định chưa hoạt động
        public DateTime CreatedDate { get; set; } = DateTime.Now; // Ngày tạo mặc định
        public DateTime? ModifiedDate { get; set; }
        public string? UserID { get; set; } // ID của chủ cửa hàng (nếu cần)
        public string? UserName { get; set; }
        public int MyProperty { get; set; }
    }
}
