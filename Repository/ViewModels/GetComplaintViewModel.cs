using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class GetComplaintViewModel
    {
        public Guid Id { get; set; }
        public string OrderCode { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string SellerReply { get; set; }
        public string AdminReply { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ReportStatus { get; set; }
        public DateTime? ReplyDate { get; set; }
        public DateTime? AdminReplyDate { get; set; }


    }
}
