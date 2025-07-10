using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ViewStoreDetailViewModel
    {
        public Guid ID { get; set; }
        public string StoreName { get; set; }
        public string StoreOwner { get; set; } // Full name từ bảng Users
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ShortDescriptions { get; set; }
        public string LongDescriptions { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string ImageUrl { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public string RejectNote { get; set; }
    }
}
