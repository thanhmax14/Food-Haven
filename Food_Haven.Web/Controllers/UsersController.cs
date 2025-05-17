using System.Data;
using BusinessLogic.Hash;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Net.payOS;
using Net.payOS.Types;
using Repository.BalanceChange;
using Repository.ViewModels;

namespace Food_Haven.Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private HttpClient client = null;
        private string _url;
        private readonly IBalanceChangeService _balance;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _product;
        public readonly ICartService _cart;
        public readonly IProductVariantService _productWarian;
        private readonly IProductImageService _img;
        private readonly IOrdersServices _order;
        private readonly IOrderDetailService _orderDetailService;
        private readonly PayOS _payos;
        private readonly ManageTransaction _managetrans;

        public UsersController(UserManager<AppUser> userManager, HttpClient client, IBalanceChangeService balance, IHttpContextAccessor httpContextAccessor, IProductService product, ICartService cart, IProductVariantService productWarian, IProductImageService img, IOrdersServices orders, IOrderDetailService orderDetailService, PayOS payos, ManageTransaction managetrans)
        {
            _userManager = userManager;
            this.client = client;
            _balance = balance;
            _httpContextAccessor = httpContextAccessor;
            _product = product;
            _cart = cart;
            _productWarian = productWarian;
            _img = img;
            _order = orders;
            _orderDetailService = orderDetailService;
            _payos = payos;
            _managetrans = managetrans;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            // Kiểm tra người dùng có đăng nhập hay không
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Lấy ID của user đăng nhập
            string userId = user.Id;
            var list = new IndexUserViewModels();
            try
            {
                var user1 = await _userManager.FindByIdAsync(userId);
                if (user1 == null)
                {
                    return BadRequest(new { message = "User ID không hợp lệ." });
                }

                /*   var stores = await IStoreDetailService.ListAsync(x => x.UserID == id);
                  var storeId = stores?.FirstOrDefault()?.ID ?? Guid.Empty; // Kiểm tra null */

                var UserModel = new UsersViewModel
                {
                    Birthday = user.Birthday,
                    Address = user.Address,
                    img = user.ImageUrl,
                    RequestSeller = user.RequestSeller,
                    isUpdateProfile = user.IsProfileUpdated,
                    ModifyUpdate = user.ModifyUpdate,
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.UserName,
                    Email = user.Email,
                    /*   StoreDeatilId = storeId */
                };
                list.userView = UserModel;
                return View(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody] IndexUserViewModels obj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }
            if (!string.IsNullOrEmpty(obj.userView.PhoneNumber))
            {
                var existPhone = await _userManager.Users.Where(x => x.PhoneNumber == obj.userView.PhoneNumber && x.Id != user.Id).FirstOrDefaultAsync();
                if (existPhone != null)
                {
                    return Json(new { success = false, message = "NumberPhone exist" });
                }
            }
            if (obj.userView.Birthday <= DateTime.Today)
            {
                user.Birthday = obj.userView.Birthday;
            }
            else
            {
                return Json(new { success = false, message = "Date of birth cannot be greater than the current date." });
            }
            user.Email = obj.userView.Email;
            user.Address = obj.userView.Address;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Profile updated successfully" });
            }
            else
            {
                return Json(new { success = false, message = result.Errors.FirstOrDefault()?.Description ?? "Update failed" });
            }

        }
        [HttpPost]
        public async Task<IActionResult> RegisterSeller(IndexUserViewModels obj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }
            else if (string.IsNullOrEmpty(user.Address) || string.IsNullOrEmpty(user.PhoneNumber) ||
            user.Birthday == null || user.Birthday == DateTime.MinValue)
            {
                return Json(new
                {
                    success = false,
                    message = "Please complete all required information before registering as a seller."
                });
            }
            user.RequestSeller = "1";
            user.ModifyUpdate = DateTime.Now;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Register Seller successfully" });
            }
            else
            {



                return Json(new { success = false, message = result.Errors.FirstOrDefault()?.Description ?? "Register Seller failed" });

            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBalance(long number)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { notAuth = true, message = "Bạn phải đăng nhập thể thực hiện hành động này!" });
            }
            if (number < 100000)
            {
                return Json(new ErroMess { msg = "Nạp tối thiểu 100,000 VND" });
            }
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            this._url = "https://localhost:5555/Gateway/WalletService/CreatePayment";
            var temdata = new DepositViewModel
            {
                number = number,
                CalleURL = $"{baseUrl}/home/invoice",
                ReturnUrl = $"{baseUrl}/home/invoice",
                UserID = user.Id
            };

            try
            {
                var getUser = await this._userManager.FindByIdAsync(temdata.UserID);
                if (getUser == null)
                    return Json(new ErroMess { msg = "Người dùng không tồn tại trong hệ thống" });
                var tien = await this._balance.GetBalance(getUser.Id);


                int orderCode = RandomCode.GenerateOrderCode();
                var check = await this._balance.FindAsync(u => u.OrderCode == orderCode);
                while (check != null)
                {
                    orderCode = RandomCode.GenerateOrderCode();
                    check = await this._balance.FindAsync(u => u.OrderCode == orderCode);
                }
                long expirationTimestamp = DateTimeOffset.Now.AddDays(1).ToUnixTimeSeconds();

                ItemData item = new ItemData($"Thực hiện nạp tiền vào tài khoản {getUser.UserName}:", 1, int.Parse(temdata.number + ""));
                List<ItemData> items = new List<ItemData> { item };
                PaymentData paymentData = new PaymentData(orderCode, int.Parse(temdata.number + ""), "", items, $"{temdata.CalleURL}/{orderCode}",
                   $"{temdata.ReturnUrl}/{orderCode}"
                , null, null, null, null, null, expirationTimestamp
                   );
                CreatePaymentResult createPayment = await this._payos.createPaymentLink(paymentData);
                var url = $"https://pay.payos.vn/web/{createPayment.paymentLinkId}/";
                await this._managetrans.ExecuteInTransactionAsync(async () =>
                {
                    var statime = DateTime.Now;
                    var temDongTien = new BalanceChange
                    {
                        MoneyBeforeChange = tien,
                        MoneyChange = temdata.number,
                        MoneyAfterChange = 0m,
                        Description = $"{url}",
                        Status = "PROCESSING",
                        Method = "Deposit",
                        OrderCode = orderCode,
                        StartTime = statime,
                        DueTime = statime,
                        CheckDone = false,

                        UserID = getUser.Id,

                    };

                    await this._balance.AddAsync(temDongTien);

                });
                await this._balance.SaveChangesAsync();

                return Json(new ErroMess { success = true, msg = $"{url}" }); ;
            }
            catch (System.Exception exception)
            {
                Console.WriteLine(exception);
                return Json(new { success = false, msg = "Lỗi không xác định, vui lòng thử lại." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateBalance()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json("Phai Dang Nhap");
            }
            var list = new List<BalanceListViewModels>();
            if (string.IsNullOrWhiteSpace(user.Id))
            {
                return Json(new ErroMess { msg = "Vui lòng nhập userID" });
            }

            try
            {
                var getUser = await this._userManager.FindByIdAsync(user.Id);
                if (getUser == null)
                    return Json(new ErroMess { msg = "Người dùng không tồn tại trong hệ thống" });
                else
                {
                    var getListBalance = await this._balance.ListAsync(
                   u => u.Display && getUser.Id == u.UserID,
                   orderBy: x => x.OrderByDescending(query => query.DueTime.HasValue)  // Ưu tiên bản ghi có DueTime
                                   .ThenByDescending(query => query.DueTime)           // Sắp xếp giảm dần theo DueTime
                                   .ThenByDescending(query => query.StartTime)         // Nếu DueTime = NULL, dùng StartTime
               );


                    if (getListBalance.Any())
                    {
                        var count = 0;
                        foreach (var item in getListBalance)
                        {
                            count++;
                            var getInvoce = RegexAll.ExtractPayosLink(item.Description);
                            if (getInvoce == null)
                                getInvoce = item.Description;
                            list.Add(new BalanceListViewModels
                            {
                                No = count,
                                After = item.MoneyAfterChange,
                                Before = item.MoneyBeforeChange,
                                Change = item.MoneyChange,
                                Date = item.DueTime ?? DateTime.Now,
                                Invoice = getInvoce,
                                Status = item.Status,
                                Types = item.Method
                            });
                        }
                    }
                    return Json(list);

                }
            }
            catch (Exception)
            {
                return Json("Lỗi Hệ Thống");
            }
        }

    }
}
