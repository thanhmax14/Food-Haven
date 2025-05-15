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

        private readonly PayOS _payos;



        public HomeController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ICategoryService categoryService, IStoreDetailService storeDetailService, IEmailSender emailSender, ICartService cart, IWishlistServices wishlist, IProductService product
, IProductImageService productimg, IProductVariantService productvarian, IBalanceChangeService balance, IOrdersServices order, PayOS payos)
        {
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
        }

        public IActionResult Index()
        {         
            return View();
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

                    }) ;
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
                   var resultAddrole= await _userManager.AddToRoleAsync(user, "User");
                    if (resultAddrole.Succeeded)
                    {
                        var token = EncryptData.Encrypt(await _userManager.GenerateEmailConfirmationTokenAsync(user), "Xinchao123@");
                        var confirmationLink = Url.Action("Index", "Home", new { userId = Uri.EscapeDataString(EncryptData.Encrypt(user.Id, "Xinchao123@")), token = Uri.EscapeDataString(token) }, Request.Scheme);
                        await _emailSender.SendEmailAsync(user.Email, "Email Verification",
                            $"{TemplateSendmail.TemplateVerifyLinkCode(model.Username,confirmationLink)}");

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
                // Bắt đầu với truy vấn gốc từ service
                var query = _product.GetAll();
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
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi tải danh sách sản phẩm.";

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


    }


}
