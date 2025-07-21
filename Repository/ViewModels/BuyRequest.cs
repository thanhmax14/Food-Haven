using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class BuyRequest
    {
        public Dictionary<Guid, int> Products { get; set; } = new();
        public bool IsOnline { get; set; } = false;
        public string UserID { get; set; }
        public string SuccessUrl { get; set; }
        public string CalledUrl { get; set; }

    }
    public class OrderInputModel
    {
        [Required(ErrorMessage = "Please enter your First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Please enter your phone number")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please enter your delivery address")]
        public string Address { get; set; }
        public string Note { get; set; } 
    }
}
