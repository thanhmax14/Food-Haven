using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Net.payOS;
using Repository.ViewModels;
using BusinessLogic.Services.BalanceChanges;
using Microsoft.AspNetCore.Identity;
using BusinessLogic.Services.Orders;
using Models;
using Repository.BalanceChange;
using BusinessLogic.Services.OrderDetailService;

namespace Food_Haven.Web.APIController
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {

        private readonly IBalanceChangeService _balance;
        private readonly PayOS _payos;
        private readonly UserManager<AppUser> _userManager;
        private readonly ManageTransaction _managetrans;
        private readonly IOrdersServices _ordersServices;
        private readonly IOrderDetailService _detail;

        public WalletController(IBalanceChangeService balance, PayOS payos, UserManager<AppUser> userManager, ManageTransaction managetrans, IOrdersServices ordersServices, IOrderDetailService detail)
        {
            _balance = balance;
            _payos = payos;
            _userManager = userManager;
            _managetrans = managetrans;
            _ordersServices = ordersServices;
            _detail = detail;
        }

        [HttpPost("webhook-url")]
        public async Task<IActionResult> ReceivePaymentAsync([FromBody] WebhookType webhook)
        {
            try
            {
                WebhookData data = _payos.verifyPaymentWebhookData(webhook);
                if (data != null && webhook.success)
                {
                    var getBalance = await this._balance.FindAsync(u => u.OrderCode == data.orderCode && u.IsComplete == false);

                    if (getBalance != null)
                    {
                        await this._managetrans.ExecuteInTransactionAsync(async () =>
                        {
                            var url = getBalance.Description;
                            var user = await this._userManager.FindByIdAsync(getBalance.UserID);
                            if (user != null)
                            {
                                var tongtien = await _balance.GetBalance(user.Id) + data.amount;
                                getBalance.Description = $"Thực hiện nạp tiền vào tài khoản,[{url}]";
                                getBalance.Status = "Success";
                                getBalance.MoneyBeforeChange = await _balance.GetBalance(user.Id);
                                getBalance.MoneyAfterChange = tongtien;
                                getBalance.MoneyChange = data.amount;
                                getBalance.Display = true;
                                getBalance.IsComplete = true;
                                getBalance.DueTime = DateTime.Now;
                                getBalance.CheckDone = true;
                            }
                            await _balance.UpdateAsync(getBalance);
                        });
                        await _balance.SaveChangesAsync();

                        return Ok(new { success = true });
                    }
                    else
                    {
                        var order = await this._ordersServices.FindAsync(u => u.OrderCode == data.orderCode + "");
                        if (order != null)
                        {
                            order.Status = "PROCESSING";
                            order.PaymentStatus = "Success";
                            order.IsActive = true;
                            order.ModifiedDate = DateTime.Now;
                            await this._ordersServices.UpdateAsync(order);
                            await this._ordersServices.SaveChangesAsync();
                            var getOrderDetil = await this._detail.ListAsync(_detail => _detail.OrderID == order.ID);
                            foreach (var item in getOrderDetil)
                            {
                                item.Status = "PROCESSING";
                                item.IsActive = true;
                                item.ModifiedDate = DateTime.Now;
                                await this._detail.UpdateAsync(item);
                                await this._detail.SaveChangesAsync();
                            }
                            return Ok(new { success = true });
                        }
                    }
                    return Ok(false);
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ErroMess { msg = ex.Message });
            }

        }




        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("confirm-webhook")]
        public IActionResult ConfirmWebhook([FromBody] WebhookType webhook)
        {
            try
            {
                WebhookData data = _payos.verifyPaymentWebhookData(webhook);
                if (data != null && webhook.success)

                    return Ok(true);
                else
                    return Ok(false);
            }
            catch (System.Exception exception)
            {

                Console.WriteLine(exception);
                return Ok(false);
            }

        }

        [HttpGet]
        public IActionResult GetTestData()
        {
            var testData = new
            {
                Id = 1,
                Message = "This is test data from API",
                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return Ok(testData);
        }
    }
}
