using System.Net.Http.Headers;
using AutoMapper;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        //Review Details
        public async Task<IActionResult> ReplyFeedback(string id)
        {
            try
            {
                // Kiểm tra id có hợp lệ không
                if (!Guid.TryParse(id, out Guid reviewId))
                {
                    return Json(new { success = false, message = "Invalid ID." });
                }

                // Tìm review theo ReviewId
                var review = await _reviewService.FindAsync(r => r.ID == reviewId);

                if (review == null)
                {
                    return Json(new { success = false, message = "No reviews found." });
                }

                // Lấy thông tin người dùng
                var user = await _userManager.FindByIdAsync(review.UserID);
                if (user == null)
                {
                    return Json(new { success = false, message = "User does not exist." });
                }

                // Lấy thông tin sản phẩm
                var product = await _productService.GetAsyncById(review.ProductID);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product does not exist." });
                }

                // Tạo ViewModel để hiển thị trong View
                var reviewModel = new ReivewViewModel
                {
                    ID = review.ID,
                    Username = user.UserName,
                    ProductName = product.Name,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CommentDate = review.CommentDate,
                    Reply = review.Reply,
                    Status = review.Status,
                    UserID = review.UserID,
                    ProductID = review.ProductID
                };

                return View(reviewModel);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để debug sau này
                Console.WriteLine($"Error: {ex.Message}");

                // Trả về lỗi JSON để tránh chết chương trình
                return Json(new { success = false, message = "An error occurred, please try again later." });
            }
        }

        //update reply
        [HttpPost]
        public async Task<IActionResult> ReplyFeedback(ReivewViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Reply))
            {
                return Json(new { success = false, message = "Invalid data!" });
            }

            try
            {
                var review = await _reviewService.GetAsyncById(model.ID);

                if (review == null)
                {
                    return Json(new { success = false, message = "No reviews found!" });
                }

                // Cập nhật phản hồi
                review.Reply = model.Reply;
                review.ReplyDate = DateTime.Now;

                // Lưu thay đổi
                await _reviewService.UpdateAsync(review);
                await _reviewService.SaveChangesAsync();

                return Redirect("/Seller/FeedbackList");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Show(string id)
        {
            if (!Guid.TryParse(id, out Guid reviewId))
            {
                return BadRequest(new ErroMess { success = false, msg = "Invalid ID." });
            }

            try
            {
                var review = await _reviewService.GetAsyncById(reviewId);

                if (review == null)
                {
                    return Json(new ErroMess { success = false, msg = "No reviews found!" });
                }

                // Cập nhật trạng thái từ ẩn sang hiện
                review.Status = false; // từ ẩn -> hiện

                await _reviewService.UpdateAsync(review);
                await _reviewService.SaveChangesAsync();

                return Json(new ErroMess { success = true, msg = "Feedback updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErroMess { success = false, msg = "System error: " + ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Hidden(string id)
        {
            if (!Guid.TryParse(id, out Guid reviewId))
            {
                return BadRequest(new ErroMess { success = false, msg = "Invalid ID." });
            }

            try
            {
                var review = await _reviewService.GetAsyncById(reviewId);

                if (review == null)
                {
                    return Json(new ErroMess { success = false, msg = "No reviews found!" });
                }

                // Cập nhật trạng thái từ ẩn sang hiện
                review.Status = true; // từ hiện -> ẩn

                await _reviewService.UpdateAsync(review);
                await _reviewService.SaveChangesAsync();

                return Json(new ErroMess { success = true, msg = "Feedback updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErroMess { success = false, msg = "System error: " + ex.Message });
            }
        }
        public IActionResult CreateStore()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStore(StoreViewModel model, IFormFile? ImgFile)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                bool isSeller = await _storeRepository.IsUserSellerAsync(user.Id);
                if (!isSeller)
                {
                    ModelState.AddModelError("", "You do not have permission to create a store.");
                    return View(model);
                }

                var storeEntity = _mapper.Map<StoreDetails>(model);
                storeEntity.UserID = user.Id;
                storeEntity.Status = "Pending";
                storeEntity.IsActive = true;
                storeEntity.CreatedDate = DateTime.Now;
                storeEntity.ModifiedDate = null;

                storeEntity.LongDescriptions = model.LongDescriptions?.Trim();
                storeEntity.ShortDescriptions = model.ShortDescriptions?.Trim();
                storeEntity.Address = model.Address?.Trim();
                storeEntity.Phone = model.Phone?.Trim();

                if (ImgFile != null && ImgFile.Length > 0)
                {
                    string[] allowedExtensions = { ".png", ".jpeg", ".jpg" };
                    string extension = Path.GetExtension(ImgFile.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("Img", "Only image files (.png, .jpeg, .jpg) are supported.");
                        return View(model);
                    }

                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + extension;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImgFile.CopyToAsync(fileStream);
                    }

                    storeEntity.ImageUrl = "/uploads/" + uniqueFileName;
                }

                await _storeDetailService.AddStoreAsync(storeEntity, user.Id);

                // 🟢 Dùng Session thay vì TempData
                HttpContext.Session.SetString("SuccessMessage", "Store registration successful! Please wait for admin approval.");

                return RedirectToAction("ViewStore"); // Điều hướng sau khi tạo thành công
            }
            return View(model);
        }
        public async Task<IActionResult> ViewProductVariants(Guid productId)
        {
            var variants = await _variantService.GetVariantsByProductIdAsync(productId);
            if (variants.Any())
            {
                ViewBag.StoreId = variants.First().StoreID; // Lấy StoreID từ danh sách variant
            }
            ViewBag.ProductId = productId; // Lưu ProductId để sử dụng trong View

            var isStoreActive = await _productService.IsStoreActiveByProductIdAsync(productId);
            ViewBag.IsStoreActive = isStoreActive;

            return View(variants);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProductVariant(Guid productId)
        {
            var model = new ProductVariantCreateViewModel
            {
                ProductID = productId
            };
            ViewBag.ProductID = productId;

            var isActive = await _variantService.IsStoreActiveByProductIdAsync(productId);
            ViewBag.IsActive = isActive ?? false;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductVariant(ProductVariantCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _variantService.CreateProductVariantAsync(model);

            return RedirectToAction("ViewProductVariants", new { productId = model.ProductID });
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProductVariant(Guid variantId)
        {
            var model = await _variantService.GetProductVariantForEditAsync(variantId);
            if (model == null)
            {
                return NotFound();
            }

            // Kiểm tra trạng thái của Store dựa trên variantId
            var isStoreActive = await _variantService.IsStoreActiveByVariantIdAsync(variantId);
            ViewBag.IsStoreActive = isStoreActive;

            ViewBag.ProductID = model.ProductID;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProductVariant(ProductVariantEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var success = await _variantService.UpdateProductVariantAsync(model);
                if (success)
                {
                    return RedirectToAction("ViewProductVariants", "Seller", new { productId = model.ProductID });
                }
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateProductVariantStatus(Guid variantId, bool isActive)
        {
            var result = _variantService.UpdateProductVariantStatus(variantId, isActive);
            return Json(new { success = result });
        }
    }
}
