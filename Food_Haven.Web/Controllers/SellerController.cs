using System.Net.Http.Headers;
using System.Security.Claims;
using AutoMapper;
using BusinessLogic.Hash;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.ComplaintImages;
using BusinessLogic.Services.Complaints;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using BusinessLogic.Services.VoucherServices;
using Food_Haven.Web.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models;
using Repository.BalanceChange;
using Repository.ViewModels;

namespace Food_Haven.Web.Controllers
{
    [Authorize(Roles = "Seller")]
    public class SellerController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IProductService _productService;
        private readonly IStoreDetailService _storeDetailService;
        private HttpClient client = null;
        private string _url;
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
        private readonly IHubContext<ChatHub> _hubContext;



        public SellerController(IReviewService reviewService, UserManager<AppUser> userManager, IProductService productService, IStoreDetailService storeDetailService, IMapper mapper, IWebHostEnvironment webHostEnvironment, IProductVariantService variantService, IOrdersServices order, IBalanceChangeService balance, IOrderDetailService orderDetail, IStoreDetailService storedetail, IProductService product, IVoucherServices voucher, IProductImageService productImageService, IComplaintImageServices complaintImageServices, IComplaintServices complaintService, ManageTransaction managetrans, IHubContext<ChatHub> hubContext)
        {
            _reviewService = reviewService;
            _userManager = userManager;
            _productService = productService;
            _storeDetailService = storeDetailService;
            client = new HttpClient();
            var contentype = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentype);
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
            _hubContext = hubContext;
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

                // Lấy các review thuộc những sản phẩm này và sắp xếp: chưa reply trước, sau đó theo ngày comment cũ nhất
                var reviews = (await _reviewService.ListAsync(r => productIds.Contains(r.ProductID)))
                                .OrderBy(r => !string.IsNullOrEmpty(r.Reply)) // chưa reply = ưu tiên
                                .ThenBy(r => r.CommentDate)
                                .ToList();

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
            if (model == null)
            {
                return Json(new { success = false, message = "Invalid data!" });
            }
            if (!ModelState.IsValid)
            {
                return View("ReplyFeedback", model); // hoặc View(model) nếu đang trong view đúng tên
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

        // [Authorize]
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> UpdateStore(Guid id, StoreViewModel model, IFormFile? ImgFile)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return View(model);
        //     }

        //     var user = await _userManager.GetUserAsync(User);
        //     if (user == null) return Unauthorized();

        //     var existingStore = await _storeDetailService.GetStoreByIdAsync(id);
        //     if (existingStore == null)
        //     {
        //         ModelState.AddModelError("", "\r\n23 / 5.000\r\nStore not found");
        //         return View(model);
        //     }

        //     string imgPath = existingStore.ImageUrl;

        //     if (ImgFile != null && ImgFile.Length > 0)
        //     {
        //         string[] allowedExtensions = { ".png", ".jpeg", ".jpg" };
        //         string extension = Path.GetExtension(ImgFile.FileName).ToLower();

        //         if (!allowedExtensions.Contains(extension))
        //         {
        //             ModelState.AddModelError("Img", "Only image files (.png, .jpeg, .jpg) are supported");
        //             return View(model);
        //         }

        //         string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
        //         if (!Directory.Exists(uploadsFolder))
        //         {
        //             Directory.CreateDirectory(uploadsFolder);
        //         }

        //         string uniqueFileName = $"{Guid.NewGuid()}{extension}";
        //         string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //         using (var fileStream = new FileStream(filePath, FileMode.Create))
        //         {
        //             await ImgFile.CopyToAsync(fileStream);
        //         }

        //         imgPath = "/uploads/" + uniqueFileName;
        //     }

        //     var success = await _storeDetailService.UpdateStoreAsync(id, model.Name, model.LongDescriptions,
        //                                                     model.ShortDescriptions, model.Address,
        //                                                     model.Phone, imgPath);

        //     if (!success)
        //     {
        //         ModelState.AddModelError("", "Store update failed!");
        //         return View(model);
        //     }

        //     // Gán lại giá trị ảnh sau cập nhật (nếu có ảnh mới)
        //     model.Img = imgPath;
        //     model.ModifiedDate = DateTime.Now;

        //     ViewBag.Success = "Store update successful!";
        //     return View(model);
        // }

        // [Authorize]
        // [HttpGet]
        // public async Task<IActionResult> UpdateStore(Guid id)
        // {
        //     var store = await _storeDetailService.GetStoreByIdAsync(id);
        //     if (store == null)
        //     {
        //         return NotFound();
        //     }

        //     var isActive = await _storeDetailService.IsStoreActiveAsync(id);
        //     if (!isActive)
        //     {
        //         ViewBag.StoreLocked = true;
        //         return View(); // Không truyền model vì không được chỉnh sửa
        //     }

        //     var model = new StoreViewModel
        //     {
        //         Name = store.Name,
        //         CreatedDate = store.CreatedDate,
        //         ModifiedDate = DateTime.Now,
        //         LongDescriptions = store.LongDescriptions,
        //         ShortDescriptions = store.ShortDescriptions,
        //         Address = store.Address,
        //         Phone = store.Phone,
        //         Img = store.ImageUrl // Giữ ảnh cũ
        //     };

        //     return View(model);
        // }

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
            ViewBag.StoreStatusText = store?.Status ?? "Pending"; // 👈 Thêm dòng này

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
                    Text = c.Name + $" - ({c.Commission}%)"
                }).ToList()
            };
            ViewBag.StoreStatusText = store?.Status ?? "Pending"; // 👈 Thêm dòng này
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _productService.GetProductByIdAsync(productId);
            if (model == null)
            {
                return NotFound();
            }

            // ✅ Gọi từ Service thay vì dùng _context
            //model.ExistingImages = await _productService.GetImageUrlsByProductIdAsync(productId);
            model.ExistingImages = await _productService.GetGalleryImageUrlsByProductIdAsync(productId);
            model.ExistingMainImage = await _productService.GetMainImageUrlsByProductIdAsync(productId); // lấy riêng ảnh chính


            var isStoreActive = await _productService.IsStoreActiveByProductIdAsync(productId);
            ViewBag.IsStoreActive = isStoreActive;

            var categories = await _productService.GetActiveCategoriesAsync();
            model.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.ID.ToString(),
                Text = c.Name + $" - ({c.Commission}%)"
            }).ToList();
            var store = await _storeDetailService.GetStoreByUserIdAsync(userId); // để lấy StoreID
            ViewBag.ProductID = productId;
            ViewBag.StoreID = model.StoreID;
            ViewBag.StoreStatusText = store?.Status ?? "Pending"; // 👈 Thêm dòng này
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(ProductUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ExistingImages = await _productService.GetGalleryImageUrlsByProductIdAsync(model.ProductID);
                model.ExistingMainImage = await _productService.GetMainImageUrlsByProductIdAsync(model.ProductID); // lấy riêng ảnh chính

                model.Categories = (await _productService.GetActiveCategoriesAsync()).Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Name
                }).ToList();

                ViewBag.StoreID = model.StoreID;
                //ViewBag.UpdateSuccess = true;
                return View(model);
            }

            // Kiểm tra tên trùng
            if (await _productService.IsProductNameTakenAsync(model.Name, model.ProductID))
            {
                TempData["DuplicateName"] = true;

                // Reload các dữ liệu để hiển thị lại view nếu cần
                model.ExistingImages = await _productService.GetGalleryImageUrlsByProductIdAsync(model.ProductID);
                model.ExistingMainImage = await _productService.GetMainImageUrlsByProductIdAsync(model.ProductID);
                model.Categories = (await _productService.GetActiveCategoriesAsync()).Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Name
                }).ToList();
                ViewBag.StoreID = model.StoreID;

                return View(model);
            }

            // try
            // {
            //     var webRootPath = _webHostEnvironment.WebRootPath;
            //     await _productService.UpdateProductAsync(model, webRootPath);
            //     ViewBag.UpdateSuccess = true;
            // }
            // catch (Exception ex)
            // {
            //     ModelState.AddModelError(string.Empty, ex.Message);
            //     ViewBag.UpdateSuccess = false;
            // }
            try
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var result = await _productService.UpdateProductAsync(model, webRootPath);

                if (result.Success)
                {
                    ViewBag.UpdateSuccess = true;
                }
                else
                {
                    ViewBag.UpdateSuccess = false;
                    ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Update failed.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.UpdateSuccess = false;
            }


            // model.ExistingImages = await _productService.GetImageUrlsByProductIdAsync(model.ProductID);
            // model.ExistingMainImage = model.ExistingImages.FirstOrDefault();
            model.ExistingImages = await _productService.GetGalleryImageUrlsByProductIdAsync(model.ProductID);
            model.ExistingMainImage = await _productService.GetMainImageUrlsByProductIdAsync(model.ProductID); // lấy riêng ảnh chính

            model.Categories = (await _productService.GetActiveCategoriesAsync()).Select(c => new SelectListItem
            {
                Value = c.ID.ToString(),
                Text = c.Name
            }).ToList();

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
            // if (ModelState.IsValid)
            // {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            bool isSeller = await _storeDetailService.IsUserSellerAsync(user.Id);
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
            // }
            //return View(model);
        }

        public async Task<IActionResult> ViewProductType(Guid productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var productType = await _variantService.GetProductTypeByProductIdAsync(productId);
            if (productType.Any())
            {
                ViewBag.StoreId = productType.First().StoreID; // Lấy StoreID từ danh sách variant
            }
            ViewBag.ProductId = productId; // Lưu ProductId để sử dụng trong View
            var store = _storeDetailService.GetStoreByUserId(userId);
            var isStoreActive = await _productService.IsStoreActiveByProductIdAsync(productId);
            ViewBag.IsStoreActive = isStoreActive;
            ViewBag.StoreStatusText = store?.Status ?? "Pending"; // 👈 Thêm dòng này
            return View(productType);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProductType(Guid productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = new ProductVariantCreateViewModel
            {
                ProductID = productId
            };
            ViewBag.ProductID = productId;
            var store = _storeDetailService.GetStoreByUserId(userId);
            var isActive = await _variantService.IsStoreActiveByProductIdAsync(productId);
            ViewBag.IsActive = isActive ?? false;
            ViewBag.StoreStatusText = store?.Status ?? "Pending"; // 👈 Thêm dòng này
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _variantService.GetProductVariantForEditAsync(variantId);
            if (model == null)
            {
                return NotFound();
            }
            var store = _storeDetailService.GetStoreByUserId(userId);
            // Kiểm tra trạng thái của Store dựa trên variantId
            var isStoreActive = await _variantService.IsStoreActiveByVariantIdAsync(variantId);
            ViewBag.IsStoreActive = isStoreActive;
            ViewBag.StoreStatusText = store?.Status ?? "Pending"; // 👈 Thêm dòng này
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
                return Json(new ErroMess { msg = "You are not logged in!" });


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
                userId = order.UserID,
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
                PhoneCustomer = getInfoCustomer.PhoneNumber.StartsWith("0")
                    ? "+(84) " + getInfoCustomer.PhoneNumber.Substring(1)
                    : getInfoCustomer.PhoneNumber,
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
                return Json(new { success = false, message = "Invalid order ID." });


            try
            {
                var order = await _order.FindAsync(o => o.ID == id);
                if (order == null)
                    return Json(new { success = false, message = "Order not found." });

                var status = order.Status;
                if (!status.Equals("Pending", StringComparison.OrdinalIgnoreCase) &&
                    !status.Equals("Preparing In Kitchen", StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new { success = false, message = "Only orders with status Pending and Preparing In Kitchen can be cancelled." });
                }


                var orderDetails = await _orderDetail.ListAsync(d => d.OrderID == order.ID);
                if (orderDetails == null || !orderDetails.Any())
                    return Json(new { success = false, message = "Đơn hàng không có sản phẩm nào." });

                foreach (var item in orderDetails)
                {
                    item.Status = "Refunded";
                    item.ModifiedDate = DateTime.Now;
                    await _orderDetail.UpdateAsync(item);

                    var product = await _variantService.FindAsync(p => p.ID == item.ProductTypesID);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                        product.IsActive = true;
                        await _variantService.UpdateAsync(product);
                    }
                }

                decimal temAdmin = -1;
                if (order.VoucherID != null)
                {
                    var v = await _voucher.FindAsync(x => x.ID == order.VoucherID && x.IsActive);
                    if (v == null)
                        return Json(new { message = "Invalid voucher" });
                    decimal discount = v.DiscountType == "Percent"
                        ? order.TotalPrice * v.DiscountAmount / 100
                        : v.DiscountAmount;

                    if (v.MaxDiscountAmount.HasValue)
                        discount = Math.Min(discount, v.MaxDiscountAmount.Value);


                    discount = Math.Min(discount, order.TotalPrice);

                    if (v.IsGlobal)
                    {
                        temAdmin = order.TotalPrice - discount;
                    }

                }
                decimal finalPrice = temAdmin == -1 ? order.TotalPrice : temAdmin;
                var currentBalance = await _balance.GetBalance(order.UserID);
                var refundTransaction = new BalanceChange
                {
                    UserID = order.UserID,
                    MoneyChange = finalPrice,
                    MoneyBeforeChange = currentBalance,
                    MoneyAfterChange = currentBalance + order.TotalPrice,
                    Method = "Refund",
                    Status = "Success",
                    Display = true,
                    IsComplete = true,
                    CheckDone = true,
                    StartTime = DateTime.Now,
                    DueTime = DateTime.Now,
                    Description = $"Refund for order {order.ID} - Cancelled by Shop",
                };
                await _balance.AddAsync(refundTransaction);

                order.Status = "Cancelled by Shop";
                order.Description = string.IsNullOrEmpty(order.Description)
    ? $"CANCELLED BY SHOP-{DateTime.Now}"
    : $"{order.Description}#CANCELLED BY SHOP-{DateTime.Now}";

                order.PaymentStatus = "Refunded";
                order.ModifiedDate = DateTime.Now;
                await _order.UpdateAsync(order);


                await _orderDetail.SaveChangesAsync();
                await _variantService.SaveChangesAsync();
                await _order.SaveChangesAsync();
                await _balance.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("ReceivManageOrder");
                await _hubContext.Clients.All.SendAsync("ReceiveOrders");
                await _hubContext.Clients.All.SendAsync("ReceiveUpdate", new { message = "Data changed" });
                return Json(new { success = true, message = "Order has been cancelled successfully." });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while processing your request. Please try again or contact admin.",

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
                    return Json(new { success = false, message = "Order not found." });
                }

                // Trạng thái hủy sẽ không xử lý ở đây
                if (status == "CANCELLED BY USER" || status == "CANCELLED BY SHOP")
                {
                    return Json(new { success = false, message = "Cancellation status cannot be processed here." });

                }
                var orderDetails = await _orderDetail.ListAsync(d => d.OrderID == order.ID);
                if (orderDetails == null || !orderDetails.Any())
                    return Json(new { success = false, message = "There are no products in this order." });


                foreach (var item in orderDetails)
                {
                    item.Status = status;
                    item.ModifiedDate = DateTime.Now;
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
                await _hubContext.Clients.All.SendAsync("ReceivManageOrder");
                await _hubContext.Clients.All.SendAsync("ReceiveOrders");
                return Json(new { success = true, message = $"Order status updated successfully: {status}" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while updating the status. Please try again later." });
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
                return NotFound("Complaint not found.");
            var getOrderDetail = await _orderDetail.FindAsync(u => u.ID == getComplaint.OrderDetailID);
            if (getOrderDetail == null)
                return NotFound("Order detail not found.");
            var getOrder = await _order.FindAsync(u => u.ID == getOrderDetail.OrderID);
            if (getOrder == null)
                return NotFound("Order not found.");
            var getProductType = await this._variantService.FindAsync(u => u.ID == getOrderDetail.ProductTypesID);
            if (getProductType == null)
                return NotFound("Productype not found.");
            var getProduct = await _product.FindAsync(u => u.ID == getProductType.ProductID);
            if (getProduct == null)
                return NotFound("Product not found.");
            var getUser = await _userManager.FindByIdAsync(getOrder.UserID);
            if (getUser == null)
                return NotFound("User not found.");
            var getStore = await _storedetail.FindAsync(u => u.ID == getProduct.StoreID);
            if (getStore == null)
                return NotFound("Store not found.");
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
                return Json(new ErroMess { msg = "You are not logged in!" });

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
                return Json(new { success = false, message = "Invalid submission information." });
            }

            var complaint = await this._complaintService.FindAsync(c => c.ID == id);
            if (complaint == null)
            {
                return Json(new { success = false, message = "Complaint not found." });
            }
            if (complaint.Status.ToLower() == "Refund".ToLower())
            {
                return Json(new { success = false, message = "This complaint has already been refunded." });
            }

            switch (action.ToLower())
            {
                case "refund":

                    complaint.Status = "Refund";
                    complaint.Reply = $"[Refund] - {note}";
                    complaint.ReplyDate = DateTime.Now;

                    try
                    {
                        var orderDetails = await _orderDetail.FindAsync(d => d.ID == complaint.OrderDetailID);
                        if (orderDetails == null)
                            return Json(new { success = false, message = "There are no products in this order." });
                        var order = await this._order.FindAsync(u => u.ID == orderDetails.OrderID);
                        if (order == null)
                            return Json(new { success = false, message = "Order not found." });

                        orderDetails.Status = "Refunded";
                        orderDetails.ModifiedDate = DateTime.Now;
                        await _orderDetail.UpdateAsync(orderDetails);

                        var getPriceRefun = orderDetails.TotalPrice;
                          if (order.VoucherID != null)
                        {
                            getPriceRefun = await _order.CalculateRefundAmount(order, orderDetails);
                        }
                      //  order.TotalPrice -= getPriceRefun;


                        var currentBalance = await _balance.GetBalance(order.UserID);
                        var refundTransaction = new BalanceChange
                        {
                            UserID = order.UserID,
                            MoneyChange = getPriceRefun,
                            MoneyBeforeChange = currentBalance,
                            MoneyAfterChange = currentBalance + getPriceRefun,
                            Method = "Refund",
                            Status = "Success",
                            Display = true,
                            IsComplete = true,
                            CheckDone = true,
                            StartTime = DateTime.Now,
                            DueTime = DateTime.Now,
                            Description= $"Refund for order {order.ID} - Complaint resolved by seller",
                        };
                      
                        await _balance.AddAsync(refundTransaction);

                        // Cập nhật trạng thái đơn hàng
                     /*   order.Status = "Refunded";
                        order.PaymentStatus = "Refunded";
                        order.ModifiedDate = DateTime.Now;
                        order.Description = string.IsNullOrEmpty(order.Description)
                            ? $"Refunded - {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
                            : $"{order.Description}#Refunded - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";*/
                      
                        await _order.UpdateAsync(order);

                        // Lưu thay đổi
                        await _orderDetail.SaveChangesAsync();

                        //   await _order.SaveChangesAsync();
                        await _balance.SaveChangesAsync();

                        await _hubContext.Clients.All.SendAsync("ReceiveUpdate", new { message = "Data changed" });
                        mess = "The order has been refunded and cancelled successfully.";

                    }
                    catch (Exception)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "An error occurred while processing the refund. Please try again or contact the administrator."
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
                            complaint.Reply = $"[WARRANTY] Order Tracking = {ortrack}\n{note}";

                            var orderDetail = await _orderDetail.FindAsync(x => x.ID == complaint.OrderDetailID);
                            if (orderDetail == null)
                                throw new Exception("Order detail for warranty not found.");

                            var product = await _variantService.FindAsync(p => p.ID == orderDetail.ProductTypesID);
                            if (product == null || !product.IsActive)
                                throw new Exception("Product does not exist or is inactive.");

                            if (orderDetail.Quantity > product.Stock)
                                throw new Exception("Warranty quantity exceeds available stock.");

                            var originalOrder = await _order.FindAsync(x => x.ID == orderDetail.OrderID);
                            if (originalOrder == null)
                                throw new Exception("Original order not found.");

                            var voucherCode = $"Warranty-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
                            var getPriceRefun = originalOrder.TotalPrice;
                            if (originalOrder.VoucherID != null)
                            {
                                getPriceRefun = await _order.CalculateRefundAmount(originalOrder, orderDetail);
                            }
                          //  originalOrder.TotalPrice -= getPriceRefun;

                            var voucher = new Voucher
                            {
                                ID = Guid.NewGuid(),
                                Code = voucherCode,
                                DiscountAmount = 100,
                                DiscountType = "Percent",
                                StartDate = DateTime.Now,
                                ExpirationDate = DateTime.Now.AddDays(3),
                                MaxUsage = 1,
                                CurrentUsage = 0,
                                MinOrderValue = 0,
                                IsActive = false,
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
                                OrderCode = "",
                                VoucherID = voucher.ID,
                                DeliveryAddress = originalOrder.DeliveryAddress ?? "Automatic warranty",
                                Note = "Automatic warranty order",
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
                                Status = "Pending",
                                ProductTypeName = product.Name,
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
                                Description = $"Warranty order created for Order Tracking: {order.ID}",
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
                            return Json(new { success = false, msg = "Operation failed." });
                        mess = "Free warranty order created successfully!";
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, msg = "Error processing warranty: " + ex.Message });
                    }

                    break;


                case "dispute":
                    complaint.Status = "Report to Admin";
                    complaint.Reply = $"[DISPUTE] {note}";

                    complaint.IsReportAdmin = true;
                    complaint.AdminReportStatus = "Pending";


                    break;

                default:
                    return Json(new { success = false, message = "Invalid action type." });

            }

            complaint.ReplyDate = DateTime.Now;

            try
            {
                await this._complaintService.UpdateAsync(complaint);
                await this._complaintService.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveOrders");
                return Json(new { success = true, message = mess });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error saving data: " + ex.Message });
            }

        }
        public async Task<IActionResult> ManagerVoucher()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllVoucher()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { error = "You are not logged in!" });

            var store = await _storedetail.FindAsync(s => s.UserID == user.Id);
            if (store == null)
                return Json(new { error = "Store not found." });
            try
            {
                var data = _voucher.GetAll().Where(u => u.StoreID == store.ID).Select(v => new
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
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { error = "You are not logged in!" });

            var store = await _storedetail.FindAsync(s => s.UserID == user.Id);
            if (store == null)
                return Json(new { error = "Store not found." });
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(v.Code))
                errors["code"] = "Code is required.";
            else
            {
                if (v.Code.Length < 3 || v.Code.Length > 20)
                    errors["code"] = "Code must be between 3 and 20 characters.";
                else if (await _voucher.FindAsync
                    (x => x.Code == v.Code.ToUpper() && x.StoreID == store.ID) != null)
                    errors["code"] = "Code already exists for this store.";
            }
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

            if (string.IsNullOrWhiteSpace(v.Scope) && v.DiscountType != "Fixed")
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
            decimal temp;
            decimal? result;
            if (decimal.TryParse(v.Scope, out temp))
            {
                result = temp;
            }
            else
            {
                result = null;
            }


            try
            {
                var entity = new Voucher
                {
                    ID = Guid.NewGuid(),
                    Code = v.Code.ToUpper(),
                    DiscountAmount = v.DiscountAmount,
                    DiscountType = v.DiscountType,
                    StartDate = startDate,
                    ExpirationDate = expirationDate,
                    MaxDiscountAmount = result,
                    MaxUsage = v.MaxUsage,
                    CurrentUsage = v.CurrentUsage,
                    MinOrderValue = v.MinOrderValue,
                    IsActive = v.IsActive,
                    CreatedDate = DateTime.Now,
                    StoreID = store.ID
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

            if (string.IsNullOrWhiteSpace(v.Scope) && v.DiscountType != "Fixed")
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
            decimal temp;
            decimal? result;
            if (decimal.TryParse(v.Scope, out temp))
            {
                result = temp;
            }
            else
            {
                result = null;
            }
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
                entity.MaxDiscountAmount = result;
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

        public async Task<IActionResult> Chat()
        {
            return View();
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetDateConfig()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var today = DateTime.Now.Date;
                var todayStr = today.ToString("yyyy-MM-dd");
                var fallbackConfig = new
                {
                    minDate = todayStr,
                    maxDate = todayStr,
                    defaultDays = 1
                };
                var store = await _storedetail.FindAsync(u => u.UserID == user.Id);
                if (store == null) return Json(fallbackConfig);
                var products = await _product.ListAsync(p => p.StoreID == store.ID);
                if (!products.Any()) return Json(fallbackConfig);
                var productIds = products.Select(p => p.ID).ToList();
                var variants = await _variantService.ListAsync(v => productIds.Contains(v.ProductID));
                if (!variants.Any()) return Json(fallbackConfig);
                var variantIds = variants.Select(v => v.ID).ToList();
                var orderDetails = await _orderDetail.ListAsync(od => variantIds.Contains(od.ProductTypesID));
                if (!orderDetails.Any()) return Json(fallbackConfig);
                var orderIds = orderDetails.Select(od => od.OrderID).Distinct().ToList();
                var confirmedStatuses = new[] { "CONFIRMED" };
                var cancelledStatuses = new[] { "CANCELLED BY USER", "CANCELLED BY SHOP" };
                var orders = await _order.ListAsync(o => orderIds.Contains(o.ID) && (confirmedStatuses.Contains(o.Status.ToUpper()) || cancelledStatuses.Contains(o.Status.ToUpper())));
                if (!orders.Any()) return Json(fallbackConfig);
                var minDateValue = orders.Min(o => o.CreatedDate).Date;
                var maxDateValue = orders.Max(o => o.CreatedDate).Date;
                int totalDays = (maxDateValue - minDateValue).Days + 1;
                return Json(new
                {
                    minDate = minDateValue.ToString("yyyy-MM-dd"),
                    maxDate = maxDateValue.ToString("yyyy-MM-dd"),
                    defaultDays = totalDays < 30 ? totalDays : 30
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error retrieving date configuration", message = ex.Message });

            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStatisticsByDate(DateTime? from = null, DateTime? to = null)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { error = "You are not logged in!" });

                var store = await _storedetail.FindAsync(s => s.UserID == user.Id);
                if (store == null)
                    return Json(new { error = "Store not found." });

                var products = await _product.ListAsync(p => p.StoreID == store.ID);
                if (!products.Any())
                    return Json(new { error = "No products found." });

                var productIds = products.Select(p => p.ID).ToList();
                var variants = await _variantService.ListAsync(v => productIds.Contains(v.ProductID));
                if (!variants.Any())
                    return Json(new { error = "No product variants found." });

                var variantIds = variants.Select(v => v.ID).ToList();
                var orderDetails = await _orderDetail.ListAsync(od => variantIds.Contains(od.ProductTypesID));
                if (!orderDetails.Any())
                    return Json(new { error = "No order details found." });

                var orderIds = orderDetails.Select(od => od.OrderID).Distinct().ToList();
                var orders = await _order.ListAsync(o => orderIds.Contains(o.ID));
                if (!orders.Any())
                    return Json(new { error = "No orders found." });

                var minDate = orders.Min(o => o.CreatedDate).Date;
                var maxDate = orders.Max(o => o.CreatedDate).Date;

                if (!from.HasValue || !to.HasValue)
                {
                    to = maxDate;
                    from = to.Value.AddDays(-29);
                }

                var fromDate = from.Value.Date;
                var toDate = to.Value.Date;

                if (fromDate < minDate) fromDate = minDate;
                if (toDate > maxDate) toDate = maxDate;

                if (fromDate > toDate)
                    return BadRequest(new { error = "Start date cannot be greater than end date." });

                var daysDiff = (int)(toDate - fromDate).TotalDays + 1;

                var ordersInRange = orders.Where(o => o.CreatedDate.Date >= fromDate && o.CreatedDate.Date <= toDate).ToList();

                var successOrders = ordersInRange.Where(o => o.Status.ToLower() == "CONFIRMED".ToLower()).ToList();
                var canceledOrders = ordersInRange.Where(o => o.Status.ToLower() == "Cancelled by User".ToLower() || o.Status.ToLower() == "Cancelled by Shop".ToLower()).ToList();

                var successOrderIds = successOrders.Select(o => o.ID).ToList();
                var successOrderDetails = orderDetails.Where(od => successOrderIds.Contains(od.OrderID)).ToList();


                var totalEarnings = successOrders.Sum(o => o.TotalPrice);

                var today = DateTime.Now.Date;
                var pendingBalance = successOrders
                    .Where(o => (today - o.CreatedDate.Date).TotalDays < 3)
                    .Sum(o => o.TotalPrice);

                var processedOrders = successOrders.Count + canceledOrders.Count;
                var totalOrders = processedOrders;
                double cancellationRate = processedOrders > 0
                    ? Math.Round((double)canceledOrders.Count * 100 / processedOrders, 1)
                    : 0.0;

                var result = new
                {
                    totalOrders,
                    totalEarnings,
                    pendingBalance,
                    cancellationRate,
                    period = new
                    {
                        from = fromDate.ToString("dd/MM/yyyy"),
                        to = toDate.ToString("dd/MM/yyyy"),
                        days = daysDiff,
                        isAdjusted = true
                    },
                    dateConfig = new
                    {
                        minDate = minDate.ToString("yyyy-MM-dd"),
                        maxDate = maxDate.ToString("yyyy-MM-dd")
                    }
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Server error while processing data", message = ex.Message });

            }
        }
        [HttpGet]
        public async Task<IActionResult> GetValidMonths()
        {
            var today = DateTime.Now.Date;
            var todayStr = today.ToString("yyyy-MM");
            var fallbackMonths = new List<string> { todayStr };
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var store = await _storedetail.FindAsync(u => u.UserID == user.Id);
                if (store == null) return Json(fallbackMonths);
                var products = await _product.ListAsync(p => p.StoreID == store.ID);
                if (!products.Any()) return Json(fallbackMonths);
                var productIds = products.Select(p => p.ID).ToList();
                var variants = await _variantService.ListAsync(v => productIds.Contains(v.ProductID));
                if (!variants.Any()) return Json(fallbackMonths);
                var variantIds = variants.Select(v => v.ID).ToList();
                var orderDetails = await _orderDetail.ListAsync(od => variantIds.Contains(od.ProductTypesID));
                if (!orderDetails.Any()) return Json(fallbackMonths);
                var orderIds = orderDetails.Select(od => od.OrderID).Distinct().ToList();
                var confirmedStatuses = new[] { "CONFIRMED" };
                var cancelledStatuses = new[] { "CANCELLED BY USER", "CANCELLED BY SHOP" };
                var orders = await _order.ListAsync(o => orderIds.Contains(o.ID) && (confirmedStatuses.Contains(o.Status.ToUpper()) || cancelledStatuses.Contains(o.Status.ToUpper())));
                if (!orders.Any()) return Json(fallbackMonths);
                var months = orders
                    .Select(o => o.CreatedDate.ToString("yyyy-MM"))
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
                return Json(months);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error retrieving month configuration", message = ex.Message });

            }
        }


        [HttpGet]
        public async Task<IActionResult> GetMonthlyData(string month)
        {
            if (string.IsNullOrEmpty(month))
                month = DateTime.Now.ToString("yyyy-MM");

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { error = "You are not logged in!" });
                var store = await _storedetail.FindAsync(s => s.UserID == user.Id);
                if (store == null)
                    return Json(new { error = "Store not found." });
                var products = await _product.ListAsync(p => p.StoreID == store.ID);
                if (!products.Any())
                    return Json(new { error = "No products found." });
                var productIds = products.Select(p => p.ID).ToList();
                var variants = await _variantService.ListAsync(v => productIds.Contains(v.ProductID));
                if (!variants.Any())
                    return Json(new { error = "No product variants found." });
                var variantIds = variants.Select(v => v.ID).ToList();
                var orderDetails = await _orderDetail.ListAsync(od => variantIds.Contains(od.ProductTypesID));
                if (!orderDetails.Any())
                    return Json(new { error = "No order details found." });
                var orderIds = orderDetails.Select(od => od.OrderID).Distinct().ToList();
                var orders = await _order.ListAsync(o => orderIds.Contains(o.ID));
                if (!orders.Any())
                    return Json(new { error = "No orders found." });

                var confirmedStatuses = new[] { "CONFIRMED" };
                var cancelledStatuses = new[] { "CANCELLED BY USER", "CANCELLED BY SHOP" };
                int year = int.Parse(month.Split('-')[0]);
                int m = int.Parse(month.Split('-')[1]);
                int daysInMonth = DateTime.DaysInMonth(year, m);
                var chartOrders = new Dictionary<string, int>();
                var chartEarnings = new Dictionary<string, decimal>();
                var chartCanceled = new Dictionary<string, int>();
                var chartCustomers = new Dictionary<string, int>();
                int totalOrders = 0;
                decimal totalEarnings = 0;
                int totalCanceled = 0;
                HashSet<string> uniqueCustomers = new HashSet<string>();
                DateTime now = DateTime.Now.Date;
                for (int day = 1; day <= daysInMonth; day++)
                {
                    var date = new DateTime(year, m, day);
                    if (date > now) break;
                    string dateKey = date.ToString("yyyy-MM-dd");
                    var dayOrders = orders.Where(o =>
                    o.CreatedDate.Date == date &&
                     (confirmedStatuses.Contains(o.Status.ToUpper()) || cancelledStatuses.Contains(o.Status.ToUpper())));
                    int orderCount = dayOrders.Count();
                    chartOrders[dateKey] = orderCount;
                    totalOrders += orderCount;
                    decimal earning = dayOrders.Where(o => confirmedStatuses.Contains(o.Status.ToUpper())).Sum(o => o.TotalPrice);
                    chartEarnings[dateKey] = earning;
                    totalEarnings += earning;
                    int canceledCount = dayOrders.Count(o => cancelledStatuses.Contains(o.Status.ToUpper()));
                    chartCanceled[dateKey] = canceledCount;
                    totalCanceled += canceledCount;
                    var customersInDay = dayOrders
                        .Where(o => o.UserID != null)
                        .Select(o => o.UserID)
                        .Distinct()
                        .ToList();
                    chartCustomers[dateKey] = customersInDay.Count;
                    foreach (var cid in customersInDay)
                        uniqueCustomers.Add(cid);
                }
                var summary = new
                {
                    orders = totalOrders,
                    earnings = totalEarnings,
                    refunds = totalCanceled,
                    newCustomers = uniqueCustomers.Count
                };
                var chartData = new
                {
                    orders = chartOrders,
                    earnings = chartEarnings,
                    refunds = chartCanceled,
                    newCustomers = chartCustomers
                };

                return Json(new { summary, chartData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Server error while processing data", message = ex.Message });

            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(string period = "today", string search = "")
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { error = "You are not logged in!" });
                var store = await _storedetail.FindAsync(u => u.UserID == user.Id);
                if (store == null)
                    return Json(new { error = "Store not found!" });

                var products = await _product.ListAsync(p => p.StoreID == store.ID);
                if (!products.Any())
                    return Json(new List<object>());
                var productIds = products.Select(p => p.ID).ToList();
                var productTypes = await _variantService.ListAsync(pt => productIds.Contains(pt.ProductID));
                if (!productTypes.Any())
                    return Json(new List<object>());
                var productTypeIds = productTypes.Select(pt => pt.ID).ToList();
                var orderDetails = await _orderDetail.ListAsync(od => productTypeIds.Contains(od.ProductTypesID));
                if (!orderDetails.Any())
                    return Json(new List<object>());
                var orderIds = orderDetails.Select(od => od.OrderID).Distinct().ToList();
                var orders = await _order.ListAsync(o => orderIds.Contains(o.ID) && o.Status.ToLower() == "confirmed");
                if (!orders.Any())
                    return Json(new List<object>());
                var confirmedOrderIds = orders.Select(o => o.ID).ToHashSet();
                var confirmedOrderDetails = orderDetails.Where(od => confirmedOrderIds.Contains(od.OrderID)).ToList();
                if (!confirmedOrderDetails.Any())
                    return Json(new List<object>());
                var now = DateTime.Now;
                IEnumerable<OrderDetail> filteredOrderDetails = confirmedOrderDetails;

                switch (period?.ToLower())
                {
                    case "today":
                        filteredOrderDetails = filteredOrderDetails.Where(od => od.CreatedDate.Date == now.Date);
                        break;
                    case "yesterday":
                        filteredOrderDetails = filteredOrderDetails.Where(od => od.CreatedDate.Date == now.AddDays(-1).Date);
                        break;
                    case "last7days":
                        filteredOrderDetails = filteredOrderDetails.Where(od => od.CreatedDate >= now.AddDays(-7));
                        break;
                    case "last30days":
                        filteredOrderDetails = filteredOrderDetails.Where(od => od.CreatedDate >= now.AddDays(-30));
                        break;
                    case "thismonth":
                        filteredOrderDetails = filteredOrderDetails.Where(od => od.CreatedDate.Month == now.Month && od.CreatedDate.Year == now.Year);
                        break;
                    case "lastmonth":
                        var lastMonth = now.AddMonths(-1);
                        filteredOrderDetails = filteredOrderDetails.Where(od =>
                            od.CreatedDate.Month == lastMonth.Month && od.CreatedDate.Year == lastMonth.Year);
                        break;
                    case "alltime":

                        break;
                    default:

                        break;
                }
                var joinPT = productTypes.ToDictionary(pt => pt.ID, pt => pt.ProductID);
                var filteredProductIds = filteredOrderDetails
                    .Where(od => joinPT.ContainsKey(od.ProductTypesID))
                    .Select(od => joinPT[od.ProductTypesID])
                    .Distinct()
                    .ToList();
                var productImages = await _productImageService.ListAsync(img =>
                    filteredProductIds.Contains(img.ProductID) && img.IsMain);
                var topProducts = filteredOrderDetails
      .Where(od => joinPT.ContainsKey(od.ProductTypesID))
      .GroupBy(od => joinPT[od.ProductTypesID])
      .Select(g =>
      {
          var product = products.FirstOrDefault(p => p.ID == g.Key);
          var topTypeId = g.GroupBy(x => x.ProductTypesID)
                           .OrderByDescending(x => x.Sum(y => y.Quantity))
                           .First().Key;
          var topType = productTypes.FirstOrDefault(pt => pt.ID == topTypeId);
          decimal price = topType?.SellPrice ?? 0;
          int stock = topType?.Stock ?? 0;


          return new
          {
              id = product?.ID ?? Guid.Empty,
              name = product?.Name ?? "Unknown",
              price = price,
              stock = stock,
              image = "🍪",
              orders = g.Sum(x => x.Status.ToLower() == "confirmed" ? x.Quantity : 0),
              totalSell = g.Sum(x => x.Quantity * x.ProductPrice),
              date = g.Max(x => x.CreatedDate).ToString("dd MMM yyyy"),
          };
      })
      .Where(x => x.id != Guid.Empty)
      .OrderByDescending(x => x.totalSell)
      .ToList();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    topProducts = topProducts
                        .Where(p => p.name.ToLower().Contains(search.ToLower()))
                        .ToList();
                }

                return Json(topProducts);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetCustomers(string period = "today", string search = "")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { error = "You are not logged in!" });
            var getStore = await _storedetail.FindAsync(u => u.UserID == user.Id);
            if (getStore == null)
                return Json(new { error = "Store not found!" });

            var products = await _product.ListAsync(p => p.StoreID == getStore.ID);
            if (!products.Any())
                return Json(new List<object>());
            var productIds = products.Select(p => p.ID).ToList();
            var productTypes = await _variantService.ListAsync(pt => productIds.Contains(pt.ProductID));
            if (!productTypes.Any())
                return Json(new List<object>());
            var productTypeIds = productTypes.Select(pt => pt.ID).ToList();
            var orderDetails = await _orderDetail.ListAsync(od => productTypeIds.Contains(od.ProductTypesID));
            if (!orderDetails.Any())
                return Json(new List<object>());
            var orderIds = orderDetails.Select(od => od.OrderID).Distinct().ToList();
            var orders = await _order.ListAsync(o => orderIds.Contains(o.ID) && o.Status.ToLower() == "confirmed");
            var now = DateTime.Now;
            IEnumerable<Order> filteredOrders = orders;
            switch (period.ToLower())
            {
                case "today":
                    filteredOrders = orders.Where(o => o.CreatedDate.Date == now.Date);
                    break;
                case "yesterday":
                    filteredOrders = orders.Where(o => o.CreatedDate.Date == now.AddDays(-1).Date);
                    break;
                case "last7days":
                    filteredOrders = orders.Where(o => o.CreatedDate >= now.AddDays(-7));
                    break;
                case "last30days":
                    filteredOrders = orders.Where(o => o.CreatedDate >= now.AddDays(-30));
                    break;
                case "thismonth":
                    filteredOrders = orders.Where(o => o.CreatedDate.Month == now.Month && o.CreatedDate.Year == now.Year);
                    break;
                case "lastmonth":
                    var lastMonth = now.AddMonths(-1);
                    filteredOrders = orders.Where(o => o.CreatedDate.Month == lastMonth.Month && o.CreatedDate.Year == lastMonth.Year);
                    break;
                case "alltime":
                    break;
            }
            var filteredOrderIds = filteredOrders.Select(o => o.ID).ToHashSet();
            var filteredOrderDetails = orderDetails.Where(od => filteredOrderIds.Contains(od.OrderID)).ToList();
            var orderDict = filteredOrders.ToDictionary(o => o.ID, o => o);
            var customerGroups = filteredOrderDetails
                .Where(od => orderDict.ContainsKey(od.OrderID))
                .GroupBy(od => orderDict[od.OrderID].UserID)
                .Select(g => new
                {
                    UserID = g.Key,
                    TotalAmount = g.Sum(od => od.Quantity * od.ProductPrice),
                    OrderCount = g.Select(od => od.OrderID).Distinct().Count(),
                    LastOrderDate = g.Max(od => orderDict[od.OrderID].CreatedDate)
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToList();

            if (!customerGroups.Any())
                return Json(new List<object>());

            var userIds = customerGroups.Select(x => x.UserID).ToList();
            var users = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.PhoneNumber,
                    u.ImageUrl
                })
                .ToListAsync();
            var userRoles = new List<dynamic>();
            foreach (var userId in userIds)
            {
                var userEntity = await _userManager.FindByIdAsync(userId);
                if (userEntity != null)
                {
                    var roles = await _userManager.GetRolesAsync(userEntity);
                    var roleName = roles.FirstOrDefault() ?? "Customer";
                    userRoles.Add(new { UserId = userId, RoleName = roleName });
                }
            }

            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();
                users = users.Where(u =>
                    (u.UserName != null && u.UserName.ToLower().Contains(searchLower)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchLower))
                ).ToList();
                customerGroups = customerGroups.Where(c => users.Any(u => u.Id == c.UserID)).ToList();
            }

            var totalAmountAll = customerGroups.Sum(x => x.TotalAmount);

            var result = customerGroups.Select((c, idx) =>
            {
                var u = users.FirstOrDefault(u => u.Id == c.UserID);
                var role = userRoles.FirstOrDefault(ur => ur.UserId == c.UserID)?.RoleName ?? "Customer";

                return new
                {
                    stt = idx + 1,
                    id = u?.Id,
                    userName = u?.UserName ?? "Unknown",
                    rolebuy = role,
                    phone = u?.PhoneNumber ?? "",
                    stock = c.OrderCount,
                    amount = c.TotalAmount,
                    growth = totalAmountAll == 0 ? 0 : Math.Round(100m * c.TotalAmount / totalAmountAll, 2),
                    lastOrderDate = c.LastOrderDate.ToString("dd/MM/yyyy"),
                    image = u?.ImageUrl ?? "/assets/imgs/theme/icons/icon-user.svg",
                };
            }).ToList();

            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> ProductStatistics(string period = "today")
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { error = "You are not logged in!" });
                var getStore = await _storedetail.FindAsync(u => u.UserID == user.Id);
                if (getStore == null)
                    return Json(new { error = "Store not found!" });

                var products = await _product.ListAsync(p => p.StoreID == getStore.ID);
                if (!products.Any())
                    return Json(new { success = true, data = new { series = new double[0], labels = new string[0], orders = new int[0], total = 0 } });

                var productIds = products.Select(p => p.ID).ToList();
                var productTypes = await _variantService.ListAsync(pt => productIds.Contains(pt.ProductID));
                if (!productTypes.Any())
                    return Json(new { success = true, data = new { series = new double[0], labels = new string[0], orders = new int[0], total = 0 } });

                var productTypeIds = productTypes.Select(pt => pt.ID).ToList();

                DateTime startDate = GetStartDateByPeriod(period);
                DateTime endDate = DateTime.Now;

                var orderDetails = period.ToLower() == "alltime" ?
                    await _orderDetail.ListAsync(od => productTypeIds.Contains(od.ProductTypesID)) :
                    await _orderDetail.ListAsync(od =>
                        productTypeIds.Contains(od.ProductTypesID) &&
                        od.CreatedDate >= startDate &&
                        od.CreatedDate <= endDate);
                var validStatuses = new[]
{
    "CONFIRMED",
    "DELIVERING",
    "PREPARING IN KITCHEN",
    "CANCELLED BY SHOP",
    "CANCELLED BY USER",
    "DELIVERY FAILED"
};

                if (!orderDetails.Any())
                    return Json(new { success = true, data = new { series = new double[0], labels = new string[0], orders = new int[0], total = 0 } });

                var orderIds = orderDetails.Select(od => od.OrderID).Distinct().ToList();
                var ordersList = await _order.ListAsync(o => orderIds.Contains(o.ID) && validStatuses.Contains(o.Status.ToUpper()));

                var validOrderIds = ordersList.Select(o => o.ID).ToHashSet();
                var validOrderDetails = orderDetails.Where(od => validOrderIds.Contains(od.OrderID)).ToList();
                var productTypeDict = productTypes.ToDictionary(pt => pt.ID, pt => pt);
                var productDict = products.ToDictionary(p => p.ID, p => p);

                var stats = validOrderDetails
                    .GroupBy(od => od.ProductTypesID)
                    .Select(g =>
                    {
                        var productType = productTypeDict[g.Key];
                        var product = productDict[productType.ProductID];
                        return new
                        {
                            label = string.IsNullOrEmpty(productType.Name) ? product.Name : $"{product.Name} ({productType.Name})",
                            orderCount = g.Sum(x => x.Quantity)
                        };
                    })
                    .Where(x => x.orderCount > 0)
                    .OrderByDescending(x => x.orderCount)
                    .ToList();

                var labels = stats.Select(x => x.label).ToArray();
                var ordersArr = stats.Select(x => x.orderCount).ToArray();
                int totalOrders = ordersArr.Sum();
                var rawPercentages = ordersArr.Select(x => totalOrders > 0 ? x * 100.0 / totalOrders : 0).ToList();
                var roundedPercentages = rawPercentages.Select(x => Math.Round(x, 1)).ToList();
                double totalPercent = roundedPercentages.Sum();
                double diff = Math.Round(100.0 - totalPercent, 1);
                if (Math.Abs(diff) > 0.0001 && roundedPercentages.Count > 0)
                {
                    int maxIdx = roundedPercentages.IndexOf(roundedPercentages.Max());
                    roundedPercentages[maxIdx] += diff;
                    if (roundedPercentages[maxIdx] < 0) roundedPercentages[maxIdx] = 0;
                }
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        series = ordersArr,
                        percentages = roundedPercentages,
                        labels = labels,
                        total = totalOrders
                    }
                });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "An error occurred: " + ex.Message });

            }
        }
        [HttpGet]
        public async Task<IActionResult> RecentOrders(string period = "today")
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { error = "You are not logged in!" });
                var getStore = await _storedetail.FindAsync(u => u.UserID == user.Id);
                if (getStore == null)
                    return Json(new { error = "Store not found!" });

                var products = await _product.ListAsync(p => p.StoreID == getStore.ID);
                if (!products.Any())
                    return Json(new { success = true, data = new List<object>() });
                var productIds = products.Select(p => p.ID).ToList();
                var productTypes = await _variantService.ListAsync(pt => productIds.Contains(pt.ProductID));
                if (!productTypes.Any())
                    return Json(new { success = true, data = new List<object>() });
                var productTypeIds = productTypes.Select(pt => pt.ID).ToList();
                DateTime startDate = GetStartDateByPeriod(period);
                DateTime endDate = DateTime.Now;

                var orderDetails = period.ToLower() == "alltime" ?
                    await _orderDetail.ListAsync(od => productTypeIds.Contains(od.ProductTypesID)) :
                    await _orderDetail.ListAsync(od =>
                        productTypeIds.Contains(od.ProductTypesID) &&
                        od.CreatedDate >= startDate &&
                        od.CreatedDate <= endDate);
                if (!orderDetails.Any())
                    return Json(new { success = true, data = new List<object>() });

                var orderIds = orderDetails.Select(od => od.OrderID).Distinct().ToList();
                var orders = await _order.ListAsync(o => orderIds.Contains(o.ID),
                    orderBy: q => q.OrderByDescending(x => x.CreatedDate));
                if (!orders.Any())
                    return Json(new { success = true, data = new List<object>() });
                var userIds = orders.Select(o => o.UserID).Distinct().ToList();
                var users = await _userManager.Users
                    .Where(u => userIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => new { u.UserName, u.FirstName, u.LastName });
                var result = orders.Select(order =>
                {
                    var orderDetailsForOrder = orderDetails.Where(od => od.OrderID == order.ID).ToList();
                    var userName = users.ContainsKey(order.UserID) ? users[order.UserID] : null;
                    var displayName = userName != null ?
                        (!string.IsNullOrEmpty(userName.FirstName) || !string.IsNullOrEmpty(userName.LastName) ?
                            $"{userName.FirstName} {userName.LastName}".Trim() : userName.UserName) : "Unknown";
                    return new
                    {
                        id = order.OrderTracking,
                        customer = new
                        {
                            name = displayName
                        },
                        quantity = orderDetailsForOrder.Sum(od => od.Quantity),
                        amount = order.TotalPrice,
                        status = order.Status,
                        createdDate = order.CreatedDate
                    };
                }).ToList();

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "An error occurred: " + ex.Message });

            }
        }

        private DateTime GetStartDateByPeriod(string period)
        {
            DateTime now = DateTime.Now;
            return period.ToLower() switch
            {
                "today" => now.Date,
                "yesterday" => now.Date.AddDays(-1),
                "last7days" => now.Date.AddDays(-7),
                "last30days" => now.Date.AddDays(-30),
                "thismonth" => new DateTime(now.Year, now.Month, 1),
                "lastmonth" => new DateTime(now.Year, now.Month, 1).AddMonths(-1),
                "alltime" => DateTime.MinValue,
                _ => now.Date
            };
        }
        [HttpGet]
        public async Task<IActionResult> ReRegisterStore()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var store = await _storeDetailService.FindAsync(s => s.UserID == user.Id && s.Status == "Rejected");
            if (store == null) return RedirectToAction("ViewStore");

            var vm = new StoreViewModel
            {
                ID = store.ID,
                Name = store.Name,
                LongDescriptions = store.LongDescriptions,
                ShortDescriptions = store.ShortDescriptions,
                Address = store.Address,
                Phone = store.Phone,
                Img = store.ImageUrl,
                Status = "Pending",
                IsActive = true,
                UserID = store.UserID,
                UserName = user.UserName
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReRegisterStore(StoreViewModel model, IFormFile imageFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // ==== Thủ công kiểm tra ====
            if (string.IsNullOrWhiteSpace(model.Name))
                ModelState.AddModelError("Name", "Name is required.");

            if (string.IsNullOrWhiteSpace(model.ShortDescriptions))
                ModelState.AddModelError("ShortDescriptions", "Short description is required.");

            if (string.IsNullOrWhiteSpace(model.LongDescriptions))
                ModelState.AddModelError("LongDescriptions", "Long description is required.");

            if (string.IsNullOrWhiteSpace(model.Phone))
                ModelState.AddModelError("Phone", "Phone number is required.");

            if (string.IsNullOrWhiteSpace(model.Address))
                ModelState.AddModelError("Address", "Address is required.");

            // Nếu không có ảnh cũ và cũng không upload ảnh mới → lỗi
            bool hasOldImage = !string.IsNullOrEmpty(model.Img);
            bool hasNewImage = imageFile != null && imageFile.Length > 0;

            if (!hasOldImage && !hasNewImage)
            {
                ModelState.AddModelError("Img", "Please upload an image.");
            }


            // if (!ModelState.IsValid)
            //     return View(model);

            // Upload ảnh mới nếu có
            if (imageFile != null && imageFile.Length > 0)
            {
                var ext = Path.GetExtension(imageFile.FileName).ToLower();
                var allowedExtensions = new[] { ".png", ".jpeg", ".jpg" };

                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("Img", "Only image files (.png, .jpeg, .jpg) are allowed.");
                    return View(model);
                }

                var fileName = $"{Guid.NewGuid()}{ext}";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                model.Img = "/uploads/" + fileName;
            }

            // Cập nhật thông tin
            var store = await _storedetail.FindAsync(s => s.UserID == user.Id);
            if (store == null)
                return NotFound();

            store.Name = model.Name;
            store.ShortDescriptions = model.ShortDescriptions;
            store.LongDescriptions = model.LongDescriptions;
            store.Phone = model.Phone;
            store.Address = model.Address;
            store.ImageUrl = model.Img;
            store.Status = "Pending";
            store.IsActive = true;
            store.ModifiedDate = DateTime.Now;

            await _storedetail.UpdateAsync(store);

            // Gán lại dữ liệu mới nhất cho ViewModel trước khi trả về View
            model.Status = store.Status;
            model.IsActive = store.IsActive;
            model.ModifiedDate = store.ModifiedDate;

            ViewBag.SuccessMessage = "Store registration updated. Waiting for approval.";
            //return View(model); // 👈 Không redirect nữa
            return RedirectToAction("ViewStore");
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderStatuses()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { error = "You are not logged in!" });

                var store = await _storedetail.FindAsync(s => s.UserID == user.Id);
                if (store == null)
                    return Json(new { error = "Store not found!" });

                var products = await _product.ListAsync(p => p.StoreID == store.ID);
                if (!products.Any())
                    return Json(new { success = true, data = new List<object>() });

                var productIds = products.Select(p => p.ID).ToList();
                var productTypes = await _variantService.ListAsync(pt => productIds.Contains(pt.ProductID));
                if (!productTypes.Any())
                    return Json(new { success = true, data = new List<object>() });

                var productTypeIds = productTypes.Select(pt => pt.ID).ToList();
                var orderDetails = await _orderDetail.ListAsync(od => productTypeIds.Contains(od.ProductTypesID));
                if (!orderDetails.Any())
                    return Json(new { success = true, data = new List<object>() });

                var orderIds = orderDetails.Select(od => od.OrderID).Distinct().ToList();
                var orders = await _order.ListAsync(o => orderIds.Contains(o.ID));

                if (!orders.Any())
                    return Json(new { success = true, data = new List<object>() });

                var result = orders.Select(o => new
                {
                    orderId = o.OrderTracking,
                    status = o.Status
                }).ToList();

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "An error occurred: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetComplaintStatusByOrderDetailId()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { error = "You are not logged in!" });

                var getStore = await _storedetail.FindAsync(s => s.UserID == user.Id);
                if (getStore == null)
                    return Json(new { error = "Store not found!" });

                // Tìm các sản phẩm thuộc store của user
                var products = await _product.ListAsync(p => p.StoreID == getStore.ID);
                if (!products.Any())
                    return Json(new { error = "No products found for store!" });

                var productIds = products.Select(p => p.ID).ToList();

                var productTypes = await _variantService.ListAsync(pt => productIds.Contains(pt.ProductID));
                if (!productTypes.Any())
                    return Json(new { error = "No product types found!" });

                var productTypeIds = productTypes.Select(pt => pt.ID).ToList();

                // Tìm OrderDetail phù hợp với OrderDetailId và có ProductTypesID thuộc store
                var orderDetails = await _orderDetail.ListAsync(od => productTypeIds.Contains(od.ProductTypesID));

                if (orderDetails == null)
                    return Json(new { error = "Order detail not found or does not belong to your store!" });
                var orderDetailId = orderDetails.Select(pt => pt.ID).ToList();
                // Lấy complaint tương ứng với OrderDetail
                var complaint = await _complaintService.ListAsync(c => orderDetailId.Contains(c.OrderDetailID));

                if (complaint == null)
                    return Json(new { error = "Complaint not found for this order detail!" });

                var result = complaint.Select(o => new
                {
                    status = o.Status
                }).ToList();

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "An error occurred: " + ex.Message });
            }
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStore(Guid id, StoreViewModel model, IFormFile? ImgFile)
        {
            // if (!ModelState.IsValid)
            // {
            //     return View(model);
            // }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Lấy thông tin cửa hàng hiện tại để giữ nguyên ảnh cũ nếu không upload ảnh mới
            var existingStore = await _storedetail.GetStoreByIdAsync(id);
            if (existingStore == null)
            {
                ModelState.AddModelError("", "No store found.");
                return View(model);
            }

            string imgPath = existingStore.ImageUrl; // Giữ ảnh cũ nếu không có ảnh mới

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

                string uniqueFileName = $"{Guid.NewGuid()}{extension}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImgFile.CopyToAsync(fileStream);
                }

                imgPath = "/uploads/" + uniqueFileName; // Cập nhật đường dẫn ảnh mới
            }

            // Cập nhật cửa hàng
            var success = await _storedetail.UpdateStoreAsync(id, model.Name, model.LongDescriptions,
                                                                model.ShortDescriptions, model.Address,
                                                                model.Phone, imgPath);

            if (!success)
            {
                ModelState.AddModelError("", "Store update failed.");
                return View(model);
            }

            //ViewBag.UpdateSuccess = true;
            //return View(model); // Trả về lại trang UpdateStore để hiển thị thông báo
            return RedirectToAction("ViewStore");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdateStore(Guid id)
        {
            var store = await _storedetail.GetStoreByIdAsync(id);
            if (store == null)
            {
                return NotFound();
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
    }
}
