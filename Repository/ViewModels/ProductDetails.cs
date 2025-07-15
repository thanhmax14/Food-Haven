using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ProductDetails
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsWishList { get; set; } = false;
        public int totalsell { get; set; } = 0;    
        public IEnumerable<Models.ProductImage> ProductImages { get; set; } = new List<Models.ProductImage>();
        public IEnumerable<ProductTypes> ProductVariants { get; set; } = new List<ProductTypes>();
        public IEnumerable<Review> Review { get; set; } = new List<Review>();
        public Categories categories { get; set; } = new Categories();
        public Models.StoreDetails storeDetails { get; set; } = new Models.StoreDetails();
        public IEnumerable<Categories> Allcate { get; set; } = new List<Categories>();
        public IEnumerable<Product> ProductBycate { get; set; } = new List<Product>();
    }
}
