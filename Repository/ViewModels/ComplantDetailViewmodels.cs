using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ComplantDetailViewmodels
    {
        public Guid ComplantID { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DateReply { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public string SellerReply { get; set; } = "";
        public string AdminrReply { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime? DateAdminCreate { get; set; }
        public string NameShop { get; set; } = "";
        public List<string> image { get; set; } = new();

    }
}
