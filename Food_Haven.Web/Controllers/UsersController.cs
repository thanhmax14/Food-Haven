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

                return View(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }
    }
}
