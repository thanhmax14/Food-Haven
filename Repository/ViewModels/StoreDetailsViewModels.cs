using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class StoreDetailsViewModels
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = default!;
        public string? LongDescriptions { get; set; }
        public string? ShortDescriptions { get; set; }
        public string CategoryName { get; set; }


        public List<ProductsViewModel> ProductViewModel { get; set; }

        public List<CategoryViewModel> CategoryViewModels { get; set; }

        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Img { get; set; }  // Hình ảnh cửa hàng
        public string? Status { get; set; } = "PENDING"; // Trạng thái mặc định
        public bool IsActive { get; set; } = false; // Mặc định chưa hoạt động
        public DateTime CreatedDate { get; set; } = DateTime.Now; // Ngày tạo mặc định
        public DateTime? ModifiedDate { get; set; }
        public string? UserID { get; set; } // ID của chủ cửa hàng (nếu cần)
        public string? UserName { get; set; }
        public string? UserNameRepo { get; set; }
        public string? Email { get; set; }
        public int MyProperty { get; set; }
        [StringLength(200, ErrorMessage = "Message must be at most 200 characters.")]

        [Required(ErrorMessage = "Message is require")]
        public string Message { get; set; }
    }
}
