using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Complaints;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.VoucherServices;
using Food_Haven.Web.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Models;
using Net.payOS;
using Quartz;
using Quartz.Util;
using System;
using System.Threading.Tasks;

namespace Food_Haven.Web.Services.Auto
{
    public class ReleasePaymentDeposit : IJob
    {
        private readonly IOrdersServices _order;
        private readonly IOrderDetailService _orderdetail;
        private readonly UserManager<AppUser> _userManager;
        private readonly IProductService _product;
        private readonly IBalanceChangeService _balance;
        private readonly IComplaintServices _complant;
        private readonly IVoucherServices _voucher;
        private readonly IProductVariantService _producttype;
        private readonly PayOS _payos;
        private readonly IHubContext<ChatHub> _hubContext;

        public ReleasePaymentDeposit(IOrdersServices order, IOrderDetailService orderdetail, UserManager<AppUser> userManager, IProductService product, IBalanceChangeService balance, IComplaintServices complant, IVoucherServices voucher, IProductVariantService producttype, PayOS payos, IHubContext<ChatHub> hubContext)
        {
            _order = order;
            _orderdetail = orderdetail;
            _userManager = userManager;
            _product = product;
            _balance = balance;
            _complant = complant;
            _voucher = voucher;
            _producttype = producttype;
            _payos = payos;
            _hubContext = hubContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var getListDeposit = await _balance.ListAsync(b => b.Method == "Deposit" && !b.IsComplete);

            if (getListDeposit != null && getListDeposit.Any())
            {
                foreach (var deposit in getListDeposit)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(deposit.OrderCode+"") || !long.TryParse(deposit.OrderCode + "", out long orderCode))
                        {
                            Console.WriteLine($"[Quartz] ❌ Deposit {deposit.ID} có OrderCode không hợp lệ: '{deposit.OrderCode}'");
                            deposit.IsComplete = true;
                            deposit.Description = "OrderCode không hợp lệ";
                            deposit.Status = "FAILED";

                            await _balance.UpdateAsync(deposit);
                            await _balance.SaveChangesAsync();
                            continue;
                        }

                        var checkOrder = await _payos.getPaymentLinkInformation(orderCode);
                        var status = checkOrder.status?.ToUpper() ?? "UNKNOWN";

                        deposit.DueTime = DateTime.Now;
                        deposit.Status = status;

                        switch (status)
                        {
                            case "CANCELLED":
                                deposit.IsComplete = true;
                                deposit.Description = "Deposit CANCELLED";

                                break;
                            case "EXPIRED":
                                deposit.IsComplete = true;
                                deposit.Description = "Deposit EXPIRED";
                                break;
                            case "FAILED":
                                deposit.IsComplete = true;
                                deposit.Description = "Deposit FAILED";
                                break;
                            case "PENDING":
                            case "UNDERPAID":
                            case "PROCESSING":
                                break;
                            default:
                                Console.WriteLine($"[Quartz] ⚠️ Trạng thái không xác định: {status} cho Deposit: {deposit.ID}");
                                break;
                        }
                        await _hubContext.Clients.All.SendAsync("ReceiveUpdate", new { message = "Data changed" });
                        await _balance.UpdateAsync(deposit);
                        await _balance.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Quartz] ❗ Lỗi xử lý Deposit: {deposit.ID}, lỗi: {ex.Message}");
                        deposit.IsComplete = true;
                        deposit.Description = "Deposit FAILED";
                        deposit.Status = "FAILED";

                        await _balance.UpdateAsync(deposit);
                        await _balance.SaveChangesAsync();
                    }
                }

                Console.WriteLine($"[Quartz] ✅ Đã kiểm tra trạng thái nạp tiền lúc {DateTime.Now}");
            }
            else
            {
                Console.WriteLine($"[Quartz] ⚠️ Không có giao dịch nạp tiền nào cần kiểm tra lúc {DateTime.Now}");
            }
        }


    }
}
