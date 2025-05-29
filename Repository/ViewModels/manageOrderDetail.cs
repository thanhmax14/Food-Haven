using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class manageOrderDetail
    {
        public List<ManageOrderDetailInfo> ItemDetail { get; set; }=new List<ManageOrderDetailInfo>();
        public string OrderTracking { get; set; }
        public Guid OrderID { get; set; }
        public string Note { get; set; }
        public decimal Subtotal { get; set; } = 0;
        public decimal TotalOrder { get; set; } = 0;
        public decimal Discount { get; set; } = 0;
        public string NameVocher { get; set; } = "";
        public string PaymentMethod { get; set; }
        public string IDLogistics { get; set; } = "";
        public string NameCustomer { get; set; }
        public string EmailCustomer { get; set; }
        public string PhoneCustomer { get; set; }
        public string ShippingAddress { get; set; }
        public string  UserNameCus { get; set; }
        public string ImageCus { get; set; }
        public List<OrderStatusHistory> StatusHistories { get; set; } = new();


    }
}
public class ManageOrderDetailInfo
{
    public string Produtype { get; set; }
    public string Product { get; set; }
    public string NameShop { get; set; }
    public Decimal ItemPrice { get; set; } = 0;
    public int Quantity { get; set; } = 0;
    public decimal Totals { get; set; } = 0;
    public Guid ProductID { get; set; }
    public string ImageProduct { get; set; }
}
public class OrderStatusHistory
{
    public string Status { get; set; }
    public DateTime Time { get; set; }

    public static List<OrderStatusHistory> Parse(string description)
    {
        var list = new List<OrderStatusHistory>();
        if (string.IsNullOrWhiteSpace(description)) return list;

        var parts = description.Split('#');
        foreach (var part in parts)
        {
            var sub = part.Split('-');
            if (sub.Length >= 2 && DateTime.TryParse(string.Join("-", sub.Skip(1)), out var time))
            {
                list.Add(new OrderStatusHistory
                {
                    Status = sub[0].Trim().ToUpper(),
                    Time = time
                });
            }
        }

        return list.OrderBy(s => s.Time).ToList();
    }
}
