using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class SellerViewModel
    {
        
        public DateTime? RegisterDate { get; set; }
        public string UserId { get; set; }
        public string? UserName { get; set; }
        public string? ProductPurchased { get; set; }
        public string? NumberOfProducts {  get; set; }
        public string? ProductsSold { get; set; }
        public string? Recipes { get; set; }
        public bool HasStore { get; set; }
        public Guid? StoreId { get; set; }
        public string? StoreName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public int TotalPosts { get; set; } 





    }
}
