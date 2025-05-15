using System.Net.Http.Headers;
using System.Security.Claims;
using AutoMapper;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Repository.StoreDetails;
using Repository.ViewModels;

namespace Food_Haven.Web.Controllers
{
    public class SellerController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IProductService _productService;
        private readonly IStoreDetailService _storeDetailService;
        private HttpClient client = null;
        private string _url;

        private readonly StoreDetailsRepository _storeRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IProductVariantService _variantService;
        private readonly IOrdersServices _order;
        private readonly IBalanceChangeService _balance;
        private readonly IOrderDetailService _orderDetail;

        public SellerController(IReviewService reviewService, UserManager<AppUser> userManager, IProductService productService, IStoreDetailService storeDetailService, StoreDetailsRepository storeRepository, IMapper mapper, IWebHostEnvironment webHostEnvironment, IProductVariantService variantService, IOrdersServices order, IBalanceChangeService balance, IOrderDetailService orderDetail)
        {
            _reviewService = reviewService;
            _userManager = userManager;
            _productService = productService;
            _storeDetailService = storeDetailService;
            client = new HttpClient();
            var contentype = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentype);
            _storeRepository = storeRepository;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _variantService = variantService;
            _order = order;
            _balance = balance;
            _orderDetail = orderDetail;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> FeedbackList()
        {
            var result = new List<ReivewViewModel>();

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Home");
                }

                var storeDetail = await _storeDetailService.FindAsync(s => s.UserID == user.Id);
                if (storeDetail == null)
                {
                    return View(result);
                }

                var storeId = storeDetail.ID;

                // Lấy danh sách sản phẩm theo StoreID
                var products = await _productService.ListAsync(p => p.StoreID == storeId);
                var productIds = products.Select(p => p.ID).ToList();

                // Lấy các review thuộc những sản phẩm này
                var reviews = await _reviewService.ListAsync(r => productIds.Contains(r.ProductID));

                foreach (var review in reviews)
                {
                    var reviewer = await _userManager.FindByIdAsync(review.UserID);
                    var product = products.FirstOrDefault(p => p.ID == review.ProductID);

                    var reviewModel = new ReivewViewModel
                    {
                        ID = review.ID,
                        Comment = review.Comment,
                        CommentDate = review.CommentDate,
                        Reply = review.Reply,
                        Status = review.Status,
                        Rating = review.Rating,
                        Username = reviewer?.UserName,
                        ProductName = product?.Name,
                        StoreId = storeId
                    };

                    result.Add(reviewModel);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", msg = "There was an error loading Feedback List." });
            }

            return View(result);
        }
        public async Task<IActionResult> ViewStore()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var store = await _storeDetailService.GetStoresByUserIdAsync(userId);

            var storeData = store.FirstOrDefault();

            ViewBag.HasStore = storeData != null;
            ViewBag.StoreStatus = storeData?.Status?.ToUpper() ?? "NONE"; // NONE nếu không có store
            ViewBag.IsActive = storeData?.IsActive ?? false; // false nếu không có store

            return View(storeData);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStore(Guid id, StoreViewModel model, IFormFile? ImgFile)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Lấy thông tin cửa hàng hiện tại để giữ nguyên ảnh cũ nếu không upload ảnh mới
            var existingStore = await _storeDetailService.GetStoreByIdAsync(id);
            if (existingStore == null)
            {
                ModelState.AddModelError("", "Không tìm thấy cửa hàng.");
                return View(model);
            }

            string imgPath = existingStore.ImageUrl; // Giữ ảnh cũ nếu không có ảnh mới

            if (ImgFile != null && ImgFile.Length > 0)
            {
                string[] allowedExtensions = { ".png", ".jpeg", ".jpg" };
                string extension = Path.GetExtension(ImgFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Img", "Chỉ hỗ trợ file ảnh (.png, .jpeg, .jpg)");
                    return View(model);
                }

                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = $"{Guid.NewGuid()}{extension}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImgFile.CopyToAsync(fileStream);
                }

                imgPath = "/uploads/" + uniqueFileName; // Cập nhật đường dẫn ảnh mới
            }

            // Cập nhật cửa hàng
            var success = await _storeDetailService.UpdateStoreAsync(id, model.Name, model.LongDescriptions,
                                                                model.ShortDescriptions, model.Address,
                                                                model.Phone, imgPath);

            if (!success)
            {
                ModelState.AddModelError("", "Cập nhật cửa hàng thất bại.");
                return View(model);
            }

            return RedirectToAction("ViewStore");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdateStore(Guid id)
        {
            var store = await _storeDetailService.GetStoreByIdAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            var isActive = await _storeDetailService.IsStoreActiveAsync(id);
            if (!isActive)
            {
                ViewBag.StoreLocked = true;
                return View(); // Không truyền model vì không được chỉnh sửa
            }

            var model = new StoreViewModel
            {
                Name = store.Name,
                CreatedDate = store.CreatedDate,
                ModifiedDate = DateTime.Now,
                LongDescriptions = store.LongDescriptions,
                ShortDescriptions = store.ShortDescriptions,
                Address = store.Address,
                Phone = store.Phone,
                Img = store.ImageUrl // Giữ ảnh cũ
            };

            return View(model);
        }

        public IActionResult ViewProductList()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var products = _productService.GetProductsByCurrentUser(userId);
            var store = _storeDetailService.GetStoreByUserId(userId);

            ViewBag.StoreId = products.FirstOrDefault()?.StoreId ?? Guid.Empty; // Lấy StoreId từ danh sách sản phẩm
            ViewBag.StoreStatus = store?.IsActive ?? false;

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var isActive = await _storeDetailService.IsStoreActiveByUserIdAsync(userId);
            if (!isActive)
            {
                ViewBag.StoreIsInactive = true;
                return View(new ProductViewModel());
            }

            var categories = await _productService.GetCategoriesAsync();
            var store = await _storeDetailService.GetStoreByUserIdAsync(userId); // để lấy StoreID

            var model = new ProductViewModel
            {
                StoreID = store.ID,
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _productService.GetCategoriesAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Name
                }).ToList();

                return View(model);
            }

            await _productService.CreateProductAsync(model);

            // Gán ViewBag để hiển thị thông báo ở view
            ViewBag.ProductCreated = true;
            ViewBag.StoreID = model.StoreID;

            var categoriesAfter = await _productService.GetCategoriesAsync();
            model.Categories = categoriesAfter.Select(c => new SelectListItem
            {
                Value = c.ID.ToString(),
                Text = c.Name
            }).ToList();

            return View(model); // Giữ lại trang Create để hiện thông báo rồi chuyển trang bằng JS
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProduct(Guid productId)
        {
            var model = await _productService.GetProductByIdAsync(productId);
            if (model == null)
            {
                return NotFound();
            }

            // Kiểm tra trạng thái của Store dựa trên productId
            var isStoreActive = await _productService.IsStoreActiveByProductIdAsync(productId);
            ViewBag.IsStoreActive = isStoreActive;

            var categories = await _productService.GetCategoriesAsync();
            model.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.ID.ToString(),
                Text = c.Name
            }).ToList();

            ViewBag.ProductID = productId;
            ViewBag.StoreID = model.StoreID;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(ProductUpdateViewModel model, List<IFormFile> NewImages)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _productService.GetCategoriesAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Name
                }).ToList();

                ViewBag.StoreID = model.StoreID;
                return View(model);
            }

            string webRootPath = _webHostEnvironment.WebRootPath; // Lấy đường dẫn thư mục wwwroot

            await _productService.UpdateProductAsync(model, NewImages, webRootPath);

            // Load lại categories vì return View
            var updatedCategories = await _productService.GetCategoriesAsync();
            model.Categories = updatedCategories.Select(c => new SelectListItem
            {
                Value = c.ID.ToString(),
                Text = c.Name
            }).ToList();

            ViewBag.UpdateSuccess = true;
            ViewBag.StoreID = model.StoreID;

            return View(model);
            //return RedirectToAction("ViewProductList", "Seller", new { storeId = model.StoreID });
        }

        [HttpPost]
        [Route("Seller/ToggleStatus")]
        public async Task<IActionResult> ToggleStatus(Guid productId, bool isActive)
        {
            var success = await _productService.ToggleProductStatus(productId);
            if (!success) return NotFound(new { message = "Product not found" });

            return Ok(new { success = true, message = "Product status updated successfully!" });
        }
    }
}
