using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class StoreReportViewModel
    {
        public Guid ID { get; set; }
        public Guid StoreID { get; set; }
        [StringLength(450)]
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        [StringLength(100)]
        public string Reason { get; set; }
        [StringLength(500)]
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
    }
}