using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Complaints;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.VoucherServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Models;
using Quartz;
using System;

namespace Food_Haven.Web.Services.Auto
{
    [Authorize]
    public class ReleasePaymentJob : IJob
    {
        private readonly IOrdersServices _order;
        private readonly IOrderDetailService _orderdetail;
        private readonly UserManager<AppUser> _userManager;
        private readonly IProductService _product;
        private readonly IBalanceChangeService _balance;
        private readonly IComplaintServices _complant;
        private readonly IVoucherServices _voucher;
        private readonly IProductVariantService _producttype;

        public ReleasePaymentJob(IOrdersServices order, IOrderDetailService orderdetail, UserManager<AppUser> usermanager, IProductService product, IBalanceChangeService balance, IComplaintServices complant, IVoucherServices voucher, IProductVariantService producttype)
        {
            _order = order;
            _orderdetail = orderdetail;
            _userManager = usermanager;
            _product = product;
            _balance = balance;
            _complant = complant;
            _voucher = voucher;
            _producttype = producttype;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("ReleasePaymentJob is executing at " + DateTime.Now);
            var orders = await _order.ListAsync(o => o.IsActive && !o.IsPaid);

            foreach (var order in orders)
            {
                try
                {
                    var timeSinceCreated = DateTime.Now - order.CreatedDate;

                    // RULE 1: Pending > 1h → Hủy và hoàn tiền
                    if (order.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase) && timeSinceCreated.TotalHours >= 1)
                    {
                        var orderDetails = await _orderdetail.ListAsync(d => d.OrderID == order.ID);

                        if (orderDetails != null && orderDetails.Any())
                        {
                            foreach (var item in orderDetails)
                            {
                                item.Status = "Refunded";
                                item.ModifiedDate = DateTime.Now;
                                await _orderdetail.UpdateAsync(item);

                                var product = await _producttype.FindAsync(p => p.ID == item.ProductTypesID);
                                if (product != null)
                                {
                                    product.Stock += item.Quantity;
                                    product.IsActive = true;
                                    await _producttype.UpdateAsync(product);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[RULE 1] Đơn {order.ID} không có sản phẩm, nhưng vẫn huỷ và hoàn tiền.");
                        }

                        var currentBalance = await _balance.GetBalance(order.UserID);
                        var refund = new BalanceChange
                        {
                            UserID = order.UserID,
                            MoneyChange = order.TotalPrice,
                            MoneyBeforeChange = currentBalance,
                            MoneyAfterChange = currentBalance + order.TotalPrice,
                            Method = "Refund",
                            Status = "Success",
                            Display = true,
                            IsComplete = true,
                            CheckDone = true,
                            StartTime = DateTime.Now,
                            DueTime = DateTime.Now,
                            Description = $"Refund for order {order.ID} due to pending for over 1 hour."
                        };
                        await _balance.AddAsync(refund);

                        order.Status = "CANCELLED BY SHOP";
                        order.Description = string.IsNullOrEmpty(order.Description)
                            ? $"CANCELLED BY SHOP - {DateTime.Now}"
                            : $"{order.Description}#CANCELLED BY SHOP - {DateTime.Now}";
                        order.PaymentStatus = "Refunded";
                        order.ModifiedDate = DateTime.Now;
                        order.IsPaid = true;

                        await _order.UpdateAsync(order);
                        await _orderdetail.SaveChangesAsync();
                        await _producttype.SaveChangesAsync();
                        await _order.SaveChangesAsync();
                        await _balance.SaveChangesAsync();

                        Console.WriteLine($"[RULE 1] Đơn {order.ID} đã huỷ do pending quá 1 giờ.");
                        continue;
                    }

                    // RULE 2: PREPARING IN KITCHEN > 2h → Hủy và hoàn tiền
                    if (order.Status.Equals("PREPARING IN KITCHEN", StringComparison.OrdinalIgnoreCase) && timeSinceCreated.TotalHours >= 2)
                    {
                        var orderDetails = await _orderdetail.ListAsync(d => d.OrderID == order.ID);

                        if (orderDetails != null && orderDetails.Any())
                        {
                            foreach (var item in orderDetails)
                            {
                                item.Status = "Refunded";
                                item.ModifiedDate = DateTime.Now;
                                await _orderdetail.UpdateAsync(item);

                                var product = await _producttype.FindAsync(p => p.ID == item.ProductTypesID);
                                if (product != null)
                                {
                                    product.Stock += item.Quantity;
                                    product.IsActive = true;
                                    await _producttype.UpdateAsync(product);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[RULE 2] Đơn {order.ID} không có sản phẩm, nhưng vẫn huỷ và hoàn tiền.");
                        }

                        var currentBalance = await _balance.GetBalance(order.UserID);
                        var refund = new BalanceChange
                        {
                            UserID = order.UserID,
                            MoneyChange = order.TotalPrice,
                            MoneyBeforeChange = currentBalance,
                            MoneyAfterChange = currentBalance + order.TotalPrice,
                            Method = "Refund (Kitchen Timeout)",
                            Status = "Success",
                            Display = true,
                            IsComplete = true,
                            CheckDone = true,
                            StartTime = DateTime.Now,
                            DueTime = DateTime.Now,
                            Description = $"Refund for order {order.ID} due to 'PREPARING IN KITCHEN' timeout (over 2 hours)."
                        };
                        await _balance.AddAsync(refund);

                        order.Status = "CANCELLED BY SHOP";
                        order.Description = string.IsNullOrEmpty(order.Description)
                            ? $"CANCELLED BY SHOP - kitchen timeout - {DateTime.Now}"
                            : $"{order.Description}#CANCELLED BY SHOP - kitchen timeout - {DateTime.Now}";
                        order.PaymentStatus = "Refunded";
                        order.ModifiedDate = DateTime.Now;
                        order.IsPaid = true;

                        await _order.UpdateAsync(order);
                        await _orderdetail.SaveChangesAsync();
                        await _producttype.SaveChangesAsync();
                        await _order.SaveChangesAsync();
                        await _balance.SaveChangesAsync();

                        await _order.UpdateAsync(order);
                        await _order.SaveChangesAsync();
                        Console.WriteLine($"[RULE 2] Đơn {order.ID} đã huỷ do 'PREPARING IN KITCHEN' quá 2 giờ.");
                        continue;
                    }


                    // RULE 3: Delivered > 3 ngày và không có tranh chấp → Cộng tiền cho người bán
                    if (order.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase) && timeSinceCreated.TotalDays >= 3)
                    {
                        var getOrderDetails = await _orderdetail.ListAsync(d => d.OrderID == order.ID);
                        if (getOrderDetails.Any())
                        {
                            bool hasAnyActiveComplaint = false;

                            foreach (var item in getOrderDetails)
                            {
                                var complaint = await _complant.FindAsync(c => c.OrderDetailID == item.ID);
                                if (complaint != null)
                                {
                                    var timeSinceComplaint = DateTime.Now - complaint.CreatedDate;

                                    if (string.IsNullOrWhiteSpace(complaint.Reply) && timeSinceComplaint.TotalDays >= 3)
                                    {
                                        // ❗ RULE 3.1: Tranh chấp quá 3 ngày chưa phản hồi → Hủy đơn và hoàn tiền
                                        decimal currentBalance = await _balance.GetBalance(order.UserID);

                                        var refund = new BalanceChange
                                        {
                                            UserID = order.UserID,
                                            MoneyChange = order.TotalPrice,
                                            MoneyBeforeChange = currentBalance,
                                            MoneyAfterChange = currentBalance + order.TotalPrice,
                                            Method = "Refund (Dispute Timeout)",
                                            Status = "Success",
                                            Display = true,
                                            IsComplete = true,
                                            CheckDone = true,
                                            StartTime = DateTime.Now,
                                            DueTime = DateTime.Now,
                                            Description = $"Refund đơn {order.ID} do tranh chấp quá 3 ngày chưa phản hồi."
                                        };

                                        await _balance.AddAsync(refund);

                                        order.Status = "CANCELLED BY SYSTEM";
                                        order.Description = string.IsNullOrEmpty(order.Description)
                                            ? $"CANCELLED BY SYSTEM - dispute timeout - {DateTime.Now}"
                                            : $"{order.Description}#CANCELLED BY SYSTEM - dispute timeout - {DateTime.Now}";
                                        order.PaymentStatus = "Refunded";
                                        order.ModifiedDate = DateTime.Now;
                                        order.IsPaid = true;

                                        await _order.UpdateAsync(order);
                                        await _balance.SaveChangesAsync();
                                        await _order.SaveChangesAsync();

                                        Console.WriteLine($"[RULE 3.1] Đơn {order.ID} bị hủy do tranh chấp quá 3 ngày không phản hồi.");
                                        hasAnyActiveComplaint = true;
                                        break; // dừng xử lý đơn này
                                    }
                                    else
                                    {
                                        // ❌ Có tranh chấp chưa xử lý xong → Không cộng tiền
                                        Console.WriteLine($"[RULE 3] Đơn {order.ID} không cộng tiền do có tranh chấp ở sản phẩm {item.ID}.");
                                        hasAnyActiveComplaint = true;
                                        break;
                                    }
                                }
                            }

                            // ✅ Nếu không có tranh chấp hoặc tất cả đã xử lý → cộng tiền cho người bán
                            if (!hasAnyActiveComplaint)
                            {
                                decimal currentBalance = await _balance.GetBalance(order.UserID);

                                foreach (var item in getOrderDetails)
                                {
                                    decimal commissionPercent = Convert.ToDecimal(item.CommissionPercent ?? 0f);
                                    decimal moneychange = item.TotalPrice * (commissionPercent / 100);

                                    var tempBalance = new BalanceChange
                                    {
                                        UserID = order.UserID,
                                        MoneyChange = moneychange,
                                        MoneyBeforeChange = currentBalance,
                                        MoneyAfterChange = currentBalance + moneychange,
                                        Method = "Sell Product",
                                        Status = "Success",
                                        Display = true,
                                        IsComplete = true,
                                        CheckDone = true,
                                        StartTime = DateTime.Now,
                                        DueTime = DateTime.Now,
                                        Description = $"Cộng tiền cho người bán sau 3 ngày delivered - Đơn {order.ID}"
                                    };

                                    await _balance.AddAsync(tempBalance);
                                    currentBalance += moneychange;
                                }

                                order.IsPaid = true;
                                await _order.UpdateAsync(order);
                                await _balance.SaveChangesAsync();
                                await _order.SaveChangesAsync();

                                Console.WriteLine($"[RULE 3] Đơn {order.ID} đã cộng tiền sau 3 ngày delivered và không có tranh chấp.");
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Đơn {order.ID} gặp lỗi: {ex.Message}");
                }
            }


        }

    }
}
