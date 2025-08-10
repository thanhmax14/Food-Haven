using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Repository.ViewModels
{
    public class CreateStoreViewModel
    {
        public Guid ID { get; set; } = Guid.Empty;

        [Required(ErrorMessage = "The Store Name field is required.")]
        [StringLength(100, ErrorMessage = "Store name must not exceed 100 characters.")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "The Short Description field is required.")]
        [StringLength(100, ErrorMessage = "Short description must not exceed 100 characters.")]
        public string ShortDescriptions { get; set; } = default!;

        [Required(ErrorMessage = "The Detail Description field is required.")]
        [StringLength(2000, ErrorMessage = "Long description must not exceed 2000 characters.")]
        public string LongDescriptions { get; set; } = default!;

        [Required(ErrorMessage = "The Address field is required.")]
        [StringLength(100, ErrorMessage = "Address must not exceed 100 characters.")]
        public string Address { get; set; } = default!;

        [Required(ErrorMessage = "The Phone Number field is required.")]
        [RegularExpression(@"^(0\d{9}|\+84\d{9})$",
            ErrorMessage = "Phone number must be in the format: 0xxxxxxxxx or +84xxxxxxxxx")]
        public string Phone { get; set; } = default!;

        // Ảnh: upload bằng IFormFile, lưu đường dẫn vào Img
        //[Required(ErrorMessage = "The Store Image field is required.")]
        public IFormFile? ImgFile { get; set; }

        // Sẽ được controller set sau khi lưu file
        public string? Img { get; set; }

        public string? Status { get; set; } = "Pending";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public string? UserID { get; set; }
        public string? UserName { get; set; }
    }
}
