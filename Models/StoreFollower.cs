using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class StoreFollower
    {
        public Guid ID { get; set; }
        public Guid StoreID { get; set; }
        [StringLength(450)]
        public string UserID { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public virtual StoreDetails Store { get; set; }
        public virtual AppUser User { get; set; }
    }

}
