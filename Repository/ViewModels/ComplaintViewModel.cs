using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ComplaintViewModel
    {
        public Guid OrderDetailID { get; set; }
        public string Description { get; set; }
        public List<IFormFile> Images { get; set; }
    }

}
