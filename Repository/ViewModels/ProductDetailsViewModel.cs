﻿namespace Repository.ViewModels
{
    public class ProductDetailsViewModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime ManufactureDate { get; set; }// Ngày sản xuất
        public bool IsActive { get; set; } = false;
        public bool IsOnSale { get; set; } // Có đang giảm giá?
        public string? StoreName { get; set; }


        public Dictionary<string, decimal> SizeWithPrice { get; set; } = new Dictionary<string, decimal>();
        public List<string> size { get; set; } = new List<string>();

        public int Stocks { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; } = 0;

        public List<string> Img { get; set; } = new List<string>();
        public Guid CateID { get; set; }

        public Guid StoreID { get; set; }

        public List<CommentViewModels> Comments { get; set; } = new List<CommentViewModels>();

    }
}
