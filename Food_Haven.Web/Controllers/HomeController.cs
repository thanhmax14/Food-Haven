using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.StoreDetail;
using BusinessLogic.Services.Wishlists;
using Food_Haven.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Models;
using Net.payOS;
using Repository.ViewModels;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Food_Haven.Web.Services;
using BusinessLogic.Hash;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using BusinessLogic.Services.ProductVariantVariants;
using BusinessLogic.Services.Reviews;
using Microsoft.AspNetCore.SignalR;
using Food_Haven.Web.Hubs;
using BusinessLogic.Services.VoucherServices;


namespace Food_Haven.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private HttpClient client = null;
        private string _url;
        private readonly ICartService _cart;
        private readonly IWishlistServices _wishlist;
        private readonly IProductService _product;
        private readonly IProductImageService _productimg;
        private readonly IProductVariantService _productvarian;
        private readonly IStoreDetailService _storeDetailService;
        private readonly IBalanceChangeService _balance;
        private readonly IOrdersServices _order;
        private readonly ICategoryService _categoryService;
        private readonly IReviewService _reviewService;
        private readonly PayOS _payos;
        private readonly IVoucherServices _voucher;

        public HomeController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ICategoryService categoryService, IStoreDetailService storeDetailService, IEmailSender emailSender, ICartService cart, IWishlistServices wishlist, IProductService product
, IProductImageService productimg, IProductVariantService productvarian, IReviewService reviewService, IBalanceChangeService balance, IOrdersServices order, PayOS payos,IVoucherServices voucherServices)
        {
            _reviewService = reviewService;
            _categoryService = categoryService;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            client = new HttpClient();
            var contentype = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentype);
            _cart = cart;
            _wishlist = wishlist;
            _product = product;
            _productimg = productimg;
            _productvarian = productvarian;
            _storeDetailService = storeDetailService;
            _balance = balance;
            _order = order;
            _payos = payos;
            _voucher = voucherServices;
        }

        public IActionResult Index(string searchName, decimal? minPrice = null, decimal? maxPrice = null, int filterCount = 0)
        {
            try
            {
                // Bắt đầu với truy vấn gốc từ service
                var query = _product.GetAll().Where(p => p.IsActive);
                var price = _productvarian.GetAll();

                // Lọc theo tên sản phẩm
                if (!string.IsNullOrEmpty(searchName))
                {
                    query = query.Where(p => p.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase));
                }

                // Lọc theo giá
                if (minPrice.HasValue || maxPrice.HasValue)
                {
                    decimal min = minPrice ?? 0;
                    decimal max = maxPrice ?? decimal.MaxValue;
                    price = price.Where(p => p.SellPrice >= min && p.SellPrice <= max);
                    filterCount++;
                }

                // Thực thi truy vấn và chuyển sang ViewModel nếu cần
                var list = query.Select(p => new ProductsViewModel
                {
                    ID = p.ID,
                    Name = p.Name,
                    LongDescription = p.LongDescription,
                    StoreName = p.StoreDetails.Name,
                    StoreId = p.StoreDetails.ID,

                    // Lấy giá từ biến thể đầu tiên (nếu có)
                    Price = p.ProductTypes
              .OrderBy(v => v.SellPrice) // hoặc FirstOrDefault nếu chỉ cần 1
              .Select(v => v.SellPrice)
              .FirstOrDefault(),

                    // Lấy danh sách ảnh (ví dụ chuỗi URL hoặc danh sách)
                    Img = p.ProductImages
                      .Select(img => img.ImageUrl)
                      .ToList()
                }).ToList();


                // Gán ViewBag
                ViewBag.MinPrice = minPrice ?? 0;
                ViewBag.MaxPrice = maxPrice ?? 2000;
                ViewBag.FilterCount = filterCount;

                return View(list);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong ListProducts: {ex.Message}\n{ex.StackTrace}");

                ViewBag.MinPrice = minPrice ?? 0;
                ViewBag.MaxPrice = maxPrice ?? 2000;
                ViewBag.FilterCount = filterCount;
                ViewBag.ErrorMessage = "An error occurred while loading the product list.";

                return View(new List<ProductsViewModel>());
            }
        }


        [HttpGet]
        public IActionResult Login(string ReturnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl) && ReturnUrl != Url.Action("Login", "Home")
                && ReturnUrl != Url.Action("Register", "Home") && ReturnUrl != Url.Action("Forgot", "Home")
                && ReturnUrl != Url.Action("ResetPassword", "Home")
                )
            {
                ViewData["ReturnUrl"] = ReturnUrl;
            }
            else
            {
                ViewData["ReturnUrl"] = "/";
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, bool? rememberMe, string ReturnUrl = null)
        {

            if (string.IsNullOrWhiteSpace(username))
            {
                return Json(new { status = "error", msg = "Tên Người Dùng Không Được Để Trống" });
            }
            if (string.IsNullOrEmpty(password))
            {
                return Json(new { status = "error", msg = "Mật Khẩu Không Được Để Trống" });
            }
            if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl) && ReturnUrl != Url.Action("Login", "Home")
            && ReturnUrl != Url.Action("Register", "Home") && ReturnUrl != Url.Action("Forgot", "Home")
            && ReturnUrl != Url.Action("ResetPassword", "Home")
            )
            {
                ViewData["ReturnUrl"] = ReturnUrl;
            }
            else
            {
                ViewData["ReturnUrl"] = "/";
            }
            // Xử lý ReturnUrl tương tự GET
            try
            {

                var user = await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);
                if (user == null)
                {
                    return Json(new { status = "error", msg = "Tài khoản không tồn tại" });
                }
                if (user.IsBannedByAdmin)
                {
                    return Json(new { status = "error", msg = "Tài khoản của bạn đã bị khóa bởi quản trị viên." });
                }
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    return Json(new { status = "error", msg = "Bạn phải xác thực email trước khi đăng nhập." });
                }
                var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
                if (!isPasswordValid)
                {
                    await _userManager.AccessFailedAsync(user);
                    var failedAttempts = await _userManager.GetAccessFailedCountAsync(user);

                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        return Json(new { status = "error", msg = "Tài khoản của bạn đã bị khóa do quá nhiều lần đăng nhập thất bại." });
                    }
                    else
                    {
                        return Json(new { status = "error", msg = $"Sai mật khẩu! Bạn còn {5 - failedAttempts} lần thử." });
                    }
                }

                await _userManager.ResetAccessFailedCountAsync(user);
                var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe ?? false, lockoutOnFailure: true);
                if (result.IsLockedOut)
                {
                    return Json(new { status = "error", msg = "Tài khoản của bạn đã bị khóa." });
                }
                var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
                if (isTwoFactorEnabled)
                {
                    var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Authenticator");
                    return Json(new { status = "verify", msg = "Nhập mã xác minh từ ứng dụng xác thực.", token = token });
                }
                if (result.Succeeded)
                {
                    return Ok(new
                    {
                        status = "success",
                        msg = "Đăng nhập thành công",
                        redirectUrl = ReturnUrl

                    });
                }
            }
            catch (Exception e)
            {
                return Json(new { status = "error", msg = "Lỗi không xác định, vui lòng thử lại." });
            }
            return Unauthorized(new { status = "error", msg = "Thông tin đăng nhập không chính xác!" });
        }

        [HttpGet]
        public IActionResult Register(string ReturnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userName = User.Identity.Name;
                return RedirectToAction("Index", "Home");
            }
            // Xử lý ReturnUrl tương tự GET
            if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl) && ReturnUrl != Url.Action("Login", "Home")
                && ReturnUrl != Url.Action("Register", "Home") && ReturnUrl != Url.Action("Forgot", "Home")
                && ReturnUrl != Url.Action("ResetPassword", "Home")
                )
            {
                ViewData["ReturnUrl"] = ReturnUrl;
            }
            else
            {
                ViewData["ReturnUrl"] = "/";
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (ModelState.ContainsKey("Username") && ModelState["Username"].Errors.Any())
                {
                    return Json(new { status = "error", msg = "" + ModelState["Username"].Errors[0].ErrorMessage });
                }

                if (ModelState.ContainsKey("Email") && ModelState["Email"].Errors.Any())
                {
                    return Json(new { status = "error", msg = "" + ModelState["Email"].Errors[0].ErrorMessage });
                }

                if (ModelState.ContainsKey("Password") && ModelState["Password"].Errors.Any())
                {
                    return Json(new { status = "error", msg = "" + ModelState["Password"].Errors[0].ErrorMessage });
                }

                if (ModelState.ContainsKey("repassword") && ModelState["repassword"].Errors.Any())
                {
                    return Json(new { status = "error", msg = "" + ModelState["repassword"].Errors[0].ErrorMessage });
                }

                return Json(new { status = "error", msg = "Dữ liệu không hợp lệ" });
            }

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl) && model.ReturnUrl != Url.Action("Login", "Home")
                && model.ReturnUrl != Url.Action("Register", "Home") && model.ReturnUrl != Url.Action("Forgot", "Home")
                && model.ReturnUrl != Url.Action("ResetPassword", "Home")
                )
            {
                ViewData["ReturnUrl"] = model.ReturnUrl;
            }
            else
            {
                ViewData["ReturnUrl"] = "/";
            }

            try
            {
                var existingUser = await _userManager.FindByNameAsync(model.Username);
                if (existingUser != null)
                {
                    return Json(new { status = "error", msg = "Tên người dùng đã tồn tại. Vui lòng chọn tên khác." });
                }
                var existingEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingEmail != null)
                {
                    return Json(new { status = "error", msg = "Email đã được đăng ký. Vui lòng sử dụng email khác." });
                }
                var user = new AppUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var resultAddrole = await _userManager.AddToRoleAsync(user, "User");
                    if (resultAddrole.Succeeded)
                    {
                        var token = EncryptData.Encrypt(await _userManager.GenerateEmailConfirmationTokenAsync(user), "Xinchao123@");
                        var confirmationLink = Url.Action("Index", "Home", new { userId = Uri.EscapeDataString(EncryptData.Encrypt(user.Id, "Xinchao123@")), token = Uri.EscapeDataString(token) }, Request.Scheme);
                        await _emailSender.SendEmailAsync(user.Email, "Email Verification",
                            $"{TemplateSendmail.TemplateVerifyLinkCode(model.Username, confirmationLink)}");

                        return Json(new { status = "success", msg = "Đăng kí thành công vui lòng xác nhận email." });
                    }
                    return Json(new { status = "error", msg = "Lỗi không xác định, vui lòng liên hệ quản trị." });

                }
                var firstResultError = result.Errors.FirstOrDefault()?.Description;
                return Json(new { status = "error", msg = firstResultError ?? "Đăng ký thất bại." });
            }
            catch (Exception e)
            {
                return Json(new { status = "error", msg = "Lỗi không xác định, vui lòng thử lại." });
            }
        }


        public IActionResult ListProducts(string searchName, decimal? minPrice = null, decimal? maxPrice = null, int filterCount = 0)
        {
            try
            {
                // Chỉ lấy sản phẩm IsActive == true
                var query = _product.GetAll().Where(p => p.IsActive);
                var price = _productvarian.GetAll();

                // Lọc theo tên sản phẩm
                if (!string.IsNullOrEmpty(searchName))
                {
                    query = query.Where(p => p.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase));
                }

                // Lọc theo giá
                if (minPrice.HasValue || maxPrice.HasValue)
                {
                    decimal min = minPrice ?? 0;
                    decimal max = maxPrice ?? decimal.MaxValue;
                    price = price.Where(p => p.SellPrice >= min && p.SellPrice <= max);
                    filterCount++;
                }

                var list = query.Select(p => new ProductsViewModel
                {
                    ID = p.ID,
                    Name = p.Name,
                   
                    LongDescription = p.LongDescription,
                    Price = p.ProductTypes
                        .OrderBy(v => v.SellPrice)
                        .Select(v => v.SellPrice)
                        .FirstOrDefault(),
                    Img = p.ProductImages
                        .Select(img => img.ImageUrl)
                        .ToList()
                }).ToList();

                ViewBag.MinPrice = minPrice ?? 0;
                ViewBag.MaxPrice = maxPrice ?? 2000;
                ViewBag.FilterCount = filterCount;

                return View(list);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ListProducts: {ex.Message}\n{ex.StackTrace}");

                ViewBag.MinPrice = minPrice ?? 0;
                ViewBag.MaxPrice = maxPrice ?? 2000;
                ViewBag.FilterCount = filterCount;
                ViewBag.ErrorMessage = "An error occurred while loading the product list.";

                return View(new List<ProductsViewModel>());
            }
        }

        public async Task<IActionResult> Logout()
        {

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var getUser = await this._userManager.FindByIdAsync(userId);
                getUser.LastAccess = DateTime.Now;
                await this._userManager.UpdateAsync(getUser);
            }
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return Json(new { success = false, message = "Yêu cầu không hợp lệ." });
            try
            {
                userId = EncryptData.Decrypt(Uri.UnescapeDataString(userId), "Xinchao123@");
                token = EncryptData.Decrypt(Uri.UnescapeDataString(token), "Xinchao123@");
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Yêu cầu không hợp lệ." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Json(new { success = false, message = "Không tìm thấy người dùng." });

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return Json(new { success = true, message = "Xác nhận email thành công!" });

            return Json(new { success = false, message = "Liên kết đã hết hạn hoặc không hợp lệ." });
        }

        [HttpPost]
        public async Task<IActionResult> ResendConfirmationEmail(string username)
        {

            try
            {
                var user = await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);
                if (user == null)
                {
                    return BadRequest(new { status = "error", msg = "Tài khoản không tồn tại." });
                }
                if (await _userManager.IsEmailConfirmedAsync(user))
                {
                    return BadRequest(new { status = "error", msg = "Email đã được xác nhận." });
                }
                var token = EncryptData.Encrypt(await _userManager.GenerateEmailConfirmationTokenAsync(user), "Xinchao123@");
                var confirmationLink = Url.Action("Index", "Home", new { userId = Uri.EscapeDataString(EncryptData.Encrypt(user.Id, "Xinchao123@")), token = Uri.EscapeDataString(token) }, Request.Scheme);
                await _emailSender.SendEmailAsync(user.Email, "Email Verification",
                    $"{TemplateSendmail.TemplateVerifyLinkCode(user.UserName, confirmationLink)}");

                return Json(new { status = "success", msg = "Một email xác nhận mới đã được gửi." });
            }
            catch (Exception ex)
            {

                return Json(new { status = "error", msg = "Lỗi không xác định, vui lòng thử lại." });
            }
        }

        [AllowAnonymous]
        public IActionResult LoginWithProdider(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Home");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            if (provider == "Google")
            {
                return Challenge(properties, GoogleDefaults.AuthenticationScheme);
            }
            else if (provider == "Microsoft")
            {
                return Challenge(properties, MicrosoftAccountDefaults.AuthenticationScheme);
            }
            return RedirectToAction("Login");
        }
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return RedirectToAction("Login");
            }

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email
                };

                var createUserResult = await _userManager.CreateAsync(user);

                if (!createUserResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Error creating user.");
                    return RedirectToAction("Login");
                }
                await _userManager.AddToRoleAsync(user, "user");
            }

            await _signInManager.SignInAsync(user, isPersistent: true);

            return RedirectToAction("Index", "Home");
        }




        public async Task<IActionResult> GetAllCategory(string searchName)
        {
            var list = new List<CategoryViewModel>();
            try
            {
                IEnumerable<Categories> categories;

                if (!string.IsNullOrEmpty(searchName))
                {
                    categories = await _categoryService.ListAsync(
                        u => u.Name.ToLower().Contains(searchName.ToLower()),
                        orderBy: x => x.OrderByDescending(s => s.CreatedDate)
                    );
                }
                else
                {
                    categories = await _categoryService.ListAsync(
                        orderBy: x => x.OrderByDescending(c => c.CreatedDate)
                    );
                }

                foreach (var item in categories)
                {
                    list.Add(new CategoryViewModel
                    {
                        ID = item.ID,
                        Name = item.Name,
                        CreatedDate = item.CreatedDate,
                        Commission = item.Commission,
                        Img = item.ImageUrl,
                        ModifiedDate = item.ModifiedDate,

                    });
                }

                return View(list);
            }
            catch (Exception)
            {
                return View(list);
            }
        }



        public async Task<IActionResult> GetAllStore(string searchName)
        {
            var list = new List<StoreViewModel>();

            try
            {
                IEnumerable<StoreDetails> stores;

                if (!string.IsNullOrEmpty(searchName))
                {
                    stores = await _storeDetailService.ListAsync(
                        s => s.IsActive && s.Name.ToLower().Contains(searchName.ToLower()),
                        orderBy: x => x.OrderByDescending(s => s.CreatedDate)
                    );
                }
                else
                {
                    stores = await _storeDetailService.ListAsync(
                        s => s.IsActive,
                        orderBy: x => x.OrderByDescending(s => s.CreatedDate)
                    );
                }

                foreach (var item in stores)
                {
                    list.Add(new StoreViewModel
                    {
                        ID = item.ID,
                        Name = item.Name,
                        Address = item.Address,
                        Phone = item.Phone,
                        Img = item.ImageUrl,
                        CreatedDate = item.CreatedDate,
                        ModifiedDate = item.ModifiedDate,
                        ShortDescriptions = item.ShortDescriptions,
                        LongDescriptions = item.LongDescriptions,
                        IsActive = item.IsActive,
                        Status = item.Status
                    });
                }

                return View(list);
            }
            catch (Exception)
            {
                return View(list); // Trả về view rỗng nếu lỗi
            }
        }

        public async Task<IActionResult> GetStoreDetail(Guid id)
        {
            // 1. Lấy thông tin cửa hàng
            var storeDetails = await _storeDetailService.FindAsync(s => s.ID == id);
            if (storeDetails == null)
            {
                return NotFound("Store not found");
            }

            // 2. Lấy thông tin người dùng tạo cửa hàng
            var user = await _userManager.FindByIdAsync(storeDetails.UserID);

            // 3. Chuẩn bị danh sách sản phẩm
            var productList = new List<ProductsViewModel>();
            var products = await _product.ListAsync(
                p => p.IsActive && p.StoreID == id,
                orderBy: x => x.OrderByDescending(p => p.CreatedDate)
            );

            foreach (var product in products)
            {
                var price = await _productvarian.FindAsync(v => v.ProductID == product.ID && v.IsActive);
                var category = await _categoryService.FindAsync(c => c.ID == product.CategoryID);
                var imageList = await _productimg.ListAsync(i => i.ProductID == product.ID);
                var imageUrls = imageList.Select(i => i.ImageUrl).ToList();

                productList.Add(new ProductsViewModel
                {
                    ID = product.ID,
                    Name = product.Name,
                    CateID = product.CategoryID,
                    CategoryName = category?.Name,
                    StoreId = product.StoreID,
                    StoreName = storeDetails.Name,
                    Price = price?.SellPrice ?? 0,
                    Img = imageUrls,
                    IsActive = product.IsActive,
                    IsOnSale = product.IsOnSale,
                    ShortDescription = product.ShortDescription,
                    LongDescription = product.LongDescription,
                    ManufactureDate = product.ManufactureDate,
                    CreatedDate = product.CreatedDate,
                    ModifiedDate = product.ModifiedDate
                });
            }

            // 4. Chuẩn bị ViewModel chi tiết cửa hàng
            var storeVM = new StoreDetailsViewModels
            {
                ID = storeDetails.ID,
                Name = storeDetails.Name,
                Address = storeDetails.Address,
                Phone = storeDetails.Phone,
                Img = storeDetails.ImageUrl,
                ShortDescriptions = storeDetails.ShortDescriptions,
                LongDescriptions = storeDetails.LongDescriptions,
                CreatedDate = storeDetails.CreatedDate,
                UserID = storeDetails.UserID,
                UserName = user?.UserName,
                ProductViewModel = productList,
                CategoryViewModels = new List<CategoryViewModel>() // nếu sau này cần thêm
            };

            return View(storeVM);
        }

        public async Task<IActionResult> ProductDetail(Guid id)
        {
            // 1. Lấy sản phẩm + cửa hàng
            var productDetail = await _product.FindAsync(x => x.ID == id);
            if (productDetail == null)
                return NotFound();

            // 2. Lấy cửa hàng
            var store = await _storeDetailService.FindAsync(s => s.ID == productDetail.StoreID);
            if (store == null)
                return NotFound("Store not found");

            // 3. Lấy AppUser từ UserID (giống GetStoreDetail)
            var user = await _userManager.FindByIdAsync(store?.UserID);

            // 4. Lấy loại sản phẩm
            var productType = await _productvarian.ListAsync(x => x.ProductID == productDetail.ID);

            // 5. Khởi tạo ViewModel
            var viewModel = new ProductDetailsViewModel
            {
                ID = productDetail.ID,
                Name = productDetail.Name,
                StoreName = store.Name ?? "Không rõ",
                Owner = user?.UserName ?? store?.UserID ?? "Không rõ",
                StoreID = productDetail.StoreID,
                ShortDescription = productDetail.ShortDescription,
                LongDescription = productDetail.LongDescription,
                CreatedDate = productDetail.CreatedDate,
                Stock = productType.FirstOrDefault()?.Stock ?? 0
            };

            // 6. Ảnh sản phẩm
            var productImages = await _productimg.ListAsync(i => i.ProductID == id);
            viewModel.Img = productImages.Select(i => i.ImageUrl).ToList();

            // 7. Biến thể (variants)
            var variants = (await _productvarian.ListAsync(v => v.ProductID == id && v.IsActive)).ToList();
            viewModel.size = variants.Select(v => v.Name).ToList();
            viewModel.Variant = variants.Select(v => new ProductVariantViewModel
            {
                ID = v.ID,
                Name = v.Name,
                Price = v.SellPrice,
                Stock = v.Stock
            }).ToList();

            var firstVariant = variants.FirstOrDefault();
            viewModel.Price = firstVariant?.SellPrice ?? 0;
            viewModel.Stocks = firstVariant?.Stock ?? 0;

            // 8. Danh mục
            var category = await _categoryService.FindAsync(c => c.ID == productDetail.CategoryID);
            viewModel.CategoryName = category?.Name;

            // 9. Bình luận
            var comments = await _reviewService.ListAsync(c => c.ProductID == id);
            viewModel.Comments = comments.Select(c => new CommentViewModels
            {
                Username = c.UserID,
                Cmt = c.Comment,
                Datecmt = c.CommentDate
            }).ToList();

            return View(viewModel);
        }


        public async Task<IActionResult> GetAllProductOfCategory(Guid id)
        {
            // Kiểm tra danh mục có tồn tại không
            var category = await _categoryService.FindAsync(c => c.ID == id);
            if (category == null)
            {
                return NotFound("Category not found");
            }

            // Khởi tạo view model
            var viewModel = new CategoryDetailsViewModel
            {
                ID = category.ID,
                Name = category.Name,

                Commission = category.Commission,
                CreatedDate = category.CreatedDate,
                ModifiedDate = category.ModifiedDate,
                Img = category.ImageUrl,
                ProductViewModel = new List<ProductsViewModel>(),
                StoreDetailViewModel = new List<StoreDetailsViewModels>()
            };

            // Lấy danh sách sản phẩm thuộc danh mục
            var products = await _product.ListAsync(
                p => p.IsActive && p.CategoryID == id,
                orderBy: x => x.OrderByDescending(p => p.CreatedDate)
            );

            foreach (var product in products)
            {
                // Lấy thông tin bổ sung cho từng sản phẩm
                var price = await _productvarian.FindAsync(v => v.ProductID == product.ID && v.IsActive);
                var store = await _storeDetailService.FindAsync(s => s.ID == product.StoreID);
                var imageList = await _productimg.ListAsync(i => i.ProductID == product.ID);
                var imageUrls = imageList.Select(i => i.ImageUrl).ToList();

                var productVM = new ProductsViewModel
                {
                    ID = product.ID,
                    Name = product.Name,
                    CateID = product.CategoryID,
                    CategoryName = category.Name,
                    StoreId = product.StoreID,
                    StoreName = store?.Name,
                    Price = price?.SellPrice ?? 0,
                    Img = imageUrls,
                    IsActive = product.IsActive,
                    IsOnSale = product.IsOnSale,
                    ShortDescription = product.ShortDescription,
                    LongDescription = product.LongDescription,
                    ManufactureDate = product.ManufactureDate,
                    CreatedDate = product.CreatedDate,
                    ModifiedDate = product.ModifiedDate
                };

                viewModel.ProductViewModel.Add(productVM);
            }

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult Forgot()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Forgot(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Json(new { status = "error", msg = "Email không hợp lệ" });
            }
            try
            {

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return Json(new { status = "error", msg = "Email không tồn tại trong hệ thống." });
                }
                var token = EncryptData.Encrypt(await _userManager.GeneratePasswordResetTokenAsync(user), "Xinchao123@");
                var confirmationLink = Url.Action("ResetPassword", "Home", new { email = Uri.EscapeDataString(EncryptData.Encrypt(user.Email, "Xinchao123@")), token = Uri.EscapeDataString(token) }, Request.Scheme);
                await _emailSender.SendEmailAsync(user.Email, "Request forgot password",
                    $"{TemplateSendmail.TemplateResetPassword(confirmationLink)}");
                return Json(new { status = "success", msg = "Đăng kí thành công vui lòng xác nhận email." });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { status = "error", msg = "Lỗi không xác định, vui lòng thử lại." });
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return BadRequest("Invalid request");
            }

            return View(new ResetPasswordViewModel { Token = token, Email = email });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (ModelState.ContainsKey("Password") && ModelState["Password"].Errors.Any())
                {
                    return Json(new { status = "error", msg = "" + ModelState["Password"].Errors[0].ErrorMessage });
                }

                if (ModelState.ContainsKey("ConfirmPassword") && ModelState["ConfirmPassword"].Errors.Any())
                {
                    return Json(new { status = "error", msg = "" + ModelState["ConfirmPassword"].Errors[0].ErrorMessage });
                }
            }

            var user = await _userManager.FindByEmailAsync(EncryptData.Decrypt(Uri.UnescapeDataString(model.Email), "Xinchao123@"));
            if (user == null)
            {
                return Json(new { status = "error", msg = "Email không tồn tại trong hệ thống." });
            }

            var result = await _userManager.ResetPasswordAsync(user, EncryptData.Decrypt(Uri.UnescapeDataString(model.Token), "Xinchao123@"), model.Password);
            if (result.Succeeded)
            {
                return Json(new { status = "success", msg = "Mật khẩu đã được thay đổi thành công." });
            }

            var firstResultError = result.Errors.FirstOrDefault()?.Description;
            if (firstResultError == "Invalid token.")
            {
                firstResultError = "Link cập nhật mật khẩu đã hết hạn!!";
            }

            return Json(new { status = "error", msg = firstResultError ?? "Cập nhật mật khẩu thất bại." });

        }
        public async Task<IActionResult> Cart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Lấy danh sách giỏ hàng theo User
            var carts = await _cart.ListAsync(x => x.UserID == user.Id);

            // Lấy tất cả biến thể sản phẩm (chỉ lấy active)
            var productVariantIds = carts.Select(c => c.ProductTypesID).ToList();
            var productVariants = await _productvarian.ListAsync(v => productVariantIds.Contains(v.ID) && v.IsActive);

            // Lấy danh sách ProductID từ các biến thể để truy vấn sản phẩm
            var productIds = productVariants.Select(v => v.ProductID).Distinct().ToList();
            var products = await _product.ListAsync(p => productIds.Contains(p.ID));

            var result = new List<CartViewModels>();

            foreach (var cart in carts)
            {
                // Tìm biến thể từ giỏ hàng
                var variant = productVariants.FirstOrDefault(v => v.ID == cart.ProductTypesID);
                if (variant == null)
                {
                    continue; // Không có biến thể hợp lệ
                }

                // Tìm sản phẩm từ biến thể
                var product = products.FirstOrDefault(p => p.ID == variant.ProductID);
                if (product == null)
                {
                    continue; // Không có sản phẩm tương ứng
                }

                // Lấy ảnh sản phẩm
                var productImg = await _productimg.FindAsync(x => x.ProductID == product.ID);

                // Tạo view model cho giỏ hàng
                var cartItem = new CartViewModels
                {
                    ProductTypeID = cart.ProductTypesID,
                    CartID = cart.ID,
                    ProductID = product.ID,
                    ProductName = product.Name ?? "Không có tên",
                    quantity = cart.Quantity,
                    price = variant.SellPrice,
                    Subtotal = cart.Quantity * variant.SellPrice,
                    img = productImg?.ImageUrl ?? "/images/default.jpg",
                    Stock = variant.Stock,
                    ProductTyName = variant.Name,
                };

                result.Add(cartItem);
            }

            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCart([FromBody] CartViewModels obj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            try
            {
                var cartItem = await _cart.FindAsync(z => z.ID == obj.CartID && z.UserID == user.Id);
                if (cartItem == null)
                {
                    return BadRequest(new { success = false, message = "Product not found in the cart." });
                }

                await _cart.DeleteAsync(cartItem);
                await _cart.SaveChangesAsync();

                return Json(new { success = true, message = "Product deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the product.", error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartViewModels obj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "You are not logged in." });
            }
            /*             var product = await _product.FindAsync(x => x.ID == obj.ProductID);
             */            // 🔍 Check if the product variant exists
            var productVariant = await _productvarian.FindAsync(x => x.ID == obj.ProductTypeID);
            if (productVariant == null)
            {
                return Json(new { success = false, message = "Product does not exist!" });
            }

            // 🔥 Check stock quantity
            if (obj.quantity > productVariant.Stock)
            {
                return Json(new { success = false, message = $"Quantity exceeds stock! Only {productVariant.Stock} items left." });
            }

            // 🔍 Check if cart item already exists
            var existingCartItem = await _cart.FindAsync(x => x.UserID == user.Id && x.ProductTypesID == productVariant.ID);
            int currentQuantity = existingCartItem?.Quantity ?? 0;
            int newTotalQuantity = currentQuantity + obj.quantity;

            if (newTotalQuantity > productVariant.Stock)
            {
                return Json(new { success = false, message = $"Quantity exceeds stock! Only {productVariant.Stock} items available." });
            }

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += obj.quantity;
                await _cart.UpdateAsync(existingCartItem);
            }
            else
            {
                var newCart = new Cart
                {
                    ID = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    UserID = user.Id,
                    ProductTypesID = productVariant.ID,
                    Quantity = obj.quantity
                };
                await _cart.AddAsync(newCart);
            }
            var hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<CartHub>>();
            await hubContext.Clients.User(user.Id).SendAsync("ReceiveCartUpdate");
            await _cart.SaveChangesAsync();

            return Json(new { success = true, message = "Added to cart successfully!" });
        }
        [HttpPost]
        public async Task<IActionResult> CheckQuantity([FromBody] CartViewModels obj)
        {
            // Kiểm tra người dùng đăng nhập
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new { message = "Vui lòng đăng nhập để thực hiện thao tác này." });
            }

            // 🔍 Tìm ProductVarian theo ProductTypeID
            var productVarian = await _productvarian.FindAsync(x => x.ID == obj.ProductTypeID);
            if (productVarian == null)
            {
                return NotFound(new { message = "Loại sản phẩm không tồn tại." });
            }

            // 🔍 Lấy Product dựa trên ProductID từ ProductVarian
            var product = await _product.FindAsync(x => x.ID == productVarian.ProductID);
            if (product == null)
            {
                return NotFound(new { message = "Sản phẩm không tồn tại." });
            }

            // 🔍 Tìm cart item theo ProductTypeID và UserID
            var cartItem = await _cart.FindAsync(x => x.ProductTypesID == obj.ProductTypeID && x.UserID == user.Id);
            if (cartItem == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ hàng." });
            }

            int currentQuantity = cartItem.Quantity;

            // 🚨 Kiểm tra tồn kho khi tăng số lượng
            if (obj.quantity > currentQuantity && obj.quantity > productVarian.Stock)
            {
                var hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<CartHub>>();
                // Gửi thông báo realtime tồn kho cập nhật cho tất cả client
                await hubContext.Clients.All.SendAsync("StockUpdated", obj.ProductTypeID, productVarian.Stock);

                return BadRequest(new
                {
                    success = false,
                    message = $"Số lượng vượt quá tồn kho. Chỉ còn {productVarian.Stock} sản phẩm.",
                    isMaxStock = true
                });
            }

            // ✅ Cập nhật số lượng trong giỏ hàng
            cartItem.Quantity = obj.quantity;
            await _cart.UpdateAsync(cartItem);
            await _cart.SaveChangesAsync();

            // ✅ Trả về Product.ID cùng với kết quả
            return Ok(new
            {
                success = true,
                message = "Cập nhật số lượng thành công.",
                productId = product.ID // Trả về Product.ID (ví dụ: 14ac68f0-85a7-45d1-83bb-3d8d2f092f0b)
            });
        }
        [HttpGet]
        public async Task<IActionResult> CartPart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return PartialView("_Cart", new List<CartViewModels>());
            }

            var carts = await _cart.ListAsync(x => x.UserID == user.Id);
            var productVariantIds = carts.Select(c => c.ProductTypesID).ToList();
            var productVariants = await _productvarian.ListAsync(v => productVariantIds.Contains(v.ID) && v.IsActive);
            var productIds = productVariants.Select(v => v.ProductID).ToList();
            var products = await _product.ListAsync(p => productIds.Contains(p.ID));

            var result = new List<CartViewModels>();

            foreach (var cart in carts)
            {
                var variant = productVariants.FirstOrDefault(v => v.ID == cart.ProductTypesID);
                if (variant == null) continue;

                var product = products.FirstOrDefault(p => p.ID == variant.ProductID);
                if (product == null) continue;

                var productImg = await _productimg.FindAsync(x => x.ProductID == product.ID);

                var cartItem = new CartViewModels
                {
                    ProductTypeID = cart.ProductTypesID,
                    CartID = cart.ID,
                    ProductID = cart.ProductTypesID,
                    ProductName = product.Name ?? "Không có tên",
                    quantity = cart.Quantity,
                    price = variant.SellPrice,
                    Subtotal = cart.Quantity * variant.SellPrice,
                    img = productImg?.ImageUrl ?? "/images/default.jpg",
                    Stock = variant.Stock,
                    ProductTyName = variant.Name,
                };
                /*    var hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<CartHub>>();
                   await hubContext.Clients.User(user.Id).SendAsync("ReceiveCartUpdate"); */
                result.Add(cartItem);
            }

            return PartialView("_Cart", result);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAllCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "You are not logged in." });
            }
            try
            {
                var cartItem = await _cart.ListAsync(x => x.UserID == user.Id);
                foreach (var item in cartItem)
                {
                    await _cart.DeleteAsync(item);
                }
                await _cart.SaveChangesAsync();
                return Json(new { success = true, message = "All products deleted from cart." });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while deleting all products.", error = ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> AddWish(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { notAuth = true, message = "Bạn phải đăng nhập thể thực hiện hành động này!" });
            }
            var check = await this._product.FindAsync(x => x.ID == id);
            if (check == null)
            {
                return Json(new { success = false, message = "Product không tồn tại!!" });
            }
            else if (await this._wishlist.FindAsync(x => x.ProductID == id && x.UserID == user.Id) != null)
            {
                return Json(new { success = false, message = $"{check.Name} đã tồn tại trong danh sách yêu thích.!" });
            }
            else
            {
                var tem = new Wishlist
                {
                    CreateDate = DateTime.Now,
                    UserID = user.Id,
                    ProductID = check.ID
                };
                try
                {
                    await this._wishlist.AddAsync(tem);
                    await this._wishlist.SaveChangesAsync();
                    return Json(new { success = true, message = $"Thêm thành công!" });
                }
                catch
                {
                    return Json(new { success = false, message = "Thêm thất bại!" });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]

        public async Task<IActionResult> RemoveWish(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { notAuth = true, message = "Bạn phải đăng nhập thể thực hiện hành động này!" });
            }
            var check = await this._product.FindAsync(x => x.ID == id);
            if (check == null)
            {
                return Json(new { success = false, message = "Product không tồn tại!!" });
            }
            var wshlict = await this._wishlist.FindAsync(x => x.ProductID == id && x.UserID == user.Id);
            if (wshlict == null)
            {
                return Json(new { success = false, message = $"{check.Name} không tồn tại trong danh sách yêu thích.!" });
            }
            else
            {

                try
                {
                    await this._wishlist.DeleteAsync(wshlict);
                    await this._wishlist.SaveChangesAsync();
                    return Json(new { success = true, message = $"Xoa thành công!" });
                }
                catch
                {
                    return Json(new { success = false, message = "Xoa thất bại!" });
                }
            }
        }

        public async Task<IActionResult> Wishlist()
        {
            var list = new List<wishlistViewModels>();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var wshlict = await this._wishlist.ListAsync(x => x.UserID == user.Id, orderBy: q => q.OrderByDescending(s => s.CreateDate));
            if (wshlict.Any())
            {
                foreach (var item in wshlict)
                {
                    var getProduct = await this._product.FindAsync(p => p.ID == item.ProductID);
                    if (getProduct != null)
                    {
                        var getimg = await this._productimg.ListAsync(u => u.ProductID == getProduct.ID);
                        var img = "https://nest-frontend-v6.vercel.app/assets/imgs/shop/product-1-1.jpg";
                        if (getimg.Any())
                        {
                            img = getimg.FirstOrDefault().ImageUrl;
                        }
                        var defauPrice = 0.0m;

                        var getPrice = await this._productvarian.FindAsync(u => u.ProductID == getProduct.ID);
                        if (getPrice != null)
                        {
                            defauPrice = getPrice.SellPrice;
                        }
                        list.Add(new wishlistViewModels
                        {
                            ID = item.ID,
                            img = img,
                            name = getProduct.Name,
                            price = defauPrice,
                            ProductID = getProduct.ID,
                            vote = 100
                        });
                    }
                }
            }
            return View(list);
        }
        [Route("Error/404")]
        public IActionResult NotFoundPage()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetBalance()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new ErroMess { msg = "Bạn phải đăng nhập thể thực hiện hành động này!" });
            }
            var balance = await this._balance.GetBalance(user.Id);
            return Json(new ErroMess { success = true, msg = $"{balance}" });
        }
        [HttpGet]
        public async Task<IActionResult> SearchCategoryList()
        {
            try
            {
                var list = await _categoryService.ListAsync();
                var listCategoryViewModel = list.Select(x => new CategoryViewModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    CreatedDate = x.CreatedDate,
                    Commission = x.Commission,
                    /*  Img = x.ImageUrl, */
                    ModifiedDate = x.ModifiedDate
                }).ToList();
                return PartialView("_SearchCategoryList", listCategoryViewModel);
            }
            catch (System.Exception)
            {
                throw;
            }

        }
        [HttpGet]
        public async Task<IActionResult> SearchProductList(string searchName)
        {
            var list = await _product.ListAsync();

            if (!string.IsNullOrEmpty(searchName))
            {
                list = list.Where(x => x.Name != null && x.Name.ToLower().Contains(searchName.ToLower())).ToList();
            }
            if (string.IsNullOrWhiteSpace(searchName))
            {
                // Redirect về trang gốc (ví dụ Index)
                return RedirectToAction("Index", "Home");
            }

            var listProductViewModel = list.Select(x => new ProductsViewModel
            {
                ID = x.ID,
                Name = x.Name,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                ShortDescription = x.ShortDescription,
                LongDescription = x.LongDescription,
                IsActive = x.IsActive,
                IsOnSale = x.IsOnSale,
                StoreId = x.StoreID,
            }).ToList();

            ViewData["SearchKeyword"] = searchName;
            // Trả về view Index (trong thư mục /Views/Product nếu controller là ProductController)
            return View("Index", listProductViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> GetVariantPrice(Guid variantId)
        {
            if (variantId == Guid.Empty)
            {
                return BadRequest(new { Message = "Invalid variant ID." });
            }

            var variant = await _productvarian.FindAsync(v => v.ID == variantId);
            if (variant == null)
            {
                return NotFound(new { Message = "Variant not found." });
            }

            return Json(new
            {
                Price = (double)variant.SellPrice,
                VariantId = variant.ID,
                Stock = variant.Stock,
            });
        }
        [HttpGet("voucher/get-all")]
        public IActionResult GetAllVouchers()
        {
            var data = _voucher.GetAll().Select(v => new
            {
                id = v.ID,
                code = v.Code,
                title = v.DiscountType == "Percent" ? $"Giảm {v.DiscountAmount}%" : $"Giảm ₫{v.DiscountAmount:n0}",
                minOrder = $"₫{v.MinOrderValue:n0}",
                expire = v.ExpirationDate.ToString("dd.MM.yyyy"),
                count = v.MaxUsage - v.CurrentUsage,
                disabled = !v.IsActive || v.ExpirationDate < DateTime.Now,
                isStoreVoucher = !v.IsGlobal,
                maxDiscountAmount = v.MaxDiscountAmount.HasValue ? $"₫{v.MaxDiscountAmount.Value:n0}" : null
            }).ToList();

            return Json(data);
        }
        [HttpPost("voucher/calculate-discount")]
        public IActionResult CalculateDiscount([FromBody] DiscountRequest request)
        {
            if (string.IsNullOrEmpty(request.Code) || request.OrderTotal <= 0)
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });

            var v = _voucher.GetAll().FirstOrDefault(x => x.Code == request.Code && x.IsActive);
            if (v == null)
                return NotFound(new { message = "Voucher không hợp lệ" });

            if (request.OrderTotal < v.MinOrderValue)
                return BadRequest(new { message = "Không đủ điều kiện áp dụng" });

            decimal discount = v.DiscountType == "Percent"
                ? request.OrderTotal * v.DiscountAmount / 100
                : v.DiscountAmount;

            if (v.MaxDiscountAmount.HasValue)
                discount = Math.Min(discount, v.MaxDiscountAmount.Value);

            var finalAmount = request.OrderTotal - discount;

            return Ok(new
            {
                discountAmount = discount,
                orderTotalAfterDiscount = finalAmount,
                discountType = v.DiscountType, // "Percent" hoặc "Fixed"
                code = v.Code
            });
        }
        [HttpGet("voucher/search")]
        public async Task<IActionResult> SearchVoucher(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest(new { message = "Thiếu mã voucher." });

            var result = await _voucher.ListAsync(
                filter: x => x.Code.ToLower() == code.ToLower()
            );

            var v = result.Select(v => new
            {
                id = v.ID,
                code = v.Code,
                title = v.DiscountType == "Percent"
                    ? $"Giảm {v.DiscountAmount}%"
                    : $"Giảm ₫{v.DiscountAmount:n0}",
                minOrder = $"₫{v.MinOrderValue:n0}",
                expire = v.ExpirationDate.ToString("dd.MM.yyyy"),
                count = v.MaxUsage - v.CurrentUsage,
                disabled = !v.IsActive || v.ExpirationDate < DateTime.Now,
                isStoreVoucher = !v.IsGlobal,
                maxDiscountAmount = v.MaxDiscountAmount.HasValue ? $"₫{v.MaxDiscountAmount.Value:n0}" : null
            }).FirstOrDefault();

            if (v == null)
                return NotFound(new { message = "Không tìm thấy voucher." });

            return Json(v);
        }


    }

}
