using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ReviewRequest
    {
        public Guid ProductId { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; }
    }

}
