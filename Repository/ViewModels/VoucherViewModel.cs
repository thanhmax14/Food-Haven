using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class VoucherViewModel
    {
        public Guid? ID { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount must be > 0")]
        public decimal DiscountAmount { get; set; }

        [Required]
        public string DiscountType { get; set; }

        [Required]
        public string StartDate { get; set; }

        [Required]
        public string ExpirationDate { get; set; }

        public string Scope { get; set; }

        [Range(0, int.MaxValue)]
        public int MaxUsage { get; set; }

        [Range(0, int.MaxValue)]
        public int CurrentUsage { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MinOrderValue { get; set; }

        public bool IsActive { get; set; }
    }

}
