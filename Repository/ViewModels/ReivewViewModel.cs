using System.ComponentModel.DataAnnotations;

namespace Repository.ViewModels
{
    public class ReivewViewModel
    {

        public Guid ID { get; set; }
        public string? Comment { get; set; }
        public DateTime CommentDate { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Reply cannot be empty")]
        public string? Reply { get; set; }


        public DateTime? ReplyDate { get; set; } = DateTime.Now;
        //1 (true) → ẩn
        //0 (false) → hiện
        public bool Status { get; set; } = false;
        public int Rating { get; set; } = 5;

        public string UserID { get; set; }

        public Guid ProductID { get; set; }

        public string? Username { get; set; }
        public string? ProductName { get; set; }

        public Guid? StoreId { get; set; }

        public string? StoreName { get; set; }


    }
}
