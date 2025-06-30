using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class StoreFollowerViewModel
    {
        public Guid ID { get; set; }
        public Guid StoreID { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; } = default!;
        public string? Img { get; set; }  // Hình ảnh cửa hàng
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? ShortDescriptions { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
    }
}