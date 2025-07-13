using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ProductListViewModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime ManufactureDate { get; set; }// Ngày sản xuất
        public bool IsActive { get; set; } = false;
        public bool IsOnSale { get; set; } // Có đang giảm giá?
        public string? StoreName { get; set; }
        public string CategoryName { get; set; }
        public Guid CateID { get; set; }
        public List<ProductImageViewModel> Images { get; set; } // Danh sách hình ảnh (1 ảnh chính, 4 ảnh phụ)
        public ProductListViewModel()
        {
            Images = new List<ProductImageViewModel>();
        }
        public Guid StoreId { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
    public class ProductsViewModel1
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string StoreName { get; set; }
        public string CategoryName { get; set; }
        public Guid CateID { get; set; }
        public decimal Price { get; set; }
        public List<string> Img { get; set; } = new List<string>();  // Danh sách hình ảnh sản phẩm
        public bool IsWishList { get; set; } = false;  // Trạng thái trong danh sách yêu thích
        public Guid StoreId { get; set; }  // ID cửa hàng
    }
    public class CategoryViewModel1
    {
        public Guid ID { get; set; }
        public string Name { get; set; }  // Tên danh mục
        public string Description { get; set; }  // Mô tả (tuỳ chọn)
        public int ProductCount { get; set; }  // Số lượng sản phẩm trong danh mục

        // Bạn có thể thêm liên kết đến sản phẩm trong danh mục
        public List<ProductsViewModel> Products { get; set; } = new List<ProductsViewModel>();
    }
}
