using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogic.Hash;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.ComplaintImages;
using BusinessLogic.Services.Complaints;
using BusinessLogic.Services.IngredientTagServices;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.RecipeServices;
using BusinessLogic.Services.StoreDetail;
using BusinessLogic.Services.StoreReports;
using BusinessLogic.Services.TypeOfDishServices;
using BusinessLogic.Services.VoucherServices;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Org.BouncyCastle.Asn1.X509;
using Repository.BalanceChange;
using Repository.OrdeDetails;
using Repository.StoreDetails;
using Repository.ViewModels;

namespace Food_Haven.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IBalanceChangeService _balance; // xử lý withdaw
        private readonly UserManager<AppUser> _userManager;
        private HttpClient client = null;
        private string url;
        private readonly IStoreDetailService _storeService;
        private readonly StoreDetailsRepository _storeRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICategoryService _categoryService;
        private readonly ManageTransaction _managetrans;
        private readonly ITypeOfDishService _typeOfDishService;
        private readonly IComplaintServices _complaintService;
        private readonly IOrderDetailService _orderDetail;
        private readonly IOrdersServices _order;
        private readonly IProductVariantService _variantService;
        private readonly IStoreDetailService _storedetail;
        private readonly IComplaintImageServices _compalntimg;
        private readonly IProductService _product;
        private readonly IVoucherServices _voucher;
        private readonly IIngredientTagService _ingredienttag;
        private readonly IRecipeService _recipeService;
        private readonly IStoreReportServices _storeReport;
        private readonly IProductImageService _productImageService;


        public AdminController(UserManager<AppUser> userManager, ITypeOfDishService typeOfDishService, IIngredientTagService ingredientTagService, IStoreDetailService storeService, 
            IMapper mapper, IWebHostEnvironment webHostEnvironment, StoreDetailsRepository storeRepository, IBalanceChangeService balance, 
            ICategoryService categoryService, ManageTransaction managetrans, IComplaintServices complaintService, IOrderDetailService orderDetail,
            IOrdersServices order, IProductVariantService variantService, IComplaintImageServices complaintImage, IStoreDetailService storeDetailService, 
            IProductService product, IVoucherServices voucher, IRecipeService recipeService, IStoreReportServices storeRepo, IStoreReportServices storeReport,
            IProductImageService productImageService)

        {
            _ingredienttag = ingredientTagService;
            _typeOfDishService = typeOfDishService;
            _userManager = userManager;
            _balance = balance;
            client = new HttpClient();
            var contentype = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentype);
            _storeService = storeService;
            _userManager = userManager;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _storeRepository = storeRepository;
            _categoryService = categoryService;
            _managetrans = managetrans;
            _complaintService = complaintService;
            _orderDetail = orderDetail;
            _order = order;
            _variantService = variantService;
            _storedetail = storeDetailService;
            _compalntimg = complaintImage;
            _product = product;
            _voucher = voucher;
            _recipeService = recipeService;
            _storeReport = storeReport;
            _productImageService = productImageService;
        }
      
        [HttpPost]
        public async Task<IActionResult> HiddenAccount([FromBody] UsersViewModel obj)
        {
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (!await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return RedirectToAction("Login", "Home");
            }
            try
            {
                if (obj == null || string.IsNullOrEmpty(obj.Email))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }
                var user = await _userManager.FindByEmailAsync(obj.Email);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }
                user.IsBannedByAdmin = true;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return Json(new { success = false, message = "Failed to update user" });
                }
                return Json(new { success = true, message = "User account Banned successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the user", error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> ShowAccount([FromBody] UsersViewModel obj)
        {
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null || !await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return RedirectToAction("Login", "Home");
            }
            try
            {
                if (obj == null || string.IsNullOrEmpty(obj.Email))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }
                var user = await _userManager.FindByEmailAsync(obj.Email);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }
                user.IsBannedByAdmin = false;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return Json(new { success = false, message = "Failed to update user" });
                }
                return Json(new { success = true, message = "User account unbanned successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the user", error = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> UpdateByAdmin(string email)
        {
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null || !await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return RedirectToAction("Login", "Home");
            }
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }
                var model = new AdminViewModel
                {
                    Email = user.Email,
                    Address = user.Address,
                    Birthday = user.Birthday,
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.UserName,
                };
                return View(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the user", error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateByAdmin([FromBody] UsersViewModel obj)
        {
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null || !await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return RedirectToAction("Login", "Home");
            }
            try
            {
                if (obj == null || string.IsNullOrEmpty(obj.Email))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }
                var user = await _userManager.FindByEmailAsync(obj.Email);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }
                user.Address = obj.Address;
                user.Birthday = obj.Birthday;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return Json(new { success = false, message = "Failed to update user" });

                }
                return Json(new { success = true, message = "User account updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the user", error = ex.Message });
            }

        }
        public async Task<IActionResult> ManagementSeller()
        {
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (!await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return RedirectToAction("Login", "Home");
            }
            try
            {
                var list = new List<UsersViewModel>();
                var obj = _userManager.Users.ToList();
                if (obj.Any())
                {
                    foreach (var user in obj)
                    {
                        // Kiểm tra nếu là Admin thì bỏ qua
                        if (_userManager.IsInRoleAsync(user, "Admin").Result)
                            continue;
                        list.Add(new UsersViewModel
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
                            IsBanByadmin = user.IsBannedByAdmin,
                        });
                    }
                }
                list = list
       .Where(x => x.RequestSeller == "1" || x.RequestSeller == "2" || x.RequestSeller == "3")  // Lọc tất cả user có RequestSeller là 1 hoặc 2
       .OrderByDescending(x => x.RequestSeller == "1" || x.RequestSeller == "3")  // Sắp xếp người đăng ký (RequestSeller == "1") lên trên
       .ToList();
                return View(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving users", error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AcceptSeller([FromBody] UsersViewModel obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.Email))
            {
                return Json(new { success = false, message = "Invalid request data" });
            }

            var admin = await _userManager.GetUserAsync(User);
            if (admin == null || !await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return StatusCode(401, new { success = false, message = "You must be an admin to perform this action" });
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(obj.Email);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                user.RequestSeller = "2";
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return Json(new { success = false, message = "Failed to update user" });
                }

                var roleResult = await _userManager.AddToRoleAsync(user, "Seller");
                if (!roleResult.Succeeded)
                {
                    return Json(new { success = false, message = "Failed to add role" });
                }

                return Json(new { success = true, message = "User approved as seller" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while updating the user", error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> RejectSeller([FromBody] UsersViewModel obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.Email))
            {
                return Json(new { success = false, message = "Invalid request data" });
            }
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null || !await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return StatusCode(401, new { success = false, message = "You must be an admin to perform this action" });
            }
            try
            {
                var user = await _userManager.FindByEmailAsync(obj.Email);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }
                user.RequestSeller = "3";
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return Json(new { success = false, message = "Failed to update user" });
                }
                return Json(new { success = true, message = "User rejected as seller" });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "An error occurred while updating the user", error = ex.Message });
            }
        }

        public async Task<IActionResult> WithdrawList()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var withdrawList = new List<WithdrawAdminListViewModel>();

            try
            {
                // Lấy danh sách giao dịch có Method = "Withdraw"
                var balances = await _balance.ListAsync(p => p.Method == "Withdraw");

                if (balances == null || !balances.Any())
                {
                    return View(withdrawList); // Trả về danh sách rỗng nếu không có dữ liệu
                }

                foreach (var balance in balances)
                {
                    var withdrawUser = await _userManager.FindByIdAsync(balance.UserID);

                    var withdrawModel = new WithdrawAdminListViewModel
                    {
                        ID = balance.ID,
                        MoneyChange = Math.Abs(balance.MoneyChange),
                        StartTime = balance.StartTime,
                        DueTime = balance.DueTime,
                        Description = balance.Description,
                        UserID = balance.UserID,
                        Status = balance.Status,
                        Method = balance.Method,
                        UserName = withdrawUser?.UserName
                    };

                    withdrawList.Add(withdrawModel);
                }

                // Sắp xếp: PROCESSING trước, sau đó theo ngày
                withdrawList = withdrawList.OrderBy(w => w.Status != "PROCESSING")
                                           .ThenBy(w => w.StartTime)
                                           .ToList();

                // Đánh số thứ tự
                for (int i = 0; i < withdrawList.Count; i++)
                {
                    withdrawList[i].No = i + 1;
                }

                return View(withdrawList);
            }
            catch (Exception ex)
            {
                // Có thể log lỗi ở đây nếu cần
                return View(withdrawList); // Trả về danh sách rỗng nếu có lỗi
            }
        }
        public async Task<IActionResult> WithdrawDetails(string id)
        {
            try
            {
                // Kiểm tra ID có hợp lệ không
                if (!Guid.TryParse(id, out Guid withdrawId))
                {
                    return Json(new ErroMess { success = false, msg = "Invalid ID." });
                }

                // Tìm thông tin rút tiền theo ID
                var withdraw = await _balance.FindAsync(w => w.ID == withdrawId);

                if (withdraw == null)
                {
                    return Json(new ErroMess { success = false, msg = "Withdrawal request not found." });
                }

                // Lấy thông tin người dùng
                var user = await _userManager.FindByIdAsync(withdraw.UserID);
                if (user == null)
                {
                    return Json(new ErroMess { success = false, msg = "User does not exist." });
                }

                // Tạo ViewModel để hiển thị trong View
                var withdrawModel = new WithdrawAdminListViewModel
                {
                    ID = withdraw.ID,
                    UserName = user.UserName,
                    MoneyChange = withdraw.MoneyChange,
                    StartTime = withdraw.StartTime,
                    DueTime = withdraw.DueTime,
                    Description = withdraw.Description,
                    Status = withdraw.Status,
                    Method = withdraw.Method,
                    UserID = withdraw.UserID
                };

                return View(withdrawModel);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để debug sau này
                Console.WriteLine($"Error: {ex.Message}");

                // Trả về lỗi JSON để tránh chết chương trình
                return Json(new ErroMess { success = false, msg = "An error occurred, please try again later." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AcceptWithdraw(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid guidId))
            {
                return Json(new { success = false, msg = "Invalid ID!" });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, msg = "You must be logged in to perform this action.!" });
            }

            try
            {
                var flag = await _balance.FindAsync(p => p.ID == guidId);
                if (flag == null)
                {
                    return Json(new { success = false, msg = "Withdrawal request not found!" });
                }

                var withdrawUser = await _userManager.FindByIdAsync(flag.UserID);
                if (withdrawUser == null)
                {
                    return Json(new { success = false, msg = "User not found!" });
                }

                // Cập nhật trạng thái
                flag.Status = "Success";
                // Có thể cập nhật thêm DueTime nếu cần
                // flag.DueTime = DateTime.Now;

                await _balance.UpdateAsync(flag);
                await _balance.SaveChangesAsync();

                return Json(new { success = true, msg = "Confirm withdrawal successful!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "System error: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectWithdraw(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out Guid guidId))
            {
                return Json(new { success = false, msg = "Invalid ID!" });
            }

            try
            {
                // Kiểm tra yêu cầu rút tiền
                var flag = await _balance.FindAsync(p => p.ID == guidId);
                if (flag == null)
                {
                    return Json(new { success = false, msg = "Withdrawal request not found!" });
                }

                // Lấy user hiện tại đang đăng nhập
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Json(new { success = false, msg = "You must be logged in to perform this action.!" });
                }

                // Lấy user người gửi yêu cầu rút tiền
                var withdrawUser = await _userManager.FindByIdAsync(flag.UserID);
                if (withdrawUser == null)
                {
                    return Json(new { success = false, msg = "User requesting withdrawal not found!" });
                }

                // Thực hiện trong transaction
                var transactionResult = await _managetrans.ExecuteInTransactionAsync(async () =>
                {
                    var currentBalance = await _balance.GetBalance(withdrawUser.Id);
                    var newBalance = currentBalance + Math.Abs(flag.MoneyChange);

                    flag.Status = "CANCELLED";
                    flag.MoneyBeforeChange = currentBalance;
                    flag.MoneyAfterChange = newBalance;
                    flag.Display = true;
                    flag.IsComplete = true;
                    flag.DueTime = DateTime.Now;
                    flag.CheckDone = true;

                    await _balance.UpdateAsync(flag);
                });

                if (!transactionResult)
                {
                    return Json(new { success = false, msg = "Withdrawal failed, transaction not completed!" });
                }

                await _balance.SaveChangesAsync();

                return Json(new { success = true, msg = "Confirm withdrawal rejection successful!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "System error: " + ex.Message });
            }
        }
        public async Task<IActionResult> ViewStoreRegistration()
        {
            var stores = await _storeService.GetStoreRegistrationRequestsAsync();

            if (stores == null || stores.Count == 0)
            {
                TempData["Message"] = "No inactive stores found.";
            }

            return View(stores);
        }

        [HttpPost]
        [Route("Admin/UpdateStoreStatus/{storeId}/{newStatus}")]
        public async Task<JsonResult> UpdateStoreStatus(Guid storeId, string newStatus)
        {
            bool isUpdated = await _storeService.UpdateStoreStatusAsync(storeId, newStatus);

            if (!isUpdated)
            {
                return Json(new { success = false, message = "Store not found" });
            }

            return Json(new { success = true, message = "Store status updated successfully", newStatus });
        }
        public async Task<IActionResult> ViewAdminStore()
        {
            var stores = await _storeService.GetInactiveStoresAsync();

            if (stores == null || stores.Count == 0)
            {
                TempData["Message"] = "No inactive stores found.";
            }

            return View(stores);
        }

        [HttpPost]
        [Route("Admin/UpdateStoreIsActive")]
        public async Task<JsonResult> UpdateStoreIsActive(Guid storeId, bool isActive)
        {
            bool isUpdated = await _storeService.UpdateStoreIsActiveAsync(storeId, isActive);

            if (!isUpdated)
            {
                return Json(new { success = false, message = "Store not found" });
            }

            return Json(new { success = true, message = "Store status updated successfully", isActive });
        }
        public async Task<IActionResult> ViewCategories()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            return View(categories);
        }
        [HttpGet]
        public IActionResult CheckNumberExists(int number)
        {
            bool exists = _categoryService.CheckNumberExists(number);
            return Json(new { exists });
        }


        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(CategoryCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                _categoryService.CreateCategory(model);

                // Gán thông báo thành công vào ViewBag
                ViewBag.SuccessMessage = "Category created successfully!";

                // Trả lại View để hiển thị SweetAlert rồi mới redirect bằng JS
                return View();
            }

            return View(model); // Nếu lỗi thì giữ nguyên form
        }
        [HttpGet("Admin/UpdateCategory/{id}")]
        public IActionResult UpdateCategory(Guid id)
        {
            var model = _categoryService.GetCategoryForUpdate(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateCategory(CategoryUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                _categoryService.UpdateCategory(model);

                // Nạp lại model sau khi cập nhật để lấy thông tin hình ảnh mới hoặc cũ
                var updatedModel = _categoryService.GetCategoryForUpdate(model.ID);
                ViewBag.SuccessMessage = "Category updated successfully!";
                return View(updatedModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public async Task<JsonResult> ToggleCategoryStatus(Guid categoryId, bool isActive)
        {
            bool success = await _categoryService.ToggleCategoryStatusAsync(categoryId, isActive);
            return Json(new { success });
        }
        public async Task<IActionResult> Managercomplant()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetComplaint()
        {
            var complaintsRaw = await _complaintService.ListAsync(c => c.IsReportAdmin);
            if (!complaintsRaw.Any())
                return Json(new List<GetComplaintViewModel>());

            var orderDetailIds = complaintsRaw.Select(c => c.OrderDetailID).Distinct().ToList();
            var orderDetails = await _orderDetail.ListAsync(od => orderDetailIds.Contains(od.ID));
            if (!orderDetails.Any())
                return Json(new List<GetComplaintViewModel>());
            var orderIds = orderDetails.Select(od => od.OrderID).Distinct().ToList();
            var orders = await _order.ListAsync(o => orderIds.Contains(o.ID));
            if (!orders.Any())
                return Json(new List<GetComplaintViewModel>());
            var userIds = orders.Select(o => o.UserID).Distinct().ToList();
            var users = await _userManager.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
            var userDict = users.ToDictionary(u => u.Id, u => u.UserName);

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
                    OrderCode = orderInfo?.OrderTracking ?? "N/A",
                    UserName = userName,
                    Description = c.Description,
                    Status = c.Status,
                    SellerReply = c.Reply,
                    AdminReply = c.AdminReply,
                    CreatedDate = c.CreatedDate,
                    ReplyDate = c.ReplyDate,
                    ReportStatus = c.AdminReportStatus,
                    AdminReplyDate = c.DateAdminReply
                };
            }).ToList();

            return Json(complaints);
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



            var getImage = await this._compalntimg.ListAsync(u => u.ComplaintID == getComplaint.ID);
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

            switch (action)
            {
                case "Accept":


                    complaint.AdminReportStatus = $"Accept";
                    complaint.DateAdminReply = DateTime.Now;
                    complaint.AdminReply = $"[Accept] - {note}";

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

                case "Reject":
                    complaint.AdminReportStatus = $"Reject";
                    complaint.DateAdminReply = DateTime.Now;
                    complaint.AdminReply = $"[Reject] - {note}";
                    mess = "Đơn hàng đã được hoàn tiền và hủy thành công.";
                    break;
                default:
                    return Json(new { success = false, message = "Loại hành động không hợp lệ." });
            }



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

        public async Task<IActionResult> GetAllTypeOfDish()
        {
            var data = await _typeOfDishService.ListAsync(); // ✅ LẤY DỮ LIỆU THẬT

            var list = data
                .OrderByDescending(item => item.CreatedDate) // 👈 Sắp xếp theo ngày tạo mới nhất
                .Select(item => new TypeOfDishViewModel
                {
                    ID = item.ID,
                    Name = item.Name,
                    IsActive = item.IsActive,
                    CreatedDate = item.CreatedDate,
                    ModifiedDate = item.ModifiedDate
                }).ToList();

            return View(list);
        }




        [HttpGet]
        public IActionResult CreateTypeOfDish()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTypeOfDish(TypeOfDishViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var entity = new TypeOfDish
            {
                ID = Guid.NewGuid(),
                Name = model.Name,
                IsActive = model.IsActive,
                CreatedDate = DateTime.Now // ✅ Ghi đúng thời điểm tạo
            };

            await _typeOfDishService.AddAsync(entity);
            await _typeOfDishService.SaveChangesAsync();

            return RedirectToAction("GetAllTypeOfDish");
        }


        [HttpGet]
        public async Task<IActionResult> UpdateTypeOfDish(Guid id)
        {
            var entity = await _typeOfDishService.GetAsyncById(id);
            if (entity == null) return NotFound();

            var viewModel = _mapper.Map<TypeOfDishUpdateViewModel>(entity);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTypeOfDish(TypeOfDishUpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var entity = await _typeOfDishService.GetAsyncById(model.ID);
                if (entity == null)
                    return NotFound();

                entity.Name = model.Name;
                entity.IsActive = model.IsActive;
                entity.ModifiedDate = DateTime.Now;

                await _typeOfDishService.UpdateAsync(entity);
                await _typeOfDishService.SaveChangesAsync();

                ViewBag.RedirectWithSuccess = true;
                return View(model); // Dùng lại view để trigger SweetAlert thành công
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }


        [HttpPost]
        public async Task<IActionResult> ToggletypeOfDishesIdTagStatus(Guid id, bool isActive)
        {
            var success = await _typeOfDishService.ToggletypeOfDishesIdTagStatus(id, isActive);
            return Json(new { success });
        }







        public async Task<IActionResult> GetAllIngredientTag()
        {
            var data = await _ingredienttag.ListAsync();

            var list = data
                .OrderByDescending(item => item.CreatedDate) // 👈 Sắp xếp theo ngày tạo mới nhất
                .Select(item => new IngredientTagViewModel
                {
                    ID = item.ID,
                    Name = item.Name,
                    IsActive = item.IsActive,
                    CreatedDate = item.CreatedDate,
                    ModifiedDate = item.ModifiedDate
                })
                .ToList();

            return View(list);
        }

        [HttpGet]
        public IActionResult CreateIngredientTag()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateIngredientTag(IngredientTagViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var entity = new IngredientTag
            {
                ID = Guid.NewGuid(),
                Name = model.Name,
                IsActive = model.IsActive,
                CreatedDate = DateTime.Now // ✅ Ghi đúng thời điểm tạo
            };

            await _ingredienttag.AddAsync(entity);
            await _ingredienttag.SaveChangesAsync();

            return RedirectToAction("GetAllIngredientTag");
        }




        [HttpGet]
        public async Task<IActionResult> UpdateIngredientTag(Guid id)
        {
            var entity = await _ingredienttag.GetAsyncById(id);
            if (entity == null) return NotFound();

            var viewModel = _mapper.Map<IngredientTagUpdateViewModel>(entity);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateIngredientTag(IngredientTagUpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var entity = await _ingredienttag.GetAsyncById(model.ID);
                if (entity == null)
                    return NotFound();

                entity.Name = model.Name;
                entity.IsActive = model.IsActive;
                entity.ModifiedDate = DateTime.Now;

                await _ingredienttag.UpdateAsync(entity);
                await _ingredienttag.SaveChangesAsync();

                ViewBag.RedirectWithSuccess = true;
                return View(model); // Không redirect, giữ lại để chạy SweetAlert trong view
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }





        [HttpPost]
        public async Task<IActionResult> ToggleIngredientTagStatus(Guid id, bool isActive)
        {
            var success = await _ingredienttag.ToggleIngredientTagStatus(id, isActive);
            return Json(new { success });
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
                var now = DateTime.Now;

                var data = _voucher.GetAll()
                    .Where(v => v.IsGlobal)
                    .Select(v => new
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
                    })
                    .ToList();

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
                errors["isPrivate"] = "isPrivate is required.";

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
                    MaxDiscountAmount = decimal.Parse(v.Scope),
                    MaxUsage = v.MaxUsage,
                    CurrentUsage = v.CurrentUsage,
                    MinOrderValue = v.MinOrderValue,
                    IsActive = v.IsActive,
                    CreatedDate = DateTime.Now,
                    IsGlobal = true
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
        [HttpGet]
        public async Task<IActionResult> ManagementRecipe()
        {
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null) return RedirectToAction("Login", "Home");
            else if (!await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return RedirectToAction("Login", "Home");
            }
            var recipe = await _recipeService.ListAsync();
            var list = new List<RecipeViewModels>();
            if (recipe.Any())
            {
                foreach (var item in recipe)
                {
                    // Lấy thông tin người dùng từ UserID
                    var user = await _userManager.FindByIdAsync(item.UserID);
                    var username = user != null ? user.UserName : "Unknown"; // Xử lý trường hợp user không tồn tại

                    var recipeViewModel = new RecipeViewModels
                    {
                        ID = item.ID,
                        Title = item.Title,
                        TotalTime = item.TotalTime,
                        ThumbnailImage = item.ThumbnailImage,
                        IsActive = item.IsActive,
                        status = item.status,
                        Username = username, // Gán Username từ Identity
                    };
                    list.Add(recipeViewModel);
                }
            }
            return View(list);
        }

        [HttpPost]
        [Route("Admin/ToggleRecipeVisibility/{id}/{visibility}")]
        public async Task<IActionResult> ToggleRecipeVisibility(Guid id, string visibility)
        {
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null || !await _userManager.IsInRoleAsync(admin, "Admin"))
                return Unauthorized();

            try
            {
                var recipe = await _recipeService.FindAsync(x => x.ID == id);
                if (recipe == null)
                    return NotFound();

                if (visibility.Equals("Hide", StringComparison.OrdinalIgnoreCase))
                {
                    recipe.IsActive = false;
                }
                else if (visibility.Equals("Show", StringComparison.OrdinalIgnoreCase))
                {
                    recipe.IsActive = true;
                }
                else
                {
                    return BadRequest(new { success = false, message = "Invalid visibility action." });
                }

                await _recipeService.UpdateAsync(recipe);
                await _recipeService.SaveChangesAsync();

                return Json(new { success = true, message = $"Recipe has been {(recipe.IsActive ? "shown" : "hidden")} successfully." });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while updating recipe visibility.",
                    error = ex.Message
                });
            }
        }

        [HttpPost("Admin/UpdateRecipeStatus/{id}/{status}")]
        public async Task<IActionResult> UpdateRecipeStatus(Guid id, string status)
        {
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null || !await _userManager.IsInRoleAsync(admin, "Admin"))
                return Unauthorized();

            try
            {
                var recipe = await _recipeService.FindAsync(x => x.ID == id);
                if (recipe == null)
                    return NotFound();

                // Cập nhật trạng thái
                if (status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                {
                    recipe.status = "Accept";
                }
                else if (status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                {
                    recipe.status = "Reject";
                }
                else
                {
                    return BadRequest(new { success = false, message = "Invalid status." });
                }

                await _recipeService.UpdateAsync(recipe);
                await _recipeService.SaveChangesAsync();

                return Json(new { success = true, message = $"Recipe {status.ToLower()} successfully." });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while updating the recipe status.",
                    error = ex.Message
                });
            }
        }
        public async Task<IActionResult> Chat()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ManagementReportStore()
        {
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null) return RedirectToAction("Login", "Home");
            else if (!await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return RedirectToAction("Login", "Home");
            }
            try
            {
                var list = new List<StoreReportViewModel>();
                var obj = await _storeReport.ListAsync();
                if (obj.Any())
                {
                    foreach (var item in obj)
                    {
                        var reportingUser = await _userManager.FindByIdAsync(item.UserID);
                        var store = await _storeService.GetAsyncById(item.StoreID);
                        var viewModel = new StoreReportViewModel
                        {
                            ID = item.ID,
                            StoreID = store.ID,
                            UserID = item.UserID,
                            Reason = item.Reason,
                            Message = item.Message,
                            CreatedDate = item.CreatedDate,
                            UserName = reportingUser?.UserName ?? "Ẩn danh",
                            Email = reportingUser?.Email ?? "Không rõ",
                            StoreName = store.Name,
                        };
                        list.Add(viewModel);
                    }
                }
                return View(list);

            }
            catch (System.Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> ManagerUser()
        {
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (!await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                return RedirectToAction("Login", "Home");
            }
            try
            {
                var list = new List<UsersViewModel>();
                var obj = _userManager.Users.ToList();
                if (obj.Any())
                {
                    foreach (var user in obj)
                    {
                        // Kiểm tra nếu là Admin thì bỏ qua
                        if (_userManager.IsInRoleAsync(user, "Admin").Result)
                            continue;
                        list.Add(new UsersViewModel
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
                            IsBanByadmin = user.IsBannedByAdmin,
                        });
                    }
                }
                return View(list);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving users", error = ex.Message });
            }
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
                return StatusCode(500, new { error = "Lỗi khi lấy cấu hình ngày", message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStatisticsByDate(DateTime? from = null, DateTime? to = null)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { error = "Bạn chưa đăng nhập!!" });

                var store = await _storedetail.FindAsync(s => s.UserID == user.Id);
                if (store == null)
                    return Json(new { error = "Không tìm thấy cửa hàng." });

                var products = await _product.ListAsync(p => p.StoreID == store.ID);
                if (!products.Any())
                    return Json(new { error = "Không có sản phẩm." });

                var productIds = products.Select(p => p.ID).ToList();
                var variants = await _variantService.ListAsync(v => productIds.Contains(v.ProductID));
                if (!variants.Any())
                    return Json(new { error = "Không có biến thể sản phẩm." });

                var variantIds = variants.Select(v => v.ID).ToList();
                var orderDetails = await _orderDetail.ListAsync(od => variantIds.Contains(od.ProductTypesID));
                if (!orderDetails.Any())
                    return Json(new { error = "Không có chi tiết đơn hàng." });

                var orderIds = orderDetails.Select(od => od.OrderID).Distinct().ToList();
                var orders = await _order.ListAsync(o => orderIds.Contains(o.ID));
                if (!orders.Any())
                    return Json(new { error = "Không có đơn hàng." });

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
                    return BadRequest(new { error = "Ngày bắt đầu không thể lớn hơn ngày kết thúc" });

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
                return StatusCode(500, new { error = "Lỗi server khi xử lý dữ liệu", message = ex.Message });
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
                return StatusCode(500, new { error = "Lỗi khi lấy cấu hình tháng", message = ex.Message });
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
                    return Json(new { error = "Bạn chưa đăng nhập!!" });
                var store = await _storedetail.FindAsync(s => s.UserID == user.Id);
                if (store == null)
                    return Json(new { error = "Không tìm thấy cửa hàng." });
                var products = await _product.ListAsync(p => p.StoreID == store.ID);
                if (!products.Any())
                    return Json(new { error = "Không có sản phẩm." });
                var productIds = products.Select(p => p.ID).ToList();
                var variants = await _variantService.ListAsync(v => productIds.Contains(v.ProductID));
                if (!variants.Any())
                    return Json(new { error = "Không có biến thể sản phẩm." });
                var variantIds = variants.Select(v => v.ID).ToList();
                var orderDetails = await _orderDetail.ListAsync(od => variantIds.Contains(od.ProductTypesID));
                if (!orderDetails.Any())
                    return Json(new { error = "Không có chi tiết đơn hàng." });
                var orderIds = orderDetails.Select(od => od.OrderID).Distinct().ToList();
                var orders = await _order.ListAsync(o => orderIds.Contains(o.ID));
                if (!orders.Any())
                    return Json(new { error = "Không có đơn hàng." });
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
                return StatusCode(500, new { error = "Lỗi server khi xử lý dữ liệu", message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(string period = "today", string search = "")
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { error = "Bạn chưa đăng nhập!" });
                var store = await _storedetail.FindAsync(u => u.UserID == user.Id);
                if (store == null)
                    return Json(new { error = "Không tìm thấy cửa hàng!" });
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
                return Json(new { error = "Bạn chưa đăng nhập!" });
            var getStore = await _storedetail.FindAsync(u => u.UserID == user.Id);
            if (getStore == null)
                return Json(new { error = "Không tìm thấy cửa hàng!" });
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
                    return Json(new { success = false, msg = "Bạn chưa đăng nhập!!" });

                var getStore = await _storedetail.FindAsync(u => u.UserID == user.Id);
                if (getStore == null)
                    return Json(new { success = false, msg = "Store not found" });

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
                return Json(new { success = false, msg = "Có lỗi xảy ra: " + ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> RecentOrders(string period = "today")
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { success = false, msg = "Bạn chưa đăng nhập!!" });
                var getStore = await _storedetail.FindAsync(u => u.UserID == user.Id);
                if (getStore == null)
                    return Json(new { success = false, msg = "Store not found" });
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
                return Json(new { success = false, msg = "Có lỗi xảy ra: " + ex.Message });
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

    }


}

