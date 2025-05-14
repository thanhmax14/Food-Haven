using System.Data;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Repository.ViewModels;

namespace Food_Haven.Web.Controllers
{
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

        public UsersController(UserManager<AppUser> userManager, HttpClient client, IBalanceChangeService balance, IHttpContextAccessor httpContextAccessor, IProductService product, ICartService cart, IProductVariantService productWarian, IProductImageService img, IOrdersServices orders, IOrderDetailService orderDetailService)
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

    }
}
