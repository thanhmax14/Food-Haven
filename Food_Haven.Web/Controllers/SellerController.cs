using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
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
using Microsoft.EntityFrameworkCore;
using BusinessLogic.Services.VoucherServices;
using BusinessLogic.Services.ProductImages;
using Food_Haven.Web.Services;
using MailKit.Search;
using BusinessLogic.Services.ComplaintImages;
using BusinessLogic.Services.Complaints;
using Repository.BalanceChange;
using BusinessLogic.Hash;

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
        private readonly IStoreDetailService _storedetail;
        private readonly IProductService _product;
        private readonly IVoucherServices _voucher;
        private readonly IProductImageService _productImageService;
        private readonly IComplaintImageServices _complaintImageServices;
        private readonly IComplaintServices _complaintService;
        private readonly ManageTransaction _manageTransaction;


        public SellerController(IReviewService reviewService, UserManager<AppUser> userManager, IProductService productService, IStoreDetailService storeDetailService, StoreDetailsRepository storeRepository, IMapper mapper, IWebHostEnvironment webHostEnvironment, IProductVariantService variantService, IOrdersServices order, IBalanceChangeService balance, IOrderDetailService orderDetail, IStoreDetailService storedetail, IProductService product, IVoucherServices voucher, IProductImageService productImageService, IComplaintImageServices complaintImageServices, IComplaintServices complaintService, ManageTransaction managetrans)
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
            _storedetail = storedetail;
            _product = product;
            _voucher = voucher;
            _productImageService = productImageService;
            _complaintImageServices = complaintImageServices;
            _complaintService = complaintService;
            _manageTransaction = managetrans;
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

            var existingStore = await _storeDetailService.GetStoreByIdAsync(id);
            if (existingStore == null)
            {
                ModelState.AddModelError("", "\r\n23 / 5.000\r\nStore not found");
                return View(model);
            }

            string imgPath = existingStore.ImageUrl;

            if (ImgFile != null && ImgFile.Length > 0)
            {
                string[] allowedExtensions = { ".png", ".jpeg", ".jpg" };
                string extension = Path.GetExtension(ImgFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Img", "Only image files (.png, .jpeg, .jpg) are supported");
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

                imgPath = "/uploads/" + uniqueFileName;
            }

            var success = await _storeDetailService.UpdateStoreAsync(id, model.Name, model.LongDescriptions,
                                                            model.ShortDescriptions, model.Address,
                                                            model.Phone, imgPath);

            if (!success)
            {
                ModelState.AddModelError("", "Store update failed!");
                return View(model);
            }

            // Gán lại giá trị ảnh sau cập nhật (nếu có ảnh mới)
            model.Img = imgPath;
            model.ModifiedDate = DateTime.Now;

            ViewBag.Success = "Store update successful!";
            return View(model);
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

        [HttpPost]
        public IActionResult ClearSuccessMessage()
        {
            HttpContext.Session.Remove("SuccessMessage");
            return Ok();
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

            var categories = await _productService.GetActiveCategoriesAsync();
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                var categories = await _productService.GetActiveCategoriesAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Name
                }).ToList();

                return View(model);
            }

            var productImages = new List<ProductImageViewModel>();
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            // Handle Main Image
            if (model.MainImage != null && model.MainImage.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.MainImage.FileName)}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.MainImage.CopyToAsync(stream);
                }

                productImages.Add(new ProductImageViewModel
                {
                    ImageUrl = $"/uploads/{fileName}",
                    IsMain = true,
                    FileName = fileName,
                    ContentType = model.MainImage.ContentType
                });
            }

            // Handle Gallery Images
            if (model.GalleryImages != null && model.GalleryImages.Any())
            {
                foreach (var img in model.GalleryImages)
                {
                    if (img != null && img.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(img.FileName)}";
                        var filePath = Path.Combine(uploadFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await img.CopyToAsync(stream);
                        }

                        productImages.Add(new ProductImageViewModel
                        {
                            ImageUrl = $"/uploads/{fileName}",
                            IsMain = false,
                            FileName = fileName,
                            ContentType = img.ContentType
                        });
                    }
                }
            }

            var isSuccess = await _productService.CreateProductAsync(model, userId, productImages);

            if (isSuccess)
            {
                TempData["ProductCreated"] = true;
                TempData["StoreID"] = model.StoreID;
                return RedirectToAction("CreateProduct");
            }

            // Nếu lỗi, nạp lại danh mục
            var categoriesAfter = await _productService.GetActiveCategoriesAsync();
            model.Categories = categoriesAfter.Select(c => new SelectListItem
            {
                Value = c.ID.ToString(),
                Text = c.Name
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProduct(Guid productId)
        {
            var model = await _productService.GetProductByIdAsync(productId);
            if (model == null)
            {
                return NotFound();
            }

            // ✅ Gọi từ Service thay vì dùng _context
            model.ExistingImages = await _productService.GetImageUrlsByProductIdAsync(productId);

            var isStoreActive = await _productService.IsStoreActiveByProductIdAsync(productId);
            ViewBag.IsStoreActive = isStoreActive;

            var categories = await _productService.GetActiveCategoriesAsync();
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
        public async Task<IActionResult> UpdateProduct(ProductUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ExistingImages = await _productService.GetImageUrlsByProductIdAsync(model.ProductID);
                model.ExistingMainImage = model.ExistingImages.FirstOrDefault();

                var categories = await _productService.GetActiveCategoriesAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Name
                }).ToList();

                ViewBag.StoreID = model.StoreID;
                return View(model);
            }

            var webRootPath = _webHostEnvironment.WebRootPath;
            await _productService.UpdateProductAsync(model, webRootPath);

            model.ExistingImages = await _productService.GetImageUrlsByProductIdAsync(model.ProductID);
            model.ExistingMainImage = model.ExistingImages.FirstOrDefault();

            model.Categories = (await _productService.GetActiveCategoriesAsync()).Select(c => new SelectListItem
            {
                Value = c.ID.ToString(),
                Text = c.Name
            }).ToList();

            ViewBag.UpdateSuccess = true;
            ViewBag.StoreID = model.StoreID;

            return View(model);
        }

        [HttpPost]
        [Route("Seller/ToggleStatus")]
        public async Task<IActionResult> ToggleStatus(Guid productId, bool isActive)
        {
            var success = await _productService.ToggleProductStatus(productId);
            if (!success) return NotFound(new { message = "Product not found" });

            return Ok(new { success = true, message = "Product status updated successfully!" });
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
                    ModelState.AddModelError("", "Bạn không có quyền tạo cửa hàng.");
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
                        ModelState.AddModelError("Img", "Chỉ hỗ trợ file ảnh (.png, .jpeg, .jpg)");
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
                HttpContext.Session.SetString("SuccessMessage", "Đăng ký cửa hàng thành công! Vui lòng chờ quản trị viên duyệt.");

                return RedirectToAction("ViewStore"); // Điều hướng sau khi tạo thành công
            }
            return View(model);
        }

        public async Task<IActionResult> ViewProductType(Guid productId)
        {
            var productType = await _variantService.GetProductTypeByProductIdAsync(productId);
            if (productType.Any())
            {
                ViewBag.StoreId = productType.First().StoreID; // Lấy StoreID từ danh sách variant
            }
            ViewBag.ProductId = productId; // Lưu ProductId để sử dụng trong View

            var isStoreActive = await _productService.IsStoreActiveByProductIdAsync(productId);
            ViewBag.IsStoreActive = isStoreActive;

            return View(productType);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProductType(Guid productId)
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProductType(ProductVariantCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _variantService.CreateProductVariantAsync(model);

            ViewBag.ProductTypeCreated = true;
            ViewBag.ProductID = model.ProductID;

            return View(model); // Giữ lại trang để hiển thị SweetAlert rồi redirect bằng JS
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProductType(Guid variantId)
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
        public async Task<IActionResult> UpdateProductType(ProductVariantEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var success = await _variantService.UpdateProductVariantAsync(model);
                if (success)
                {
                    return RedirectToAction("ViewProductType", "Seller", new { productId = model.ProductID });
                }
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateProductTypeStatus(Guid variantId, bool isActive)
        {
            var result = _variantService.UpdateProductVariantStatus(variantId, isActive);
            return Json(new { success = result });
        }

        public async Task<IActionResult> ManageOrder()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetOrder()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new ErroMess { msg = "Bạn chưa đăng nhập!!" });

            var getStore = await _storedetail.FindAsync(u => u.UserID == user.Id);
            if (getStore == null)
                return Json("Store not found");
            var products = await _product.ListAsync(p => p.StoreID == getStore.ID);
            if (!products.Any())
                return Json(new List<GetSellerOrder>());
            var productIds = products.Select(p => p.ID).ToList();
            var productTypes = await _variantService.ListAsync(pt => productIds.Contains(pt.ProductID));
            if (!productTypes.Any())
                return Json(new List<GetSellerOrder>());
            var productTypeIds = productTypes.Select(pt => pt.ID).ToList();
            var orderDetails = await _orderDetail.ListAsync(od => productTypeIds.Contains(od.ProductTypesID));
            if (!orderDetails.Any())
                return Json(new List<GetSellerOrder>());
            var orderIds = orderDetails.Select(od => od.OrderID).Distinct().ToList();
            var orders = await _order.ListAsync(o => orderIds.Contains(o.ID), orderBy: q => q.OrderByDescending(x => x.CreatedDate));
            if (!orders.Any())
                return Json(new List<GetSellerOrder>());
            var userIds = orders.Select(o => o.UserID).Distinct().ToList();
            var users = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName);
            var result = orders.Select((order, index) => new GetSellerOrder
            {
                STT = index + 1,
                OrderTracking = order.OrderTracking,
                UserName = users.ContainsKey(order.UserID) ? users[order.UserID] : "Unknown",
                OrderDate = order.CreatedDate,
                Quantity = orderDetails.Where(od => od.OrderID == order.ID).Sum(od => od.Quantity),
                Total = order.TotalPrice,
                Status = order.Status.ToUpper(),
                StatusPayment = order.PaymentStatus,
                Note = order.Note,
                DeliveryAddress = order.DeliveryAddress,
                Desctiption = order.Description
            }).ToList();

            return Json(result);
        }
        public async Task<IActionResult> ViewOrderDetails(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Home");

            if (string.IsNullOrEmpty(id))
                return NotFound("Invalid Order ID");

            var order = await _order.FindAsync(u => u.OrderTracking == id);
            if (order == null)
                return NotFound("Order not found");

            var orderDetails = await _orderDetail.ListAsync(od => od.OrderID == order.ID);
            if (!orderDetails.Any())
                return NotFound("Order has no details");

            var productTypeIds = orderDetails.Select(od => od.ProductTypesID).ToList();
            var productTypes = await _variantService.ListAsync(pt => productTypeIds.Contains(pt.ID));
            var productIds = productTypes.Select(pt => pt.ProductID).ToList();

            var store = await _storedetail.FindAsync(s => s.UserID == user.Id);
            if (store == null)
                return NotFound("Store not found");

            var products = await _product.ListAsync(p => p.StoreID == store.ID);
            var storeProductIds = products.Select(p => p.ID).ToHashSet();

            if (!productIds.Any(pid => storeProductIds.Contains(pid)))
                return Forbid("You do not have permission to view this order.");

            var productDict = products.ToDictionary(p => p.ID, p => p.Name);
            var productTypeDict = productTypes.ToDictionary(pt => pt.ID, pt => pt.Name);
            var getInfoCustomer = await _userManager.FindByIdAsync(order.UserID);

            if (getInfoCustomer == null)
                return NotFound("Customer not found");


            var codeVoucher = "";
            decimal discountVoucher = 0m;

            if (order.VoucherID.HasValue && order.VoucherID != Guid.Empty)
            {
                var voucher = await _voucher.GetAsyncById(order.VoucherID.Value);
                if (voucher != null)
                {
                    codeVoucher = $"({voucher.Code})";

                    // Tính tổng giá trị đơn hàng từ chi tiết
                    decimal totalBeforeDiscount = orderDetails.Sum(od => od.Quantity * od.ProductPrice);

                    // Tính giá trị giảm
                    discountVoucher = voucher.DiscountType switch
                    {
                        "Percent" => (voucher.DiscountAmount / 100m) * totalBeforeDiscount,
                        "Fixed" => voucher.DiscountAmount,
                        _ => 0m
                    };


                    discountVoucher = Math.Min(discountVoucher, totalBeforeDiscount);
                }
            }


            // ✅ Dùng tuần tự thay vì song song để tránh DbContext lỗi
            var itemDetailResult = new List<ManageOrderDetailInfo>();

            foreach (var od in orderDetails)
            {
                var pt = productTypeDict.ContainsKey(od.ProductTypesID) ? productTypeDict[od.ProductTypesID] : "Unknown";
                var productType = productTypes.FirstOrDefault(p => p.ID == od.ProductTypesID);
                var pid = productType?.ProductID;
                var pName = pid != null && productDict.ContainsKey(pid.Value) ? productDict[pid.Value] : "Unknown";

                var image = await _productImageService.FindAsync(u => u.ProductID == pid && u.IsMain);
                var imageUrl = image?.ImageUrl ?? "https://nest-frontend-v6.vercel.app/assets/imgs/shop/product-1-1.jpg";

                itemDetailResult.Add(new ManageOrderDetailInfo
                {
                    Produtype = pt,
                    Product = pName,
                    NameShop = store.Name,
                    ItemPrice = od.ProductPrice,
                    Quantity = od.Quantity,
                    Totals = od.Quantity * od.ProductPrice,
                    ProductID = pid ?? Guid.Empty,
                    ImageProduct = imageUrl,
                    Status = od.Status.ToUpper()
                });
            }

            var viewModel = new manageOrderDetail
            {
                OrderTracking = order.OrderTracking,
                OrderID = order.ID,
                Note = order.Note,
                Subtotal = orderDetails.Sum(od => od.Quantity * od.ProductPrice),
                TotalOrder = order.TotalPrice,
                Discount = discountVoucher,
                NameVocher = !string.IsNullOrEmpty(codeVoucher) ? $"{codeVoucher}" : "",
                PaymentMethod = order.PaymentMethod,
                IDLogistics = order.OrderTracking,
                NameCustomer = $"{getInfoCustomer.FirstName} {getInfoCustomer.LastName}",
                EmailCustomer = getInfoCustomer.Email,
                PhoneCustomer = getInfoCustomer.PhoneNumber,
                UserNameCus = getInfoCustomer.UserName,
                ShippingAddress = order.DeliveryAddress,
                ImageCus = getInfoCustomer.ImageUrl ?? "~/assets/imgs/theme/icons/icon-user.svg",
                ItemDetail = itemDetailResult,
                StatusHistories = OrderStatusHistory.Parse(order.Description),
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(Guid id)
        {

            if (string.IsNullOrWhiteSpace(id + ""))
                return Json(new { success = false, message = "Mã đơn hàng không hợp lệ." });

            try
            {
                var order = await _order.FindAsync(o => o.ID == id);
                if (order == null)
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng." });

                if (!string.Equals(order.Status, "Pending", StringComparison.OrdinalIgnoreCase) || string.Equals(order.Status, "Preparing In Kitchen", StringComparison.OrdinalIgnoreCase))
                    return Json(new { success = false, message = "Chỉ có thể hủy đơn hàng đang chờ xử lý và Preparing In Kitchens." });

                var orderDetails = await _orderDetail.ListAsync(d => d.OrderID == order.ID);
                if (orderDetails == null || !orderDetails.Any())
                    return Json(new { success = false, message = "Đơn hàng không có sản phẩm nào." });

                foreach (var item in orderDetails)
                {
                    item.Status = "Refunded";
                    item.ModifiedDate = DateTime.UtcNow;
                    await _orderDetail.UpdateAsync(item);

                    var product = await _variantService.FindAsync(p => p.ID == item.ProductTypesID);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                        product.IsActive = true;
                        await _variantService.UpdateAsync(product);
                    }
                }


                var currentBalance = await _balance.GetBalance(order.UserID);
                var refundTransaction = new BalanceChange
                {
                    UserID = order.UserID,
                    MoneyChange = order.TotalPrice,
                    MoneyBeforeChange = currentBalance,
                    MoneyAfterChange = currentBalance + order.TotalPrice,
                    Method = "Refund",
                    Status = "Success",
                    Display = true,
                    IsComplete = true,
                    CheckDone = true,
                    StartTime = DateTime.Now,
                    DueTime = DateTime.Now
                };
                await _balance.AddAsync(refundTransaction);

                order.Status = "Cancelled by Shop";
                order.Description = string.IsNullOrEmpty(order.Description)
    ? $"CANCELLED BY SHOP-{DateTime.Now}"
    : $"{order.Description}#CANCELLED BY SHOP-{DateTime.Now}";

                order.PaymentStatus = "Refunded";
                order.ModifiedDate = DateTime.UtcNow;
                await _order.UpdateAsync(order);


                await _orderDetail.SaveChangesAsync();
                await _variantService.SaveChangesAsync();
                await _order.SaveChangesAsync();
                await _balance.SaveChangesAsync();

                return Json(new { success = true, message = "Đơn hàng đã được hủy thành công." });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Đã xảy ra lỗi khi xử lý yêu cầu. Vui lòng thử lại hoặc liên hệ admin.",
                    /*   error = ex.Message // ❗ chỉ nên show ra trong môi trường dev*/
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, string status)
        {
            try
            {
                var order = await _order.FindAsync(o => o.ID == id);
                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
                }

                // Trạng thái hủy sẽ không xử lý ở đây
                if (status == "CANCELLED BY USER" || status == "CANCELLED BY SHOP")
                {
                    return Json(new { success = false, message = "Trạng thái hủy không được xử lý tại đây." });
                }
                var orderDetails = await _orderDetail.ListAsync(d => d.OrderID == order.ID);
                if (orderDetails == null || !orderDetails.Any())
                    return Json(new { success = false, message = "Đơn hàng không có sản phẩm nào." });

                foreach (var item in orderDetails)
                {
                    item.Status = status;
                    item.ModifiedDate = DateTime.UtcNow;
                    await _orderDetail.UpdateAsync(item);

                }
                // Ghi chú thời gian thay đổi trạng thái vào Description
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string entry = $"{status}-{timestamp}";
                order.Description = string.IsNullOrWhiteSpace(order.Description)
                    ? entry
                    : $"{order.Description}#{entry}";

                // Cập nhật trạng thái và thời gian
                order.Status = status;
                order.ModifiedDate = DateTime.Now;

                await _order.UpdateAsync(order);
                await _orderDetail.SaveChangesAsync();
                await _order.SaveChangesAsync();

                return Json(new { success = true, message = $"Cập nhật trạng thái đơn hàng thành công: {status}" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Lỗi xảy ra khi cập nhật trạng thái. Vui lòng thử lại sau." });
            }
        }

        public async Task<IActionResult> Managercomplant()
        {
            return View();
        }
        public async Task<IActionResult> Detailcomplant(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Home");
            var getComplaint = await _complaintService.FindAsync(u => u.ID == id);
            if (getComplaint == null)
                return NotFound("Complant not found.");
            var getOrderDetail = await _orderDetail.FindAsync(u => u.ID == getComplaint.OrderDetailID);
            if (getOrderDetail == null)
                return NotFound("Order detail not found.");
            var getOrder = await _order.FindAsync(u => u.ID == getOrderDetail.OrderID);
            if (getOrder == null)
                return NotFound("Order not found.");

            var getStore = await _storedetail.FindAsync(u => u.UserID == user.Id);
            if (getStore == null)
                return NotFound("Store not found.");
            var getProductType = await this._variantService.FindAsync(u => u.ID == getOrderDetail.ProductTypesID);
            if (getProductType == null)
                return NotFound("Productype not found.");
            var getProduct = await _product.FindAsync(u => u.ID == getProductType.ProductID);
            if (getProduct == null)
                return NotFound("Product not found.");
            var getUser = await _userManager.FindByIdAsync(getOrder.UserID);
            if (getUser == null)
                return NotFound("User not found.");

            var model = new ComplantDetailViewmodels();
            model.Status = getComplaint.Status;
            model.CreateDate = getComplaint.CreatedDate;
            model.AdminrReply = getComplaint.AdminReply;
            model.DateAdminCreate = getComplaint.DateAdminReply;
            model.SellerReply = getComplaint.Reply;
            model.DateReply = getComplaint.ReplyDate;
            model.ComplantID = getComplaint.ID;
            model.NameShop = getStore.Name;
            model.ProductName = getProduct.Name;
            model.ProductType = getProductType.Name;
            model.Description = getComplaint.Description;
            model.UserName = getUser.UserName;
            model.OrderTracking = getOrder.OrderTracking;
            if (getComplaint.IsReportAdmin)
            {
                model.IsreportAdmin = true;
            }
            if (getComplaint.AdminReportStatus == "Pending")
            {
                model.statusAdmin = "Pending";
            }

            var getImage = await this._complaintImageServices.ListAsync(u => u.ComplaintID == getComplaint.ID);
            if (getImage.Any())
            {
                foreach (var item in getImage)
                {
                    model.image.Add(item.ImageUrl);
                }
            }
            else
            {
                model.image.Add("https://nest-frontend-v6.vercel.app/assets/imgs/shop/product-2-2.jpg");
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> GetComplaint()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new ErroMess { msg = "Bạn chưa đăng nhập!!" });
            var getStore = await _storedetail.FindAsync(u => u.UserID == user.Id);
            if (getStore == null)
                return Json("Store not found");
            var products = await _product.ListAsync(p => p.StoreID == getStore.ID);
            if (!products.Any())
                return Json(new List<GetComplaintViewModel>());

            var productIds = products.Select(p => p.ID).ToList();

            var productTypes = await _variantService.ListAsync(pt => productIds.Contains(pt.ProductID));
            if (!productTypes.Any())
                return Json(new List<GetComplaintViewModel>());

            var productTypeIds = productTypes.Select(pt => pt.ID).ToList();

            var orderDetails = await _orderDetail.ListAsync(od => productTypeIds.Contains(od.ProductTypesID));
            if (!orderDetails.Any())
                return Json(new List<GetComplaintViewModel>());

            var orderIds = orderDetails.Select(od => od.OrderID).Distinct().ToList();

            var orders = await _order.ListAsync(o => orderIds.Contains(o.ID), orderBy: q => q.OrderByDescending(x => x.CreatedDate));
            if (!orders.Any())
                return Json(new List<GetComplaintViewModel>());

            var userIds = orders.Select(o => o.UserID).Distinct().ToList();
            var orderDetailIds = orderDetails.Select(od => od.ID).ToList();

            var complaintsRaw = await this._complaintService.ListAsync(c => orderDetailIds.Contains(c.OrderDetailID));
            if (!complaintsRaw.Any())
                return Json(new List<GetComplaintViewModel>());

            var userDict = (await _userManager.Users.ToListAsync())
                .Where(u => userIds.Contains(u.Id))
                .ToDictionary(u => u.Id, u => u.UserName);

            var orderDict = orders.ToDictionary(o => o.ID, o => new { o.ID, o.UserID, o.OrderTracking });

            var orderDetailDict = orderDetails.ToDictionary(od => od.ID, od => od.OrderID);


            var complaints = complaintsRaw.Select(c =>
            {
                var orderId = orderDetailDict.ContainsKey(c.OrderDetailID) ? orderDetailDict[c.OrderDetailID] : Guid.Empty;
                var orderInfo = orderDict.ContainsKey(orderId) ? orderDict[orderId] : null;
                var userName = orderInfo != null && userDict.ContainsKey(orderInfo.UserID) ? userDict[orderInfo.UserID] : "Unknown";

                return new GetComplaintViewModel
                {
                    Id = c.ID,
                    OrderCode = orderInfo?.OrderTracking ?? "N/A", //
                    UserName = userName,
                    Description = c.Description,
                    Status = c.Status,
                    SellerReply = c.Reply,
                    AdminReply = c.AdminReply,
                    CreatedDate = c.CreatedDate,
                    ReplyDate = c.ReplyDate,
                    ReportStatus = c.AdminReportStatus,
                    AdminReplyDate = c.DateAdminReply,


                };
            }).ToList();

            return Json(complaints);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolveComplaint(Guid id, string action, string note)
        {
            string mess = "";
            if (id == Guid.Empty || string.IsNullOrWhiteSpace(action) || string.IsNullOrWhiteSpace(note))
            {
                return Json(new { success = false, message = "Thông tin gửi lên không hợp lệ." });
            }

            var complaint = await this._complaintService.FindAsync(c => c.ID == id);
            if (complaint == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khiếu nại." });
            }
            if (complaint.Status.ToLower() == "Refund".ToLower())
            {
                return Json(new { success = false, message = "Khiếu nại này đã được xử lý hoàn tiền." });
            }

            switch (action.ToLower())
            {
                case "refund":

                    complaint.Status = "Refund";
                    complaint.Reply = $"[Refund] - {note}";
                    complaint.ReplyDate = DateTime.Now;

                    try
                    {

                        // Lấy danh sách chi tiết đơn hàng
                        var orderDetails = await _orderDetail.FindAsync(d => d.ID == complaint.OrderDetailID);
                        if (orderDetails == null)
                            return Json(new { success = false, message = "Đơn hàng không có sản phẩm nào." });
                        var order = await this._order.FindAsync(u => u.ID == orderDetails.OrderID);
                        if (order == null)
                            return Json(new { success = false, message = "Không tìm thấy đơn hàng." });


                        orderDetails.Status = "Refunded";
                        orderDetails.ModifiedDate = DateTime.Now;
                        await _orderDetail.UpdateAsync(orderDetails);
                        var currentBalance = await _balance.GetBalance(order.UserID);
                        var refundTransaction = new BalanceChange
                        {
                            UserID = order.UserID,
                            MoneyChange = orderDetails.TotalPrice,
                            MoneyBeforeChange = currentBalance,
                            MoneyAfterChange = currentBalance + orderDetails.TotalPrice,
                            Method = "Refund",
                            Status = "Success",
                            Display = true,
                            IsComplete = true,
                            CheckDone = true,
                            StartTime = DateTime.Now,
                            DueTime = DateTime.Now
                        };
                        await _balance.AddAsync(refundTransaction);

                        /* // Cập nhật trạng thái đơn hàng
                         order.Status = "Refunded";
                         order.PaymentStatus = "Refunded";
                         order.ModifiedDate = DateTime.UtcNow;
                         order.Description = string.IsNullOrEmpty(order.Description)
                             ? $"Refunded - {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
                             : $"{order.Description}#Refunded - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                         await _order.UpdateAsync(order);*/

                        // Lưu thay đổi
                        await _orderDetail.SaveChangesAsync();

                        //   await _order.SaveChangesAsync();
                        await _balance.SaveChangesAsync();


                        mess = "Đơn hàng đã được hoàn tiền và hủy thành công.";
                    }
                    catch (Exception)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Đã xảy ra lỗi khi xử lý hoàn tiền. Vui lòng thử lại hoặc liên hệ quản trị viên."
                        });
                    }
                    break;

                case "warranty":

                    try
                    {
                        var result = await _manageTransaction.ExecuteInTransactionAsync(async () =>
                        {
                            var ortrack = RandomCode.GenerateUniqueCode();
                            complaint.Status = "Warranty";
                            complaint.Reply = $"[BẢO HÀNH] Order Tracking ={ortrack}\n{note}";

                            var orderDetail = await _orderDetail.FindAsync(x => x.ID == complaint.OrderDetailID);
                            if (orderDetail == null)
                                throw new Exception("Không tìm thấy chi tiết đơn hàng cần bảo hành.");

                            var product = await _variantService.FindAsync(p => p.ID == orderDetail.ProductTypesID);
                            if (product == null || !product.IsActive)
                                throw new Exception("Sản phẩm không tồn tại hoặc đã ngưng hoạt động.");

                            if (orderDetail.Quantity > product.Stock)
                                throw new Exception("Số lượng bảo hành vượt quá tồn kho.");

                            var originalOrder = await _order.FindAsync(x => x.ID == orderDetail.OrderID);
                            if (originalOrder == null)
                                throw new Exception("Không tìm thấy đơn hàng gốc.");

                            // Tạo voucher giảm 100%
                            var voucherCode = $"Warranty-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
                            var voucher = new Voucher
                            {
                                ID = Guid.NewGuid(),
                                Code = voucherCode,
                                DiscountAmount = 100,
                                DiscountType = "Percent",
                                StartDate = DateTime.Now,
                                ExpirationDate = DateTime.Now.AddDays(3),
                              //  Scope = "Warranty",
                                MaxUsage = 1,
                                CurrentUsage = 0,
                                MinOrderValue = 0,
                                IsActive = true,
                                CreatedDate = DateTime.Now
                            };
                            await _voucher.AddAsync(voucher);
                            await _voucher.SaveChangesAsync();

                            // Tạo đơn hàng miễn phí
                            var newOrderId = Guid.NewGuid();
                            var order = new Order
                            {
                                ID = newOrderId,
                                UserID = originalOrder.UserID,
                                OrderTracking = ortrack,
                                TotalPrice = 0,
                                Status = "Pending",
                                CreatedDate = DateTime.Now,
                                PaymentMethod = "wallet",
                                PaymentStatus = "Success",
                                Quantity = orderDetail.Quantity,
                                OrderCode = voucherCode,
                                VoucherID = voucher.ID,
                                DeliveryAddress = originalOrder.DeliveryAddress ?? "Bảo hành tự động",
                                Note = "Đơn bảo hành tự động",
                                Description = $"Pending-{DateTime.Now}"
                            };

                            var detail = new OrderDetail
                            {
                                ID = Guid.NewGuid(),
                                OrderID = newOrderId,
                                ProductTypesID = product.ID,
                                Quantity = orderDetail.Quantity,
                                ProductPrice = product.SellPrice,
                                TotalPrice = 0,
                                Status = "Pending"
                            };

                            // Ghi lịch sử ví
                            var balance = new BalanceChange
                            {
                                UserID = originalOrder.UserID,
                                MoneyChange = 0,
                                MoneyBeforeChange = await _balance.GetBalance(originalOrder.UserID),
                                MoneyAfterChange = await _balance.GetBalance(originalOrder.UserID),
                                Method = "Warranty",
                                Status = "Success",
                                Display = true,
                                IsComplete = true,
                                CheckDone = true,
                                StartTime = DateTime.Now,
                                DueTime = DateTime.Now,
                                //     OrderID = newOrderId
                            };

                            // Đánh dấu voucher đã dùng
                            voucher.CurrentUsage++;
                            await _voucher.UpdateAsync(voucher);
                            await _voucher.SaveChangesAsync();

                            await _order.AddAsync(order);
                            await _orderDetail.AddAsync(detail);
                            await _balance.AddAsync(balance);

                            await _order.SaveChangesAsync();
                            await _orderDetail.SaveChangesAsync();
                            await _balance.SaveChangesAsync();
                        });
                        complaint.Status = "Product Warranty";
                        complaint.Reply = $"[Product Warranty] - {note}";
                        complaint.ReplyDate = DateTime.Now;
                        if (!result)
                            return Json(new { success = false, msg = "Thao tác không thành công." });
                        mess = "Đơn bảo hành miễn phí thành công!";
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, msg = "Lỗi khi xử lý bảo hành: " + ex.Message });
                    }
                    break;


                case "dispute":
                    complaint.Status = "Report to Admin";
                    complaint.Reply = $"[TRANH CHẤP] {note}";
                    complaint.IsReportAdmin = true;
                    complaint.AdminReportStatus = "Pending";


                    break;

                default:
                    return Json(new { success = false, message = "Loại hành động không hợp lệ." });
            }

            complaint.ReplyDate = DateTime.Now;

            try
            {
                await this._complaintService.UpdateAsync(complaint);
                await this._complaintService.SaveChangesAsync();

                return Json(new { success = true, message = mess });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi lưu dữ liệu: " + ex.Message });
            }
        }
        public async Task<IActionResult> ManagerVoucher()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetAllVoucher()
        {
            try
            {
                var data = _voucher.GetAll().Select(v => new
                {
                    id = v.ID,
                    code = v.Code,
                    discountAmount = v.DiscountAmount,
                    discountType = v.DiscountType,
                    createdDate = v.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    startDate = v.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    expirationDate = v.ExpirationDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    maxUsage = v.MaxUsage,
                    currentUsage = v.CurrentUsage,
                    isActive = v.IsActive,
                    scope = v.MaxDiscountAmount,
                    minOrderValue = v.MinOrderValue
                }).ToList();

                return Json(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetVoucher(Guid id)
        {
            try
            {
                var v = await _voucher.FindAsync(x => x.ID == id);
                if (v == null) return NotFound();

                return Json(new
                {
                    id = v.ID,
                    code = v.Code,
                    discountAmount = v.DiscountAmount,
                    discountType = v.DiscountType,
                    startDate = v.StartDate.ToString("yyyy-MM-dd"),
                    expirationDate = v.ExpirationDate.ToString("yyyy-MM-dd"),
                    maxUsage = v.MaxUsage,
                    currentUsage = v.CurrentUsage,
                    isActive = v.IsActive,
                    scope = v.MaxDiscountAmount,
                    minOrderValue = v.MinOrderValue
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucher([FromBody] VoucherViewModel v)
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(v.Code))
                errors["code"] = "Code is required.";

            if (v.DiscountAmount <= 0)
                errors["discountAmount"] = "Discount amount must be greater than 0.";

            if (string.IsNullOrWhiteSpace(v.DiscountType) ||
                (v.DiscountType != "Fixed" && v.DiscountType != "Percent"))
                errors["discountType"] = "Discount type must be Fixed or Percent.";

            if (!DateTime.TryParse(v.StartDate, out var startDate))
                errors["startDate"] = "Start date is invalid or missing.";

            if (!DateTime.TryParse(v.ExpirationDate, out var expirationDate))
                errors["expirationDate"] = "Expiration date is invalid or missing.";

            if (!errors.ContainsKey("startDate") && !errors.ContainsKey("expirationDate"))
            {
                if (startDate >= expirationDate)
                    errors["startDate"] = "Start date must be before expiration date.";
            }

            if (string.IsNullOrWhiteSpace(v.Scope))
                errors["scope"] = "Scope is required.";

            if (v.MaxUsage < 0)
                errors["maxUsage"] = "Max usage must be 0 or greater.";

            if (v.CurrentUsage < 0)
                errors["currentUsage"] = "Current usage must be 0 or greater.";
            else if (v.CurrentUsage > v.MaxUsage)
                errors["currentUsage"] = "Current usage cannot exceed max usage.";

            if (v.MinOrderValue < 0)
                errors["minOrderValue"] = "Minimum order value must be 0 or greater.";

            if (errors.Any())
                return BadRequest(new { success = false, fieldErrors = errors });

            try
            {
                var entity = new Voucher
                {
                    ID = Guid.NewGuid(),
                    Code = v.Code,
                    DiscountAmount = v.DiscountAmount,
                    DiscountType = v.DiscountType,
                    StartDate = startDate,
                    ExpirationDate = expirationDate,
                    MaxDiscountAmount =decimal.Parse(v.Scope),
                    MaxUsage = v.MaxUsage,
                    CurrentUsage = v.CurrentUsage,
                    MinOrderValue = v.MinOrderValue,
                    IsActive = v.IsActive,
                    CreatedDate = DateTime.Now
                };

                await _voucher.AddAsync(entity);
                await _voucher.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = "Server error: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateVoucher([FromBody] VoucherViewModel v)
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(v.Code))
                errors["code"] = "Code is required.";

            if (v.DiscountAmount <= 0)
                errors["discountAmount"] = "Discount amount must be greater than 0.";

            if (string.IsNullOrWhiteSpace(v.DiscountType) ||
                (v.DiscountType != "Fixed" && v.DiscountType != "Percent"))
                errors["discountType"] = "Discount type must be Fixed or Percent.";

            if (!DateTime.TryParse(v.StartDate, out var startDate))
                errors["startDate"] = "Start date is invalid or missing.";

            if (!DateTime.TryParse(v.ExpirationDate, out var expirationDate))
                errors["expirationDate"] = "Expiration date is invalid or missing.";

            if (!errors.ContainsKey("startDate") && !errors.ContainsKey("expirationDate"))
            {
                if (startDate >= expirationDate)
                    errors["startDate"] = "Start date must be before expiration date.";
            }

            if (string.IsNullOrWhiteSpace(v.Scope))
                errors["scope"] = "Scope is required.";

            if (v.MaxUsage < 0)
                errors["maxUsage"] = "Max usage must be 0 or greater.";

            if (v.CurrentUsage < 0)
                errors["currentUsage"] = "Current usage must be 0 or greater.";
            else if (v.CurrentUsage > v.MaxUsage)
                errors["currentUsage"] = "Current usage cannot exceed max usage.";

            if (v.MinOrderValue < 0)
                errors["minOrderValue"] = "Minimum order value must be 0 or greater.";

            if (errors.Any())
                return BadRequest(new { success = false, fieldErrors = errors });

            try
            {
                var entity = await _voucher.FindAsync(x => x.ID == v.ID);
                if (entity == null)
                    return NotFound();

                entity.Code = v.Code;
                entity.DiscountAmount = v.DiscountAmount;
                entity.DiscountType = v.DiscountType;
                entity.StartDate = startDate;
                entity.ExpirationDate = expirationDate;
                entity.MaxDiscountAmount = decimal.Parse(v.Scope);
                entity.MaxUsage = v.MaxUsage;
                entity.CurrentUsage = v.CurrentUsage;
                entity.MinOrderValue = v.MinOrderValue;
                entity.IsActive = v.IsActive;

                await _voucher.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = "Server error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVoucher([FromBody] DeleteVoucherRequest request)
        {
            try
            {
                var v = await _voucher.FindAsync(x => x.ID == request.Id);
                if (v == null)
                    return NotFound(new { success = false, error = "Voucher not found." });

                v.IsActive = false;
                await _voucher.UpdateAsync(v);
                await _voucher.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

       

    }
}
