using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using BusinessLogic.Hash;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.ExpertRecipes;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.RecipeServices;
using BusinessLogic.Services.RecipeViewHistorys;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using BusinessLogic.Services.StoreFollowers;
using BusinessLogic.Services.StoreReports;
using BusinessLogic.Services.VoucherServices;
using BusinessLogic.Services.Wishlists;
using Food_Haven.Web.Hubs;
using Food_Haven.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models;
using Net.payOS;
using Repository.ViewModels;
using X.PagedList;



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
        private readonly IOrderDetailService _orderDetail;
        private readonly ICategoryService _categoryService;
        private readonly IReviewService _reviewService;
        private readonly PayOS _payos;
        private readonly IVoucherServices _voucher;
        private readonly IRecipeService _recipeService;
        private readonly IStoreReportServices _storeReport;
        private readonly IStoreFollowersService _storeFollowersService;
        private readonly RecipeSearchService _service;
        private readonly IExpertRecipeServices _expertRecipeServices;
        private readonly IRecipeViewHistoryServices _recipeViewHistoryServices;
        public bool IsTesting { get; set; } = false;

        public HomeController(SignInManager<AppUser> signInManager, IOrderDetailService orderDetail, IRecipeService recipeService, UserManager<AppUser> userManager, ICategoryService categoryService, IStoreDetailService storeDetailService, IEmailSender emailSender, ICartService cart, IWishlistServices wishlist, IProductService product
, IProductImageService productimg, IProductVariantService productvarian, IReviewService reviewService, IBalanceChangeService balance, IOrdersServices order, PayOS payos, IVoucherServices voucherServices, IStoreReportServices storeReport, IStoreFollowersService storeFollowersService, RecipeSearchService service,
            IExpertRecipeServices expertRecipeServices, IRecipeViewHistoryServices recipeViewHistoryServices)

        {
            _recipeService = recipeService;
            _orderDetail = orderDetail;
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
            _storeReport = storeReport;
            _storeFollowersService = storeFollowersService;
            _service = service;
            this._expertRecipeServices = expertRecipeServices;
            _recipeViewHistoryServices = recipeViewHistoryServices;
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
                return Json(new { status = "error", msg = "Username cannot be empty" });
            }
            if (string.IsNullOrEmpty(password))
            {
                return Json(new { status = "error", msg = "Password cannot be empty" });
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
                    return Json(new { status = "error", msg = "Account does not exist" });
                }
                if (user.IsBannedByAdmin)
                {
                    return Json(new { status = "error", msg = "Your account has been locked by the administrator." });
                }
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    return Json(new { status = "notcomfirm", msg = "You must verify your email before logging in." });
                }
                var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
                if (!isPasswordValid)
                {
                    await _userManager.AccessFailedAsync(user);
                    var failedAttempts = await _userManager.GetAccessFailedCountAsync(user);

                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        return Json(new { status = "error", msg = "Your account has been locked due to too many failed login attempts." });
                    }
                    else
                    {
                        return Json(new { status = "error", msg = $"Incorrect password! You have {5 - failedAttempts} attempts left." });
                    }
                }

                await _userManager.ResetAccessFailedCountAsync(user);
                var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe ?? false, lockoutOnFailure: true);
                if (result.IsLockedOut)
                {
                    return Json(new { status = "error", msg = "Your account has been locked." });
                }
                var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
                if (isTwoFactorEnabled)
                {
                    var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Authenticator");
                    return Json(new { status = "verify", msg = "Enter the verification code from your authenticator app.", token = token });
                }
                if (result.Succeeded)
                {

                    var roles = await _userManager.GetRolesAsync(user);

                    string redirectUrl = ReturnUrl;

                    if (roles.Contains("Admin"))
                    {
                        redirectUrl = "/Admin";
                    }

                    return Ok(new
                    {
                        status = "success",
                        msg = "Login successful",
                        redirectUrl = redirectUrl
                    });
                }
            }
            catch (Exception e)
            {
                return Json(new { status = "error", msg = "Unknown error, please try again." });
            }
            return Unauthorized(new { status = "error", msg = "Incorrect login information!" });
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

                return Json(new { status = "error", msg = "Invalid data" });

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
                    return Json(new { status = "error", msg = "Username already exists. Please choose a different username." });
                }
                var existingEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingEmail != null)
                {
                    return Json(new { status = "error", msg = "Email has already been registered. Please use a different email." });
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
                        if (!IsTesting && Request?.Scheme != null && Url != null)
                        {
                            var token = EncryptData.Encrypt(await _userManager.GenerateEmailConfirmationTokenAsync(user), "Xinchao123@");
                            var confirmationLink = Url.Action("Index", "Home", new
                            {
                                userId = Uri.EscapeDataString(EncryptData.Encrypt(user.Id, "Xinchao123@")),
                                token = Uri.EscapeDataString(token)
                            }, Request.Scheme);

                            await _emailSender.SendEmailAsync(user.Email, "Email Verification",
                                $"{TemplateSendmail.TemplateVerifyLinkCode(model.Username, confirmationLink)}");
                        }

                        return Json(new { status = "success", msg = "Registration successful, please confirm your email." });
                    }
                    return Json(new { status = "error", msg = "Unknown error, please contact the administrator." });
                }
                var firstResultError = result.Errors.FirstOrDefault()?.Description;
                return Json(new { status = "error", msg = firstResultError ?? "Registration failed." });
            }
            catch (Exception e)
            {
                return Json(new { status = "error", msg = "Unknown error, please try again." });
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
                return Json(new { success = false, message = "Invalid request." });
            try
            {
                userId = EncryptData.Decrypt(Uri.UnescapeDataString(userId), "Xinchao123@");
                token = EncryptData.Decrypt(Uri.UnescapeDataString(token), "Xinchao123@");
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Invalid request." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Json(new { success = false, message = "User not found." });

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return Json(new { success = true, message = "Email confirmed successfully!" });

            return Json(new { success = false, message = "The link has expired or is invalid." });
        }

        [HttpPost]
        public async Task<IActionResult> ResendConfirmationEmail(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);
                if (user == null)
                {
                    return BadRequest(new { status = "error", msg = "Account does not exist." });
                }
                if (await _userManager.IsEmailConfirmedAsync(user))
                {
                    return BadRequest(new { status = "error", msg = "Email has already been confirmed." });
                }
                var token = EncryptData.Encrypt(await _userManager.GenerateEmailConfirmationTokenAsync(user), "Xinchao123@");
                var confirmationLink = Url.Action("Index", "Home", new { userId = Uri.EscapeDataString(EncryptData.Encrypt(user.Id, "Xinchao123@")), token = Uri.EscapeDataString(token) }, Request.Scheme);
                await _emailSender.SendEmailAsync(user.Email, "Email Verification",
                    $"{TemplateSendmail.TemplateVerifyLinkCode(user.UserName, confirmationLink)}");

                return Json(new { status = "success", msg = "A new confirmation email has been sent." });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", msg = "Unknown error, please try again." });
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
                        Img = !string.IsNullOrEmpty(item.ImageUrl) ? "/uploads/" + item.ImageUrl : "/uploads/default.png",
                        // Img = item.ImageUrl,
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
            var users = await _userManager.GetUserAsync(User);
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
                var price = await _productvarian.ListAsync(v => v.ProductID == product.ID && v.IsActive);
                if (price.Any())
                {
                    var getCate = await this._categoryService.FindAsync(c => c.ID == product.CategoryID);
                    var reviews = await _reviewService.ListAsync(r => r.ProductID == product.ID);
                    var store = await _storeDetailService.FindAsync(s => s.ID == product.StoreID);
                    var imageList = await _productimg.ListAsync(i => i.ProductID == product.ID);
                    var imageUrls = imageList.Select(i => i.ImageUrl).ToList();
                    var flag = false;
                    var checkwwith = await this._wishlist.FindAsync(x => x.UserID == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.ProductID == product.ID);
                    if (checkwwith != null)
                    {
                        flag = true;
                    }
                    var productVM = new ProductsViewModel
                    {
                        ID = product.ID,
                        Name = product.Name,
                        CateID = product.CategoryID,
                        CategoryName = getCate.Name,
                        StoreId = product.StoreID,
                        StoreName = store?.Name,
                        ProductTypes = price.ToList(),
                        Img = imageUrls,
                        IsActive = product.IsActive,
                        IsOnSale = product.IsOnSale,
                        ShortDescription = product.ShortDescription,
                        LongDescription = product.LongDescription,
                        ManufactureDate = product.ManufactureDate,
                        CreatedDate = product.CreatedDate,
                        ModifiedDate = product.ModifiedDate,
                        Reviews = reviews.ToList(),
                        isWish = flag,
                    };

                    productList.Add(productVM);
                }
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
                CategoryViewModels = new List<CategoryViewModel>(), // nếu sau này cần thêm
                Email = user.Email,
                UserNameRepo = user.UserName
            };

            return View(storeVM);
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
                Img = !string.IsNullOrEmpty(category.ImageUrl) ? "/uploads/" + category.ImageUrl : "/uploads/default.png",
                //Img = category.ImageUrl,
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
                var price = await _productvarian.ListAsync(v => v.ProductID == product.ID && v.IsActive);
                if (price.Any())
                {
                    var reviews = await _reviewService.ListAsync(r => r.ProductID == product.ID);
                    var store = await _storeDetailService.FindAsync(s => s.ID == product.StoreID);
                    var imageList = await _productimg.ListAsync(i => i.ProductID == product.ID);
                    var imageUrls = imageList.Select(i => i.ImageUrl).ToList();
                    var flag = false;
                    var checkwwith = await this._wishlist.FindAsync(x => x.UserID == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.ProductID == product.ID);
                    if (checkwwith != null)
                    {
                        flag = true;
                    }
                    var productVM = new ProductsViewModel
                    {
                        ID = product.ID,
                        Name = product.Name,
                        CateID = product.CategoryID,
                        CategoryName = category.Name,
                        StoreId = product.StoreID,
                        StoreName = store?.Name,
                        ProductTypes = price.ToList(),
                        Img = imageUrls,
                        IsActive = product.IsActive,
                        IsOnSale = product.IsOnSale,
                        ShortDescription = product.ShortDescription,
                        LongDescription = product.LongDescription,
                        ManufactureDate = product.ManufactureDate,
                        CreatedDate = product.CreatedDate,
                        ModifiedDate = product.ModifiedDate,
                        Reviews = reviews.ToList(),
                        isWish = flag,
                    };

                    viewModel.ProductViewModel.Add(productVM);
                }
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
                return Json(new { status = "error", msg = "Invalid email" });
            }
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return Json(new { status = "error", msg = "Email does not exist in the system." });
                }
                var token = EncryptData.Encrypt(await _userManager.GeneratePasswordResetTokenAsync(user), "Xinchao123@");
                var confirmationLink = Url.Action("ResetPassword", "Home", new { email = Uri.EscapeDataString(EncryptData.Encrypt(user.Email, "Xinchao123@")), token = Uri.EscapeDataString(token) }, Request.Scheme);
                await _emailSender.SendEmailAsync(user.Email, "Request forgot password",
                    $"{TemplateSendmail.TemplateResetPassword(confirmationLink)}");
                return Json(new { status = "success", msg = "A password reset email has been sent. Please check your email." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { status = "error", msg = "Unknown error, please try again." });
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
                return Json(new { status = "error", msg = "Email does not exist in the system." });
            }

            var result = await _userManager.ResetPasswordAsync(user, EncryptData.Decrypt(Uri.UnescapeDataString(model.Token), "Xinchao123@"), model.Password);
            if (result.Succeeded)
            {
                return Json(new { status = "success", msg = "Password has been changed successfully." });
            }

            var firstResultError = result.Errors.FirstOrDefault()?.Description;
            if (firstResultError == "Invalid token.")
            {
                firstResultError = "The password reset link has expired!";
            }

            return Json(new { status = "error", msg = firstResultError ?? "Password update failed." });
        }

        public async Task<IActionResult> Cart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var carts = await _cart.ListAsync(x => x.UserID == user.Id);

            var productVariantIds = carts.Select(c => c.ProductTypesID).ToList();
            var productVariants = await _productvarian.ListAsync(v => productVariantIds.Contains(v.ID) && v.IsActive);

            var productIds = productVariants.Select(v => v.ProductID).Distinct().ToList();
            var products = await _product.ListAsync(p => productIds.Contains(p.ID));

            // ✅ Lấy StoreID từ danh sách sản phẩm
            var storeIds = products.Select(p => p.StoreID).Distinct().ToList();
            var stores = await _storeDetailService.ListAsync(s => storeIds.Contains(s.ID)); // lấy toàn bộ store liên quan

            var groupedByStore = carts
                .GroupBy(cart =>
                {
                    var variant = productVariants.FirstOrDefault(v => v.ID == cart.ProductTypesID);
                    var product = products.FirstOrDefault(p => p.ID == variant?.ProductID);
                    return product?.StoreID ?? Guid.Empty;
                });

            var result = new List<StoreCartViewModel>();

            foreach (var group in groupedByStore)
            {
                var storeCartItems = new List<CartViewModels>();
                var storeId = group.Key;
                var store = stores.FirstOrDefault(s => s.ID == storeId); // ✅ tìm Store theo ID

                foreach (var cart in group)
                {
                    var variant = productVariants.FirstOrDefault(v => v.ID == cart.ProductTypesID);
                    if (variant == null) continue;

                    var product = products.FirstOrDefault(p => p.ID == variant.ProductID);
                    if (product == null) continue;

                    var productImg = await _productimg.FindAsync(x => x.ProductID == product.ID);

                    storeCartItems.Add(new CartViewModels
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
                        StoreID = storeId,
                        StoreName = store?.Name ?? "Không rõ cửa hàng"
                    });
                }

                if (storeCartItems.Any())
                {
                    result.Add(new StoreCartViewModel
                    {
                        StoreID = storeId,
                        StoreName = store?.Name ?? "Không rõ cửa hàng",
                        CartItems = storeCartItems
                    });
                }
            }

            return View(result); // model: List<StoreCartViewModel>
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
                quantity = cartItem.Quantity,
                productId = obj.ProductTypeID // ✅ Đúng với data-id ở phía client
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
                return Json(new { notAuth = true, message = "You must be logged in to perform this action!" });
            }
            var check = await this._product.FindAsync(x => x.ID == id);
            if (check == null)
            {
                return Json(new { success = false, message = "Product does not exist!" });
            }
            else if (await this._wishlist.FindAsync(x => x.ProductID == id && x.UserID == user.Id) != null)
            {
                return Json(new { success = false, message = $"{check.Name} already exists in your wishlist!" });
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
                    return Json(new { success = true, message = "Added successfully!" });
                }
                catch
                {
                    return Json(new { success = false, message = "Add failed!" });
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
                return Json(new { notAuth = true, message = "You must be logged in to perform this action!" });
            }
            var check = await this._product.FindAsync(x => x.ID == id);
            if (check == null)
            {
                return Json(new { success = false, message = "Product does not exist!" });
            }
            var wishlist = await this._wishlist.FindAsync(x => x.ProductID == id && x.UserID == user.Id);
            if (wishlist == null)
            {
                return Json(new { success = false, message = $"{check.Name} does not exist in your wishlist!" });
            }
            else
            {
                try
                {
                    await this._wishlist.DeleteAsync(wishlist);
                    await this._wishlist.SaveChangesAsync();
                    return Json(new { success = true, message = "Deleted successfully!" });
                }
                catch
                {
                    return Json(new { success = false, message = "Delete failed!" });
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

            var wishlist = await this._wishlist.ListAsync(x => x.UserID == user.Id, orderBy: q => q.OrderByDescending(s => s.CreateDate));
            if (wishlist.Any())
            {
                foreach (var item in wishlist)
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
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Seller") || User.IsInRole("Admin"))
                {
                    return View("NotFound");
                }
                else if (User.IsInRole("User"))
                {
                    return View("Error404");
                }
            }

            return View("Error404");
        }


        [HttpPost]
        public async Task<IActionResult> GetBalance()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new ErroMess { msg = "You must be logged in to perform this action!" });
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
        public async Task<IActionResult> SearchProductList(string searchName = null)
        {
            // 1. Lấy danh mục (chỉ danh mục active)
            var categories = await _categoryService.ListAsync(
                filter: c => c.IsActive,
                orderBy: q => q.OrderByDescending(c => c.CreatedDate)
            );

            // 2. Lấy tất cả sản phẩm IsActive = true
            var allProducts = await _product.ListAsync(
                filter: p => p.IsActive,
                orderBy: q => q.OrderByDescending(p => p.CreatedDate)
            );
            var products = allProducts.ToList();

            // 3. Lấy danh sách product type active
            var productIds = products.Select(p => p.ID).ToList();
            var productTypes = await _productvarian.ListAsync(
                pt => productIds.Contains(pt.ProductID) && pt.IsActive
            );

            // ⚠️ Loại bỏ product không có product type active
            var validProductIds = productTypes.Select(pt => pt.ProductID).Distinct().ToHashSet();
            products = products
                .Where(p => validProductIds.Contains(p.ID))
                .ToList();

            // 4. Cập nhật lại ID sau khi lọc
            productIds = products.Select(p => p.ID).ToList();
            var categoryIds = products.Select(p => p.CategoryID).Distinct().ToList();
            var storeIds = products.Select(p => p.StoreID).Distinct().ToList();

            // 5. Truy vấn dữ liệu liên kết
            var categoriesMap = (await _categoryService.ListAsync(
                c => categoryIds.Contains(c.ID) && c.IsActive)
            ).ToDictionary(c => c.ID);

            var storeMap = (await _storeDetailService.ListAsync(
                s => storeIds.Contains(s.ID))
            ).ToDictionary(s => s.ID);

            var images = await _productimg.ListAsync(img => productIds.Contains(img.ProductID));
            var reviews = await _reviewService.ListAsync(r => productIds.Contains(r.ProductID));

            var user = await _userManager.GetUserAsync(User);
            var wishlists = user != null
                ? await _wishlist.ListAsync(w => productIds.Contains(w.ProductID) && w.UserID == user.Id)
                : new List<Wishlist>();

            // 6. Gán dữ liệu phụ vào từng sản phẩm
            foreach (var product in products)
            {
                product.Categories = categoriesMap.TryGetValue(product.CategoryID, out var cat) ? cat : null;
                product.StoreDetails = storeMap.TryGetValue(product.StoreID, out var store) ? store : null;
                product.ProductTypes = productTypes.Where(t => t.ProductID == product.ID).ToList();
                product.ProductImages = images.Where(img => img.ProductID == product.ID).ToList();
                product.Reviews = reviews.Where(r => r.ProductID == product.ID).ToList();
                product.Wishlists = wishlists.Where(w => w.ProductID == product.ID).ToList();
            }

            // 7. Gán sản phẩm vào từng danh mục
            foreach (var category in categories)
            {
                category.Products = products
                    .Where(p => p.CategoryID == category.ID)
                    .ToList();
            }

            var viewModel = new HomeViewModel
            {
                Categories = categories,
                Products = products
            };

            // 8. Xử lý tìm kiếm nếu có searchName
            if (!string.IsNullOrWhiteSpace(searchName))
            {
                var searchTerms = searchName.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var filtered = await _product.ListAsync(p =>
                    !string.IsNullOrEmpty(p.Name) &&
                    searchTerms.All(term => p.Name.ToLower().Contains(term))) ?? new List<Product>();

                if (filtered.Any())
                {
                    // Lọc lại các sản phẩm chỉ có ProductVariant IsActive == true
                    var filteredProductIds = filtered.Select(p => p.ID).ToList();

                    // Lấy các product types của filtered (đã có ở trước: productTypes)
                    var filteredProductTypes = productTypes
                        .Where(pt => filteredProductIds.Contains(pt.ProductID) && pt.IsActive) // ✅ chỉ lấy variant active
                        .ToList();

                    // Lọc các sản phẩm thật sự có variant active
                    var validFilteredProductIds = filteredProductTypes
                        .Select(pt => pt.ProductID)
                        .Distinct()
                        .ToHashSet();

                    var filteredProducts = filtered
                        .Where(p => validFilteredProductIds.Contains(p.ID))
                        .ToList();

                    // Lọc lại images, reviews, wishlists theo filteredProductIds
                    var filteredImages = images.Where(img => validFilteredProductIds.Contains(img.ProductID)).ToList();
                    var filteredReviews = reviews.Where(r => validFilteredProductIds.Contains(r.ProductID)).ToList();
                    var filteredWishlists = wishlists.Where(w => validFilteredProductIds.Contains(w.ProductID)).ToList();

                    // Gán dữ liệu phụ
                    foreach (var product in filteredProducts)
                    {
                        product.Categories = categoriesMap.TryGetValue(product.CategoryID, out var cat) ? cat : null;
                        product.StoreDetails = storeMap.TryGetValue(product.StoreID, out var store) ? store : null;
                        product.ProductTypes = filteredProductTypes.Where(t => t.ProductID == product.ID).ToList();
                        product.ProductImages = filteredImages.Where(img => img.ProductID == product.ID).ToList();
                        product.Reviews = filteredReviews.Where(r => r.ProductID == product.ID).ToList();
                        product.Wishlists = filteredWishlists.Where(w => w.ProductID == product.ID).ToList();
                    }

                    viewModel.SearchResults = filteredProducts;
                    ViewData["HasSearchResults"] = filteredProducts.Any();
                }

                else
                {
                    viewModel.SearchResults = new List<Product>();
                    ViewData["HasSearchResults"] = false;
                }
            }
            else
            {
                // Khi không có searchName, sử dụng toàn bộ products như Index1
                return RedirectToAction(nameof(Index));

            }

            ViewData["SearchKeyword"] = searchName ?? "";

            return View("Index", viewModel);
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
        public async Task<IActionResult> GetAllVouchers(Guid productId)
        {
            // Lấy biến thể sản phẩm theo ID
            var productVariant = await _productvarian.FindAsync(u => u.ID == productId);
            if (productVariant == null)
                return NotFound(new { message = "Product variant not found" });

            // Lấy sản phẩm cha của biến thể
            var product = await _product.FindAsync(p => p.ID == productVariant.ProductID);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            var storeId = product.StoreID;

            var now = DateTime.Now;

            // Lấy voucher: voucher toàn sàn (IsGlobal == true) hoặc voucher thuộc store,
            // loại bỏ voucher có Code bắt đầu bằng "Warranty" (không phân biệt hoa thường),
            // và chỉ lấy voucher active
            var vouchers = _voucher.GetAll()
     .Where(v => v.IsGlobal || (!v.IsGlobal && v.StoreID == storeId))
     .ToList()
.Where(v =>
    !v.Code.StartsWith("Warranty", StringComparison.OrdinalIgnoreCase)
    && v.IsActive
    && v.ExpirationDate > now
    && (v.MaxUsage - v.CurrentUsage) > 0
);

            var data = vouchers.Select(v => new
            {
                id = v.ID,
                code = v.Code,
                title = v.DiscountType == "Percent"
                    ? $"Discount {v.DiscountAmount}%"
                    : $"Discount ₫{v.DiscountAmount:n0}",
                minOrder = $"Min order: ₫{v.MinOrderValue:n0}",
                expire = v.ExpirationDate.ToString("MM/dd/yyyy"),
                count = v.MaxUsage - v.CurrentUsage,
                disabled = false,
                isStoreVoucher = !v.IsGlobal,
                maxDiscountAmount = v.MaxDiscountAmount.HasValue ? $"Max discount: ₫{v.MaxDiscountAmount.Value:n0}" : null
            }).ToList();

            return Json(data);
        }





        [HttpPost("voucher/calculate-discount")]
        public async Task<IActionResult> CalculateDiscount([FromBody] DiscountRequest request)
        {
            if (string.IsNullOrEmpty(request.Code) || request.OrderTotal <= 0)
                return BadRequest(new { message = "Invalid data" });

            var v = await _voucher.FindAsync(x => x.Code.ToLower() == request.Code.ToLower() && x.IsActive);
            if (v == null)
                return NotFound(new { message = "Invalid voucher" });

            if (request.OrderTotal < v.MinOrderValue)
                return BadRequest(new { message = "Order does not meet the minimum requirement" });

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
                discountType = v.DiscountType, // "Percent" or "Fixed"
                code = v.Code
            });
        }

        [HttpGet("voucher/search")]
        public async Task<IActionResult> SearchVoucher(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest(new { message = "Voucher code is required." });

            var result = await _voucher.ListAsync(
                filter: x => x.Code.ToLower() == code.ToLower()
            );

            var v = result.Select(v => new
            {
                id = v.ID,
                code = v.Code,
                title = v.DiscountType == "Percent"
                    ? $"Discount {v.DiscountAmount}%"
                    : $"Discount ₫{v.DiscountAmount:n0}",
                minOrder = $"Min order: ₫{v.MinOrderValue:n0}",
                expire = v.ExpirationDate.ToString("MM.dd.yyyy"), // Anh-Mỹ, chỉnh lại nếu muốn dd.MM.yyyy
                count = v.MaxUsage - v.CurrentUsage,
                disabled = !v.IsActive || v.ExpirationDate < DateTime.Now,
                isStoreVoucher = !v.IsGlobal,
                maxDiscountAmount = v.MaxDiscountAmount.HasValue ? $"Max discount: ₫{v.MaxDiscountAmount.Value:n0}" : null
            }).FirstOrDefault();

            if (v == null)
                return NotFound(new { message = "Voucher not found." });

            return Json(v);
        }


        [HttpGet]
        public async Task<IActionResult> UserInformation(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Missing user ID.");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Lấy StoreDetails của user
                var store = await _storeDetailService.GetStoreByUserIdAsync(id);

                // Lấy bài viết (recipe)
                var recipes = await _recipeService.ListAsync();
                var totalRecipes = recipes.Count(r => r.UserID == id);

                var products = await _product.ListAsync(
     filter: p => p.IsActive && p.StoreDetails != null && p.StoreDetails.UserID == id,
     includeProperties: p => p.Include(x => x.StoreDetails)
 ) ?? new List<Product>();

                var totalProducts = products.Count();

                // Lấy OrderDetail kèm Product -> StoreDetails + Order
                var orderDetails = await _orderDetail.ListAsync(
                    includeProperties: od => od
                        .Include(d => d.ProductTypes)
                            .ThenInclude(pt => pt.Product)
                                .ThenInclude(p => p.StoreDetails)
                        .Include(d => d.Order)
                ) ?? new List<OrderDetail>();

                // Lọc ra các đơn hàng có sản phẩm của người bán
                var sellerOrderDetails = orderDetails
                    .Where(d =>
                        d.ProductTypes?.Product?.StoreDetails?.UserID == id
                    )
                    .ToList();

                var totalSold = sellerOrderDetails.Sum(d => d.Quantity);

                // Lấy danh sách đơn hàng mà người dùng đã mua
                var orders = await _order.ListAsync() ?? new List<Order>();
                var userOrders = orders.Where(o => o.UserID == id).ToList();
                var totalOrders = userOrders.Count;

                // ViewModel
                var model = new SellerViewModel
                {
                    UserId = user.Id,
                    RegisterDate = user.JoinedDate,
                    UserName = user.UserName,
                    ProductPurchased = $"{totalOrders} Orders",
                    NumberOfProducts = $"{totalProducts} Products",
                    ProductsSold = $"{totalSold} Products",
                    ProfileImageUrl = user.ImageUrl,
                    TotalPosts = totalRecipes,
                    StoreId = store?.ID,
                    StoreName = store?.Name,// 👈 Gán StoreId
                    HasStore = store != null           // 👈 Có store hay không
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Sever error", error = ex.ToString() });
            }
        }
        [HttpGet]
        [Route("Home/GetCurrentUserRole")]
        public IActionResult GetCurrentUserRole()
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { role = "Guest" });

            string role = "User";
            if (User.IsInRole("Admin"))
                role = "Admin";
            else if (User.IsInRole("Seller"))
                role = "Seller";

            return Json(new { role });
        }
        [HttpPost]
        public async Task<IActionResult> StoreReport(StoreReportViewModel obj)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { success = false, message = "You must be logged in to report." });

                if (string.IsNullOrWhiteSpace(obj.Reason) || obj.Reason == "-- Select a reason --")
                    return Json(new { success = false, message = "Please select a reason for reporting." });

                if (string.IsNullOrWhiteSpace(obj.Message))
                    return Json(new { success = false, message = "Message is required." });

                var report = new StoreReport
                {
                    StoreID = obj.StoreID,
                    UserID = user.Id,
                    Reason = obj.Reason,
                    Message = obj.Message,
                    CreatedDate = DateTime.UtcNow,
                };

                await _storeReport.AddAsync(report);
                await _storeReport.SaveChangesAsync();

                return Json(new { success = true, message = "Report submitted successfully." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred. Please try again later." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> ManagementFollowedStores()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Home");

            var list = new List<StoreFollowerViewModel>();

            try
            {
                var followedStores = await _storeFollowersService.ListAsync(f => f.UserID == user.Id);
                foreach (var item in followedStores)
                {
                    var store = await _storeDetailService.GetAsyncById(item.StoreID);
                    if (store != null)
                    {
                        list.Add(new StoreFollowerViewModel
                        {
                            ID = store.ID, // Chuyển Guid sang string nếu cần
                            Name = store.Name,
                            Img = store.ImageUrl,
                            Address = store.Address,
                            Phone = store.Phone,
                            ShortDescriptions = store.ShortDescriptions,
                            CreatedDate = store.CreatedDate
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred. Please try again later."); // Trả về mã 500 với thông báo
            }

            return View(list);
        }
        [HttpPost]
        public async Task<IActionResult> UnfollowStore(Guid storeId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "You must be logged in to perform this action." });
                }

                var storeFollower = await _storeFollowersService.FindAsync(sf => sf.StoreID == storeId && sf.UserID == user.Id);
                if (storeFollower == null)
                {
                    return Json(new { success = false, message = "You are not following this store." });
                }

                await _storeFollowersService.DeleteAsync(storeFollower);
                await _storeFollowersService.SaveChangesAsync();

                return Json(new { success = true, message = "Successfully unfollowed the store." });
            }
            catch (Exception ex)
            {
                // Optional: log the error e.g. _logger.LogError(ex, "UnfollowStore failed");

                return Json(new
                {
                    success = false,
                    message = "An error occurred while trying to unfollow the store. Please try again later.",
                    error = ex.Message // Consider removing in production
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAvailableIngredients()
        {
            var allRecipes = await _expertRecipeServices.ListAsync();
            var ingredientFrequency = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var recipe in allRecipes)
            {
                if (!string.IsNullOrWhiteSpace(recipe.NER))
                {
                    try
                    {
                        var nerList = JsonSerializer.Deserialize<List<string>>(recipe.NER);
                        var uniquePerRecipe = new HashSet<string>();

                        foreach (var ing in nerList)
                        {
                            var cleaned = ing?.Trim().ToLower();

                            // Bỏ qua nguyên liệu không hợp lệ
                            if (string.IsNullOrWhiteSpace(cleaned)) continue;
                            if (cleaned.Length < 2) continue;
                            if (cleaned.Any(c => "!@#$%^&*+=[]{}|\\<>?/~`".Contains(c))) continue;
                            if (cleaned.StartsWith("'s") || cleaned.StartsWith(",") || cleaned.All(char.IsSymbol)) continue;

                            // Tránh đếm lặp nguyên liệu trong cùng 1 món
                            if (uniquePerRecipe.Add(cleaned))
                            {
                                if (ingredientFrequency.ContainsKey(cleaned))
                                    ingredientFrequency[cleaned]++;
                                else
                                    ingredientFrequency[cleaned] = 1;
                            }
                        }
                    }
                    catch { }
                }
            }

            // Chọn 100 nguyên liệu phổ biến nhất
            var topIngredients = ingredientFrequency
                .OrderByDescending(x => x.Value)
                .Take(100)
                .Select(x => x.Key)
                .OrderBy(x => x) // Sắp xếp ABC cho dễ hiển thị
                .ToList();

            return Json(topIngredients);
        }


        [HttpPost]
        public async Task<ActionResult> FindRecipes(List<string> ingredients, int limit = 5)
        {
            if (ingredients == null || ingredients.Count == 0)
                return BadRequest("You must enter at least 1 recipe");

            var allRecipes = await _expertRecipeServices.ListAsync();
            var results = new List<ExpertRecipe>();
            var normalizedInput = ingredients.Select(i => i.Trim().ToLower()).ToList();

            foreach (var recipe in allRecipes)
            {
                if (string.IsNullOrWhiteSpace(recipe.NER)) continue;

                try
                {
                    var nerList = JsonSerializer.Deserialize<List<string>>(recipe.NER)
                        .Select(x => x.Trim().ToLower())
                        .ToList();

                    if (normalizedInput.All(sel => nerList.Contains(sel)))
                    {
                        results.Add(recipe);
                        if (results.Count >= limit)
                            break;
                    }
                }
                catch { }
            }

            return PartialView("_RecipeResults", results);
        }


        [HttpGet]
        public async Task<IActionResult> FindRecipes()
        {
            int skip = await _expertRecipeServices.CountAsync(); // bạn đã có 10000 bản
            int limit = 100;

            var recipes = _service.LoadRecipesFromCsv(skip, limit);

            foreach (var recipe in recipes)
            {
                var entity = _service.MapToExpertRecipe(recipe);
                await _expertRecipeServices.AddAsync(entity);
            }

            await _expertRecipeServices.SaveChangesAsync();
            return View();
        }
        public async Task<IActionResult> Index()
        {
            // 1. Lấy danh mục (chỉ danh mục active)
            var categories = await _categoryService.ListAsync(
                filter: c => c.IsActive,
                orderBy: q => q.OrderByDescending(c => c.CreatedDate)
            );

            // 2. Lấy tất cả sản phẩm IsActive = true
            var allProducts = await _product.ListAsync(
                filter: p => p.IsActive,
                orderBy: q => q.OrderByDescending(p => p.CreatedDate)
            );
            var products = allProducts.ToList();

            // 3. Lấy danh sách product type active
            var productIds = products.Select(p => p.ID).ToList();
            var productTypes = await _productvarian.ListAsync(
                pt => productIds.Contains(pt.ProductID) && pt.IsActive
            );

            // ⚠️ Loại bỏ product không có product type active
            var validProductIds = productTypes.Select(pt => pt.ProductID).Distinct().ToHashSet();
            products = products
                .Where(p => validProductIds.Contains(p.ID))
                .ToList();

            // 4. Cập nhật lại ID sau khi lọc
            productIds = products.Select(p => p.ID).ToList();
            var categoryIds = products.Select(p => p.CategoryID).Distinct().ToList();
            var storeIds = products.Select(p => p.StoreID).Distinct().ToList();

            // 5. Truy vấn dữ liệu liên kết
            var categoriesMap = (await _categoryService.ListAsync(
                c => categoryIds.Contains(c.ID) && c.IsActive)
            ).ToDictionary(c => c.ID);

            var storeMap = (await _storeDetailService.ListAsync(
                s => storeIds.Contains(s.ID))
            ).ToDictionary(s => s.ID);

            var images = await _productimg.ListAsync(img => productIds.Contains(img.ProductID));
            var reviews = await _reviewService.ListAsync(r => productIds.Contains(r.ProductID));

            var user = await _userManager.GetUserAsync(User);
            var wishlists = user != null
                ? await _wishlist.ListAsync(w => productIds.Contains(w.ProductID) && w.UserID == user.Id)
                : new List<Wishlist>();

            // 6. Gán dữ liệu phụ vào từng sản phẩm
            foreach (var product in products)
            {
                product.Categories = categoriesMap.TryGetValue(product.CategoryID, out var cat) ? cat : null;
                product.StoreDetails = storeMap.TryGetValue(product.StoreID, out var store) ? store : null;
                product.ProductTypes = productTypes.Where(t => t.ProductID == product.ID).ToList();
                product.ProductImages = images.Where(img => img.ProductID == product.ID).ToList();
                product.Reviews = reviews.Where(r => r.ProductID == product.ID).ToList();
                product.Wishlists = wishlists.Where(w => w.ProductID == product.ID).ToList();
            }

            // 7. Gán sản phẩm vào từng danh mục (nếu danh mục tồn tại và active)
            foreach (var category in categories)
            {
                category.Products = products
                    .Where(p => p.CategoryID == category.ID)
                    .ToList();
            }

            // 8. Gửi về view
            var viewModel = new HomeViewModel
            {
                Categories = categories,
                Products = products,
                SearchResults = products
            };
            ViewData["HasSearchResults"] = products.Any(); // Gán để hỗ trợ điều kiện hiển thị trong view

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> FilterProducts(decimal? minPrice, decimal? maxPrice)
        {
            IEnumerable<Product> products;

            // Create filter expression based on price range
            // Assuming price is stored in ProductTypes
            Expression<Func<Product, bool>> priceFilter = null;

            if (minPrice.HasValue && maxPrice.HasValue)
            {
                priceFilter = p => p.ProductTypes.Any(pt => pt.SellPrice >= minPrice.Value && pt.SellPrice <= maxPrice.Value);
            }
            else if (minPrice.HasValue)
            {
                priceFilter = p => p.ProductTypes.Any(pt => pt.SellPrice >= minPrice.Value);
            }
            else if (maxPrice.HasValue)
            {
                priceFilter = p => p.ProductTypes.Any(pt => pt.SellPrice <= maxPrice.Value);
            }

            // Get products with the same include properties as FilterByCategory
            products = await _product.ListAsync(
                filter: priceFilter,
                orderBy: q => q.OrderByDescending(p => p.CreatedDate),
                includeProperties: q => q
                    .Include(p => p.Categories)       // Include Categories
                    .Include(p => p.StoreDetails)     // Include StoreDetails
                    .Include(p => p.ProductTypes)     // Include ProductTypes
                    .Include(p => p.ProductImages)    // Include ProductImages
.Include(x => x.Wishlists).ThenInclude(r => r.AppUser)
        .Include(x => x.Reviews).ThenInclude(r => r.AppUser));
            return PartialView("_ProductGrid", products);
        }
        [HttpPost]
        public async Task<IActionResult> FilterByCategory(string categoryType)
        {
            // 1. Lấy tất cả sản phẩm đang hoạt động
            var allProducts = await _product.ListAsync(
                filter: p => p.IsActive,
                orderBy: q => q.OrderByDescending(p => p.CreatedDate)
            );

            List<Product> filteredProducts;

            // 2. Lọc theo loại danh mục (theo Guid hoặc theo tên)
            if (categoryType.ToLower() == "all")
            {
                filteredProducts = allProducts.ToList();
            }
            else
            {
                Guid categoryId;
                if (Guid.TryParse(categoryType, out categoryId))
                {
                    filteredProducts = allProducts
                        .Where(p => p.CategoryID == categoryId)
                        .ToList();
                }
                else
                {
                    var category = (await _categoryService.ListAsync(
                        c => c.Name.ToLower().Contains(categoryType.ToLower()) && c.IsActive
                    )).FirstOrDefault();

                    filteredProducts = category != null
                        ? allProducts.Where(p => p.CategoryID == category.ID).ToList()
                        : new List<Product>();
                }
            }

            // 3. Lấy các ID liên quan
            var productIds = filteredProducts.Select(p => p.ID).ToList();
            var categoryIds = filteredProducts.Select(p => p.CategoryID).Distinct().ToList();
            var storeIds = filteredProducts.Select(p => p.StoreID).Distinct().ToList();

            // 4. Truy vấn dữ liệu phụ
            var variants = await _productvarian.ListAsync(v => productIds.Contains(v.ProductID) && v.IsActive);
            var validProductIds = variants.Select(v => v.ProductID).Distinct().ToHashSet();

            // ⚠️ Loại bỏ product không có variant hoặc tất cả variant không active
            filteredProducts = filteredProducts
                .Where(p => validProductIds.Contains(p.ID))
                .ToList();

            productIds = filteredProducts.Select(p => p.ID).ToList(); // update lại productIds sau khi lọc
            categoryIds = filteredProducts.Select(p => p.CategoryID).Distinct().ToList();
            storeIds = filteredProducts.Select(p => p.StoreID).Distinct().ToList();

            var images = await _productimg.ListAsync(i => productIds.Contains(i.ProductID));
            var reviews = await _reviewService.ListAsync(r => productIds.Contains(r.ProductID));
            var categories = await _categoryService.ListAsync(c => categoryIds.Contains(c.ID) && c.IsActive);
            var stores = await _storeDetailService.ListAsync(s => storeIds.Contains(s.ID));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishlists = !string.IsNullOrEmpty(userId)
                ? await _wishlist.ListAsync(w => productIds.Contains(w.ProductID) && w.UserID == userId)
                : new List<Wishlist>();

            // 5. Gán dữ liệu phụ vào từng sản phẩm
            foreach (var product in filteredProducts)
            {
                product.ProductImages = images.Where(i => i.ProductID == product.ID).ToList();
                product.ProductTypes = variants.Where(v => v.ProductID == product.ID).ToList();
                product.Reviews = reviews.Where(r => r.ProductID == product.ID).ToList();
                product.Wishlists = wishlists.Where(w => w.ProductID == product.ID).ToList();
                product.Categories = categories.FirstOrDefault(c => c.ID == product.CategoryID);
                product.StoreDetails = stores.FirstOrDefault(s => s.ID == product.StoreID);
            }

            return PartialView("_ProductGrid", filteredProducts);
        }



        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.ListAsync(
                filter: null,
                orderBy: q => q.OrderByDescending(p => p.CreatedDate),
                includeProperties: q => q.Include(p => p.Recipes)
            );

            // Perform the count query asynchronously for each category using _product
            var result = await Task.WhenAll(categories.Select(async c => new
            {
                id = c.ID,
                name = c.Name,
                count = await _product.GetAll().Where(p => p.CategoryID == c.ID).CountAsync() // Count using _product
            }));

            return Json(result);
        }

        public async Task<IActionResult> ProductDetail(Guid id)
        {
            var productDetail = await _product.FindAsync(x => x.ID == id && x.IsActive);
            if (productDetail == null)
                return RedirectToAction("NotFoundPage");

            var store = await _storeDetailService.FindAsync(s => s.ID == productDetail.StoreID && s.IsActive);
            if (store == null)
                return RedirectToAction("NotFoundPage");
            store.AppUser = await _userManager.FindByIdAsync(store.UserID);

            var productCategory = await _categoryService.FindAsync(c => c.ID == productDetail.CategoryID);

            var productImages = await _productimg.ListAsync(p => p.ProductID == id);
            var productTypes = await _productvarian.ListAsync(p => p.ProductID == id);

            var reviews = (await _reviewService.ListAsync(r => r.ProductID == id)).ToList();
            foreach (var r in reviews)
            {
                r.AppUser = await _userManager.FindByIdAsync(r.UserID);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishlists = !string.IsNullOrEmpty(userId)
                ? await _wishlist.ListAsync(w => w.ProductID == id && w.UserID == userId)
                : new List<Wishlist>();

            int totalSold = 0;
            var orderDetails = await _orderDetail.ListAsync(x => x.ProductTypes.ProductID == id);
            if (orderDetails != null && orderDetails.Any())
            {
                totalSold = orderDetails.Sum(x => x.Quantity);
            }

            var categories = await _categoryService.ListAsync(
                orderBy: q => q.OrderByDescending(c => c.CreatedDate));

            var allProducts = await _product.ListAsync(
                filter: p => p.IsActive,
                orderBy: q => q.OrderByDescending(p => p.CreatedDate)
            );

            // Gán danh sách sản phẩm vào từng danh mục
            foreach (var category in categories)
            {
                category.Products = allProducts
                    .Where(p => p.CategoryID == category.ID)
                    .ToList();
            }

            // Bước 1: Lọc danh sách sản phẩm cùng danh mục (IsActive = true và cùng Category)
            var sameCategoryProducts = allProducts
                .Where(p => p.IsActive && p.CategoryID == productDetail.CategoryID && p.ID != productDetail.ID)
                .ToList();

            // Bước 2: Lấy các ProductType đang hoạt động của các sản phẩm đó
            var sameProductIds = sameCategoryProducts.Select(p => p.ID).ToList();
            var activeVariants = await _productvarian.ListAsync(v => sameProductIds.Contains(v.ProductID) && v.IsActive);

            // Bước 3: Loại bỏ các sản phẩm KHÔNG có bất kỳ variant nào đang hoạt động
            var validProductIds = activeVariants.Select(v => v.ProductID).Distinct().ToHashSet();
            sameCategoryProducts = sameCategoryProducts
                .Where(p => validProductIds.Contains(p.ID))
                .ToList();

            // Bước 4: Chuẩn bị ID để truy vấn các dữ liệu phụ
            sameProductIds = sameCategoryProducts.Select(p => p.ID).ToList();
            var sameStoreIds = sameCategoryProducts.Select(p => p.StoreID).Distinct().ToList();
            var sameCategoryIds = sameCategoryProducts.Select(p => p.CategoryID).Distinct().ToList();

            // Bước 5: Lấy dữ liệu phụ
            var sameImages = await _productimg.ListAsync(i => sameProductIds.Contains(i.ProductID));
            var sameReviews = await _reviewService.ListAsync(r => sameProductIds.Contains(r.ProductID));
            var sameWishlists = !string.IsNullOrEmpty(userId)
                ? await _wishlist.ListAsync(w => sameProductIds.Contains(w.ProductID) && w.UserID == userId)
                : new List<Wishlist>();
            var sameStores = await _storeDetailService.ListAsync(s => sameStoreIds.Contains(s.ID));
            var sameCategories = await _categoryService.ListAsync(c => sameCategoryIds.Contains(c.ID) && c.IsActive);

            // Bước 6: Gán dữ liệu vào từng sản phẩm
            foreach (var product in sameCategoryProducts)
            {
                product.ProductImages = sameImages.Where(i => i.ProductID == product.ID).ToList();
                product.ProductTypes = activeVariants.Where(v => v.ProductID == product.ID).ToList();
                product.Reviews = sameReviews.Where(r => r.ProductID == product.ID).ToList();
                product.Wishlists = sameWishlists.Where(w => w.ProductID == product.ID).ToList();
                product.StoreDetails = sameStores.FirstOrDefault(s => s.ID == product.StoreID);
                product.Categories = sameCategories.FirstOrDefault(c => c.ID == product.CategoryID);
            }

            var tem = new ProductDetails
            {
                ID = productDetail.ID,
                Name = productDetail.Name,
                ShortDescription = productDetail.ShortDescription,
                LongDescription = productDetail.LongDescription,
                CreatedDate = productDetail.CreatedDate,
                ModifiedDate = productDetail.ModifiedDate,
                IsActive = productDetail.IsActive,

                categories = productCategory,
                storeDetails = store,
                ProductImages = productImages.ToList(),
                ProductVariants = productTypes.ToList(),
                Review = reviews,
                IsWishList = wishlists.Any(),
                totalsell = totalSold,
                Allcate = categories,
                ProductBycate = sameCategoryProducts // 👈 Thêm sản phẩm cùng danh mục
            };

            return View(tem);
        }
        [HttpGet]
        public async Task<IActionResult> Invoice(string id)
        {
            var tem = new InvoiceViewModels();

            if (!Guid.TryParse(id, out var invoiceID))
            {
                return RedirectToAction("NotFoundPage");
            }
            var order = await _order.FindAsync(o => o.ID == invoiceID && o.IsActive);
            if (order != null)
            {
                var getUser = await _userManager.FindByIdAsync(order.UserID);
                if (getUser != null)
                {
                    tem.orderCoce = order.OrderTracking;
                    tem.invoiceDate = order.CreatedDate;
                    tem.DueDate = order.ModifiedDate;
                    tem.NameUse = getUser.FirstName + " " + getUser.LastName;
                    tem.paymentMethod = order.PaymentMethod;
                    tem.status = order.Status;
                    tem.emailUser = getUser.Email;
                    tem.phoneUser = getUser.PhoneNumber;

                    tem.AddressUse = getUser.Address;

                    // Voucher
                    if (order.VoucherID != Guid.Empty)
                    {
                        var getVoucher = await _voucher.FindAsync(v => v.ID == order.VoucherID && v.IsActive);
                        if (getVoucher != null)
                        {
                            tem.vocherName = getVoucher.Code;

                            if (getVoucher.DiscountType == "Fixed")
                            {
                                tem.discountVocher = getVoucher.DiscountAmount;
                                tem.subtotal = Math.Max(0, order.TotalPrice - getVoucher.DiscountAmount);
                            }
                            else if (getVoucher.DiscountType == "Percent")
                            {
                                var discountAmount = (order.TotalPrice * getVoucher.DiscountAmount) / 100;
                                tem.discountVocher = discountAmount;
                                tem.subtotal = Math.Max(0, order.TotalPrice - discountAmount);
                            }
                            else
                            {
                                tem.discountVocher = 0;
                                tem.subtotal = order.TotalPrice;
                            }
                        }
                        else
                        {
                            tem.discountVocher = 0;
                            tem.subtotal = order.TotalPrice;
                        }
                    }
                    else
                    {
                        tem.discountVocher = 0;
                        tem.subtotal = order.TotalPrice;
                    }

                    // Order detail
                    var orderDetails = await _orderDetail.ListAsync(u => u.OrderID == order.ID && u.IsActive);
                    if (orderDetails != null && orderDetails.Any())
                    {
                        foreach (var item in orderDetails)
                        {
                            var getProduct = await _productvarian.FindAsync(u => u.ID == item.ProductTypesID && u.IsActive);
                            if (getProduct != null)
                            {
                                tem.itemList.Add(new ItemInvoice
                                {
                                    amount = item.Quantity * item.ProductPrice,
                                    nameItem = string.IsNullOrWhiteSpace(item.ProductTypeName)
                                        ? getProduct.Name
                                        : item.ProductTypeName,
                                    quantity = item.Quantity,
                                    unitPrice = item.ProductPrice
                                });
                            }
                        }
                    }
                    var a = orderDetails;
                    return View(tem);
                }
            }

            var balance = await _balance.FindAsync(b => b.ID == invoiceID && b.Display);
            if (balance != null)
            {
                var getUser = await _userManager.FindByIdAsync(balance.UserID);
                if (getUser != null && (balance.Method == "Withdraw" || balance.Method == "Deposit"))
                {
                    tem.orderCoce = id;
                    tem.invoiceDate = balance.StartTime;
                    tem.DueDate = balance.DueTime;
                    tem.NameUse = getUser.FirstName + " " + getUser.LastName;
                    tem.paymentMethod = balance.Method;
                    tem.status = balance.Status;
                    tem.emailUser = getUser.Email;
                    tem.phoneUser = getUser.PhoneNumber;
                    tem.subtotal = Math.Abs(balance.MoneyChange);
                    tem.AddressUse = getUser.Address;

                    tem.itemList.Add(new ItemInvoice
                    {
                        amount = Math.Abs(balance.MoneyChange),
                        nameItem = balance.Method == "Withdraw"
                            ? $"Withdraw to {getUser.UserName}"
                            : $"Deposit to {getUser.UserName}",
                        quantity = 1,
                        unitPrice = Math.Abs(balance.MoneyChange)
                    });

                    return View(tem);
                }
            }

            return RedirectToAction("NotFoundPage");
        }
        [HttpGet]
        public async Task<IActionResult> LoadRecipeViewHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var history = await _recipeViewHistoryServices.ListAsync(
                filter: h => h.UserID == user.Id,
                orderBy: q => q.OrderByDescending(h => h.ViewedAt),
                includeProperties: q => q.Include(x => x.ExpertRecipe)
            );

            var topHistory = history.Take(50); // Lấy tối đa 50 lịch sử gần nhất

            return PartialView("_RecipeHistoryPartial", topHistory);
        }

    }
    public class HomeViewModel
    {
        public IEnumerable<Categories> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }

        public IEnumerable<Product> SearchResults { get; set; }

    }

    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<ProductImage> ProductImages { get; set; }
        public IEnumerable<ProductTypes> ProductVariants { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
    }

    public class CategoryProductsViewModel
    {
        public Categories Category { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }

    public class StoreDetailViewModel
    {
        public StoreDetails Store { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}


