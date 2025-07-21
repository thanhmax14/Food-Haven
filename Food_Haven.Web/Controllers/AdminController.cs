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
using BusinessLogic.Services.RecipeIngredientTagIngredientTagServices;
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
using X.PagedList;

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
        private readonly IRecipeIngredientTagIngredientTagSerivce _recipeIngredientTagIngredientTagIngredientTagSerivce;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<AppUser> userManager, ITypeOfDishService typeOfDishService, IIngredientTagService ingredientTagService, IStoreDetailService storeService,
            IMapper mapper, IWebHostEnvironment webHostEnvironment, StoreDetailsRepository storeRepository, IBalanceChangeService balance,
            ICategoryService categoryService, ManageTransaction managetrans, IComplaintServices complaintService, IOrderDetailService orderDetail,
            IOrdersServices order, IProductVariantService variantService, IComplaintImageServices complaintImage, IStoreDetailService storeDetailService,
            IProductService product, IVoucherServices voucher, IRecipeService recipeService, IStoreReportServices storeRepo, IStoreReportServices storeReport,
            IProductImageService productImageService, IRecipeIngredientTagIngredientTagSerivce recipeIngredientTagIngredientTagIngredientTagSerivce, RoleManager<IdentityRole> roleManager)

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
            _recipeIngredientTagIngredientTagIngredientTagSerivce = recipeIngredientTagIngredientTagIngredientTagSerivce;
            _roleManager = roleManager;
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
        [HttpGet]
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
                            UserNameAdmin = admin.UserName,
                        });
                    }
                }
                list = list
       .Where(x => x.RequestSeller == "1" || x.RequestSeller == "2" || x.RequestSeller == "3")  // Lọc tất cả user có RequestSeller là 1 hoặc 2
       .OrderByDescending(x => x.RequestSeller == "1" || x.RequestSeller == "3")  // Sắp xếp người đăng ký (RequestSeller == "1") lên trên
       .ToList();
                ViewBag.AdminUserName = admin.UserName;

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

                // 🔹 Thêm phần lấy RoleId (KHÔNG thay đổi logic gốc)
                var sellerRole = await _roleManager.FindByNameAsync("Seller");
                var roleId = sellerRole?.Id ?? "unknown";

                return Json(new { success = true, message = "User approved as seller", roleId });
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
                user.RejectNote = obj.RejectNote;
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

            switch (action)
            {
                case "Accept":

                    complaint.AdminReportStatus = $"Accept";
                    complaint.DateAdminReply = DateTime.Now;
                    complaint.AdminReply = $"[Accept] - {note}";

                    try
                    {
                        // Get order detail list
                        var orderDetails = await _orderDetail.FindAsync(d => d.ID == complaint.OrderDetailID);
                        if (orderDetails == null)
                            return Json(new { success = false, message = "There are no products in this order." });
                        var order = await this._order.FindAsync(u => u.ID == orderDetails.OrderID);
                        if (order == null)
                            return Json(new { success = false, message = "Order not found." });

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

                        // Update order status
                        order.Status = "Refunded";
                        order.PaymentStatus = "Refunded";
                        order.ModifiedDate = DateTime.UtcNow;
                        order.Description = string.IsNullOrEmpty(order.Description)
                            ? $"Refunded - {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
                            : $"{order.Description}#Refunded - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                        order.IsPaid = true;
                        await _order.UpdateAsync(order);

                        // Save changes
                        await _orderDetail.SaveChangesAsync();

                        //   await _order.SaveChangesAsync();
                        await _balance.SaveChangesAsync();

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

                case "Reject":
                    complaint.AdminReportStatus = $"Reject";
                    complaint.DateAdminReply = DateTime.Now;
                    complaint.AdminReply = $"[Reject] - {note}";
                    mess = "The order has been refunded and cancelled successfully.";
                    break;
                default:
                    return Json(new { success = false, message = "Invalid action type." });
            }

            try
            {
                await this._complaintService.UpdateAsync(complaint);
                await this._complaintService.SaveChangesAsync();

                return Json(new { success = true, message = mess });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error saving data: " + ex.Message });
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

            // ❗ Kiểm tra tên trùng
            if (await _typeOfDishService.ExistsAsync(model.Name))
            {
                TempData["SwalError"] = "The dish type name already exists.";
                return View(model);
            }

            var entity = new TypeOfDish
            {
                ID = Guid.NewGuid(),
                Name = model.Name.Trim(),
                IsActive = model.IsActive,
                CreatedDate = DateTime.Now
            };

            await _typeOfDishService.AddAsync(entity);
            await _typeOfDishService.SaveChangesAsync();

            TempData["SuccessMessage"] = "Dish type has been created successfully!";
            return RedirectToAction("GetAllTypeOfDish"); // quay lại form trống
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
                // 🔍 Check duplicate name excluding current ID
                if (await _typeOfDishService.ExistsAsync(model.Name, model.ID))
                {
                    TempData["SwalError"] = "The dish type name already exists.";
                    return View(model);
                }

                var entity = await _typeOfDishService.GetAsyncById(model.ID);
                if (entity == null)
                    return NotFound();

                entity.Name = model.Name;
                entity.IsActive = model.IsActive;
                entity.ModifiedDate = DateTime.Now;

                await _typeOfDishService.UpdateAsync(entity);
                await _typeOfDishService.SaveChangesAsync();

                TempData["SuccessMessage"] = "Dish type has been updated successfully!";
                return RedirectToAction("GetAllTypeOfDish", new { id = model.ID }); // reload form sau khi update
            }
            catch (Exception ex)
            {
                TempData["SwalError"] = ex.Message;
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

            var exists = await _ingredienttag.ExistsAsync(model.Name);

            if (await _ingredienttag.ExistsAsync(model.Name))
            {
                TempData["SwalError"] = "The ingredient tag name already exists.";
                return View(model);
            }


            var entity = new IngredientTag
            {
                ID = Guid.NewGuid(),
                Name = model.Name.Trim(),
                IsActive = model.IsActive,
                CreatedDate = DateTime.Now
            };

            await _ingredienttag.AddAsync(entity);
            await _ingredienttag.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ingredient tag has been created successfully!";
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
                // ✅ Kiểm tra tên trùng (ngoại trừ chính nó)
                if (await _ingredienttag.ExistsAsync(model.Name, model.ID))
                {
                    TempData["SwalError"] = "The ingredient tag name already exists.";
                    return View(model);
                }

                var entity = await _ingredienttag.GetAsyncById(model.ID);
                if (entity == null)
                    return NotFound();

                entity.Name = model.Name;
                entity.IsActive = model.IsActive;
                entity.ModifiedDate = DateTime.Now;

                await _ingredienttag.UpdateAsync(entity);
                await _ingredienttag.SaveChangesAsync();

                TempData["SuccessMessage"] = "Ingredient tag has been updated successfully!";
                return RedirectToAction("GetAllIngredientTag");
            }
            catch (Exception ex)
            {
                TempData["SwalError"] = ex.Message;
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
                        ModifiedDate = item.ModifiedDate,
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
        public async Task<IActionResult> UpdateRecipeStatus(Guid id, string status, string rejectNote)
        {
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null || !await _userManager.IsInRoleAsync(admin, "Admin"))
                return Unauthorized();

            try
            {
                var recipe = await _recipeService.FindAsync(x => x.ID == id);
                if (recipe == null)
                    return NotFound();

                if (status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                {
                    recipe.status = "Accept";
                }
                else if (status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                {
                    recipe.status = "Reject";
                    recipe.RejectNote = rejectNote;
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
                return View("~/Views/Admin/ManagerUser.cshtml", list);
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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetDateConfig()
        {
            try
            {
                var today = DateTime.Now.Date;
                var todayStr = today.ToString("yyyy-MM-dd");
                var fallbackConfig = new
                {
                    minDate = todayStr,
                    maxDate = todayStr,
                    defaultDays = 1
                };

                var orders = await _order.ListAsync(o => true);

                var validStatuses = new[]
                {
            "CONFIRMED",
            "DELIVERING",
            "PREPARING IN KITCHEN",
            "PENDING"
        };

                var validOrders = orders
                    .Where(o => validStatuses.Contains(o.Status.ToUpper()))
                    .ToList();

                // Lấy ngày đầu tiên có đơn hàng hợp lệ
                DateTime? minOrderDate = validOrders.Any()
                    ? validOrders.Min(o => o.CreatedDate).Date
                    : (DateTime?)null;

                DateTime? maxOrderDate = validOrders.Any()
                    ? validOrders.Max(o => o.CreatedDate).Date
                    : (DateTime?)null;

                // Lấy ngày đầu tiên user đăng ký
                var allUsers = await _userManager.Users.ToListAsync();
                var joinedDates = allUsers
                    .Where(u => u.JoinedDate.HasValue)
                    .Select(u => u.JoinedDate.Value.Date)
                    .ToList();

                DateTime? minUserDate = joinedDates.Any()
                    ? joinedDates.Min()
                    : (DateTime?)null;

                // Lấy ngày nhỏ nhất giữa ngày đầu tiên có order và ngày user
                DateTime? minDateValue = null;
                if (minOrderDate.HasValue && minUserDate.HasValue)
                    minDateValue = minOrderDate < minUserDate ? minOrderDate : minUserDate;
                else if (minOrderDate.HasValue)
                    minDateValue = minOrderDate;
                else if (minUserDate.HasValue)
                    minDateValue = minUserDate;

                // Ngày lớn nhất vẫn là ngày cuối cùng có order hợp lệ
                DateTime? maxDateValue = maxOrderDate ?? today;

                // Nếu không có order và cũng không có user thì fallback
                if (!minDateValue.HasValue || !maxDateValue.HasValue)
                    return Json(fallbackConfig);

                int totalDays = (maxDateValue.Value - minDateValue.Value).Days + 1;

                return Json(new
                {
                    minDate = minDateValue.Value.ToString("yyyy-MM-dd"),
                    maxDate = maxDateValue.Value.ToString("yyyy-MM-dd"),
                    defaultDays = totalDays < 30 ? totalDays : 30
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error retrieving date configuration", message = ex.Message });

            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetStatisticsByDate(DateTime? from = null, DateTime? to = null)
        {
            try
            {
                var pendingStoreOpen = (await _storedetail.ListAsync(s => s.Status.ToUpper() == "REJECTED")).Count();
                var userList = await _userManager.Users.ToListAsync();
                var pendingSellerRequest = userList.Count(u => u.RequestSeller == "1" || u.RequestSeller == "3");
                var allWithdraw = await _balance.ListAsync(b =>
                    b.Method.ToUpper() == "WITHDRAW" &&
                    b.Status.ToUpper() == "PROCESSING"
                );
                var pendingWithdrawal = allWithdraw.Count();
                var complaintList = await _complaintService.ListAsync(c =>
                    c.Status == "Report to Admin"
                    && c.IsReportAdmin == true
                    && c.AdminReportStatus == "Pending");
                var totalcomplant = complaintList.Count();
                var totalStore = (await _storedetail.ListAsync(s => s.Status.ToUpper() == "APPROVED")).Count();
                var totalCustomer = userList.Count();
                var totalRecipe = (await _recipeService.ListAsync(r => r.status.ToUpper() == "ACCEPT")).Count();
                var pendingPublicRecipe = (await _recipeService.ListAsync(r => r.status.ToUpper() == "PENDING")).Count();
                var orders = await _order.ListAsync(o => true);
                var validStatuses = new[] { "CONFIRMED", "DELIVERING", "PREPARING IN KITCHEN", "PENDING" };
                var validOrders = orders.Where(o => validStatuses.Contains(o.Status.ToUpper())).ToList();
                DateTime? minOrderDate = validOrders.Any()
                    ? validOrders.Min(o => o.CreatedDate).Date
                    : (DateTime?)null;
                var depositChanges = await _balance.ListAsync(bc =>
                    bc.Method.ToUpper() == "DEPOSIT" &&
                    bc.Status.ToUpper() == "SUCCESS" &&
                    bc.StartTime.HasValue);
                DateTime? minDepositDate = depositChanges.Any()
                    ? depositChanges.Min(bc => bc.StartTime.Value.Date)
                    : (DateTime?)null;

                DateTime? minStartDate = null;
                if (minOrderDate.HasValue && minDepositDate.HasValue)
                    minStartDate = minOrderDate < minDepositDate ? minOrderDate : minDepositDate;
                else if (minOrderDate.HasValue)
                    minStartDate = minOrderDate;
                else if (minDepositDate.HasValue)
                    minStartDate = minDepositDate;

                if (!minStartDate.HasValue)
                    return Json(new { error = "No orders or deposits have been made yet." });


                var minDate = minStartDate.Value;

                DateTime? maxOrderDate = validOrders.Any() ? validOrders.Max(o => o.CreatedDate).Date : (DateTime?)null;
                DateTime? maxDepositDate = depositChanges.Any() ? depositChanges.Max(bc => bc.StartTime.Value.Date) : (DateTime?)null;
                DateTime maxDate;
                if (maxOrderDate.HasValue && maxDepositDate.HasValue)
                    maxDate = maxOrderDate > maxDepositDate ? maxOrderDate.Value : maxDepositDate.Value;
                else if (maxOrderDate.HasValue)
                    maxDate = maxOrderDate.Value;
                else
                    maxDate = maxDepositDate.Value;

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

                var ordersInRange = validOrders
                    .Where(o => o.CreatedDate.Date >= fromDate && o.CreatedDate.Date <= toDate)
                    .ToList();

                var grossSales = ordersInRange.Sum(o => o.TotalPrice);

                var depositInRange = depositChanges
                    .Where(bc => bc.StartTime.Value.Date >= fromDate && bc.StartTime.Value.Date <= toDate)
                    .ToList();
                decimal totalDeposit = depositInRange.Sum(bc => bc.MoneyChange);

                var orderIds = ordersInRange.Select(o => o.ID).ToList();
                var orderDetails = await _orderDetail.ListAsync(od => orderIds.Contains(od.OrderID));
                decimal commissionRevenue = orderDetails
                    .Where(od => od.CommissionPercent >= 0 && od.CommissionPercent <= 100)
                    .Sum(od => od.ProductPrice * od.Quantity * ((decimal)od.CommissionPercent / 100));
                commissionRevenue = Math.Round(commissionRevenue, 2);

                int newcustomer = userList.Count(u =>
                    u.JoinedDate.HasValue &&
                    u.JoinedDate.Value.Date >= minStartDate.Value &&
                    u.JoinedDate.Value.Date >= fromDate &&
                    u.JoinedDate.Value.Date <= toDate);

                var result = new
                {
                    grossSales,
                    commissionRevenue,
                    totalDeposit,
                    totalCustomer,
                    totalStore,
                    totalRecipe,
                    pendingSellerRequest,
                    pendingStoreOpen,
                    pendingPublicRecipe,
                    pendingWithdrawal,
                    newcustomer,
                    totalcomplant,
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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetValidMonths()
        {
            var today = DateTime.Now.Date;
            var todayStr = today.ToString("yyyy-MM");
            var fallbackMonths = new List<string> { todayStr };

            try
            {
                // Lấy toàn bộ order hợp lệ
                var orders = await _order.ListAsync(o => true);

                // Lấy toàn bộ nạp tiền thành công
                var deposits = await _balance.ListAsync(b =>
                    b.Method.ToUpper() == "DEPOSIT"
                    && b.Status.ToUpper() == "SUCCESS"
                    && b.StartTime.HasValue);

                // Lấy toàn bộ rút tiền thành công (không reject)
                var withdrawals = await _balance.ListAsync(b =>
                    b.Method.ToUpper() == "WITHDRAW"
                    && b.Status.ToUpper() == "SUCCESS"
                    && b.StartTime.HasValue);

                // Lấy các tháng từ order
                var orderMonths = orders
                    .Select(o => o.CreatedDate.ToString("yyyy-MM"));

                // Lấy các tháng từ nạp tiền
                var depositMonths = deposits
                    .Select(b => b.StartTime.Value.ToString("yyyy-MM"));

                // Lấy các tháng từ rút tiền
                var withdrawalMonths = withdrawals
                    .Select(b => b.StartTime.Value.ToString("yyyy-MM"));

                // Gộp tất cả và loại trùng, sắp xếp tăng dần
                var allMonths = orderMonths
                    .Concat(depositMonths)
                    .Concat(withdrawalMonths)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                if (allMonths.Any())
                    return Json(allMonths);
                else
                    return Json(fallbackMonths);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error retrieving month configuration", message = ex.Message });

            }
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetMonthlyData(string month)
        {
            if (string.IsNullOrEmpty(month))
                month = DateTime.Now.ToString("yyyy-MM");

            try
            {
                var validStatuses = new[] { "CONFIRMED", "DELIVERING", "PREPARING IN KITCHEN", "PENDING" };
                var orders = await _order.ListAsync(o => validStatuses.Contains(o.Status.ToUpper()));

                int year = int.Parse(month.Split('-')[0]);
                int m = int.Parse(month.Split('-')[1]);
                int daysInMonth = DateTime.DaysInMonth(year, m);
                DateTime now = DateTime.Now.Date;

                var allOrderIds = orders.Select(o => o.ID).ToList();
                var allOrderDetails = await _orderDetail.ListAsync(od => allOrderIds.Contains(od.OrderID));

                var allDeposits = await _balance.ListAsync(b =>
                    b.Method.ToUpper() == "DEPOSIT" &&
                    b.Status.ToUpper() == "SUCCESS" &&
                    b.StartTime.HasValue);

                var allWithdrawals = await _balance.ListAsync(b =>
                    b.Method.ToUpper() == "WITHDRAW" &&
                    b.Status.ToUpper() == "SUCCESS" &&
                    b.StartTime.HasValue);

                var chartGrossSales = new Dictionary<string, decimal>();
                var chartCommissionRevenue = new Dictionary<string, decimal>();
                var chartDeposits = new Dictionary<string, decimal>();
                var chartWithdrawals = new Dictionary<string, decimal>();

                decimal totalGrossSales = 0;
                decimal totalCommissionRevenue = 0;
                decimal totalDeposits = 0;
                decimal totalWithdrawals = 0;

                for (int day = 1; day <= daysInMonth; day++)
                {
                    var date = new DateTime(year, m, day);
                    if (date > now) break;
                    string dateKey = date.ToString("yyyy-MM-dd");

                    // 1. Lấy các order hợp lệ trong ngày
                    var dayOrders = orders.Where(o => o.CreatedDate.Date == date).ToList();
                    var dayOrderIds = dayOrders.Select(o => o.ID).ToList();
                    var dayOrderDetails = allOrderDetails.Where(od => dayOrderIds.Contains(od.OrderID)).ToList();

                    // Tổng tiền gross (chưa trừ hoa hồng)
                    decimal gross = dayOrders.Sum(o => o.TotalPrice);
                    chartGrossSales[dateKey] = gross;
                    totalGrossSales += gross;

                    // Tổng tiền hoa hồng admin nhận được trong ngày (commission revenue)
                    decimal commission = 0;
                    foreach (var od in dayOrderDetails)
                    {
                        if (od.CommissionPercent >= 0 && od.CommissionPercent <= 100)
                        {
                            commission += od.ProductPrice * od.Quantity * ((decimal)od.CommissionPercent / 100);
                        }
                    }
                    chartCommissionRevenue[dateKey] = commission;
                    totalCommissionRevenue += commission;

                    // Tiền nạp thành công trong ngày
                    decimal deposits = allDeposits
                        .Where(b => b.StartTime.Value.Date == date)
                        .Sum(b => Math.Abs(b.MoneyChange));
                    chartDeposits[dateKey] = deposits;
                    totalDeposits += deposits;

                    // Tiền rút thành công trong ngày
                    decimal withdrawals = allWithdrawals
                        .Where(b => b.StartTime.Value.Date == date)
                        .Sum(b => Math.Abs(b.MoneyChange));
                    chartWithdrawals[dateKey] = withdrawals;
                    totalWithdrawals += withdrawals;
                }

                var summary = new
                {
                    grosssales = totalGrossSales,
                    commissonrevenue = totalCommissionRevenue,
                    totalsdeposit = totalDeposits,
                    totalswithdrawal = totalWithdrawals
                };
                var chartData = new
                {
                    grosssales = chartGrossSales,
                    commissonrevenue = chartCommissionRevenue,
                    totalsdeposit = chartDeposits,
                    totalswithdrawal = chartWithdrawals
                };

                return Json(new { summary, chartData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Server error while processing data", message = ex.Message });

            }
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTopStores(string period = "today", string search = "")
        {
            try
            {
                var validStatuses = new[]
             {
            "CONFIRMED",
            "DELIVERING",
            "PREPARING IN KITCHEN",
            "PENDING"
        };
                // Lấy toàn bộ stores
                var allStores = await _storedetail.ListAsync(s => true);
                var allProducts = await _product.ListAsync(p => true);
                var allProductTypes = await _variantService.ListAsync(pt => true);
                var allOrderDetails = await _orderDetail.ListAsync(od => true);
                var allOrders = await _order.ListAsync(o => validStatuses.Contains(o.Status.ToUpper()));

                // Gộp dữ liệu theo store
                var storeProducts = allProducts.GroupBy(p => p.StoreID)
                    .ToDictionary(g => g.Key, g => g.Select(p => p.ID).ToList());
                var productTypesDict = allProductTypes.GroupBy(pt => pt.ProductID)
                    .ToDictionary(g => g.Key, g => g.Select(pt => pt.ID).ToList());
                var productTypeStoreMap = allProductTypes.ToDictionary(pt => pt.ID, pt => pt.ProductID);

                // Áp dụng filter thời gian lên orderDetails
                var now = DateTime.Now;
                IEnumerable<OrderDetail> filteredOrderDetails = allOrderDetails
                    .Where(od => allOrders.Any(o => o.ID == od.OrderID));

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
                    default:
                        // Không filter thêm
                        break;
                }

                // Gom nhóm theo StoreID
                var orderDetailsByStore = filteredOrderDetails
                    .Where(od => productTypeStoreMap.ContainsKey(od.ProductTypesID))
                    .GroupBy(od =>
                    {
                        var productId = productTypeStoreMap[od.ProductTypesID];
                        var product = allProducts.FirstOrDefault(p => p.ID == productId);
                        return product?.StoreID ?? Guid.Empty;
                    })
                    .Where(g => g.Key != Guid.Empty)
                    .ToList();

                var storeData = orderDetailsByStore.Select(g =>
                {
                    var storeId = g.Key;
                    var store = allStores.FirstOrDefault(s => s.ID == storeId);
                    var productsOfStore = storeProducts.ContainsKey(storeId) ? storeProducts[storeId] : new List<Guid>();
                    var orderCount = g.Select(od => od.OrderID).Distinct().Count();
                    var grosssales = g.Sum(od => od.ProductPrice * od.Quantity);
                    var commissionrevenue = g
                        .Where(od => od.CommissionPercent >= 0 && od.CommissionPercent <= 100)
                        .Sum(od => od.ProductPrice * od.Quantity * ((decimal)od.CommissionPercent / 100));

                    return new
                    {
                        storeid = store?.ID ?? Guid.Empty,
                        storename = store?.Name ?? "Unknown",
                        storejoindate = store?.CreatedDate.ToString("dd/MM/yyyy") ?? "",
                        productcount = productsOfStore.Count,
                        ordercount = orderCount,
                        grosssales = Math.Round(grosssales, 2),
                        commissionrevenue = Math.Round(commissionrevenue, 2)
                    };
                })
                .Where(x => x.storeid != Guid.Empty)
                .OrderByDescending(x => x.grosssales)
                .ToList();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    storeData = storeData
                        .Where(s => s.storename.ToLower().Contains(search.ToLower()))
                        .ToList();
                }

                return Json(storeData);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTopCategories(string period = "today", string search = "")
        {
            var validStatuses = new[] { "CONFIRMED", "DELIVERING", "PREPARING IN KITCHEN", "PENDING" };

            // Lấy toàn bộ sản phẩm, product type, order detail
            var products = await _product.ListAsync(p => true);
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

            // Lấy toàn bộ order hợp lệ (status đúng)
            var orderIds = orderDetails.Select(od => od.OrderID).Distinct().ToList();
            var orders = await _order.ListAsync(o => orderIds.Contains(o.ID) && validStatuses.Contains(o.Status.ToUpper()));
            if (!orders.Any())
                return Json(new List<object>());

            // Lọc theo period
            var now = DateTime.Now;
            IEnumerable<Order> filteredOrders = orders;
            switch (period?.ToLower())
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
                default:
                    // Không lọc, lấy tất cả
                    break;
            }
            var filteredOrderIds = filteredOrders.Select(o => o.ID).ToHashSet();
            var filteredOrderDetails = orderDetails.Where(od => filteredOrderIds.Contains(od.OrderID)).ToList();

            // Map ProductTypesID -> ProductID
            var productTypeToProduct = productTypes.ToDictionary(pt => pt.ID, pt => pt.ProductID);
            // Map ProductID -> CategoryID
            var productToCategory = products.ToDictionary(p => p.ID, p => p.CategoryID);

            // Lấy tất cả category toàn hệ thống
            var categoryIds = products.Select(p => p.CategoryID).Distinct().ToList();
            var categories = await _categoryService.ListAsync(c => categoryIds.Contains(c.ID));

            // Gom nhóm và tính grossSales, commission, orderCount
            var grouped = filteredOrderDetails
                .Where(od => productTypeToProduct.ContainsKey(od.ProductTypesID)
                          && productToCategory.ContainsKey(productTypeToProduct[od.ProductTypesID]))
                .GroupBy(od => productToCategory[productTypeToProduct[od.ProductTypesID]])
                .Select(g =>
                {
                    var cat = categories.FirstOrDefault(c => c.ID == g.Key);
                    decimal grossSales = g.Sum(od => od.Quantity * od.ProductPrice);
                    decimal commissionRevenue = g.Sum(od =>
                        (od.CommissionPercent ?? 0) >= 0 && (od.CommissionPercent ?? 0) <= 100
                            ? od.Quantity * od.ProductPrice * ((decimal)(od.CommissionPercent ?? 0) / 100m)
                            : 0
                    );
                    int orderCount = g.Select(od => od.OrderID).Distinct().Count();

                    return new
                    {
                        id = cat?.ID ?? Guid.Empty,
                        name = cat?.Name ?? "Unknown",
                        commitson = cat?.Commission ?? 0,
                        createdDate = cat?.CreatedDate?.ToString("dd/MM/yyyy") ?? "",
                        grossSales = Math.Round(grossSales, 2),
                        commissionRevenue = Math.Round(commissionRevenue, 2),
                        orderCount = orderCount
                    };
                })
                .Where(x => x.id != Guid.Empty)
                .OrderByDescending(x => x.grossSales)
                .ToList();

            // Tính tổng doanh thu để lấy phần trăm growth từng category
            decimal allGrossSales = grouped.Sum(x => x.grossSales);

            // Tính growth từng category (làm tròn từng mục, rồi bù cho mục lớn nhất)
            var growths = grouped
                .Select(x => allGrossSales == 0 ? 0 : Math.Round(100m * x.grossSales / allGrossSales, 2))
                .ToList();

            decimal sumGrowth = growths.Sum();
            decimal diff = 100m - sumGrowth;
            if (growths.Count > 0)
            {
                // Tìm index mục có grossSales lớn nhất để bù diff
                int maxIdx = 0;
                decimal maxGross = grouped[0].grossSales;
                for (int i = 1; i < grouped.Count; i++)
                {
                    if (grouped[i].grossSales > maxGross)
                    {
                        maxGross = grouped[i].grossSales;
                        maxIdx = i;
                    }
                }
                // Bù và đảm bảo không âm
                growths[maxIdx] += diff;
                if (growths[maxIdx] < 0) growths[maxIdx] = 0;
            }

            var categoryStats = grouped
                .Select((x, idx) => new
                {
                    id = x.id,
                    name = x.name,
                    commitson = x.commitson,
                    createdDate = x.createdDate,
                    grossSales = x.grossSales,
                    commissionRevenue = x.commissionRevenue,
                    orderCount = x.orderCount,
                    growth = growths[idx]
                })
                .OrderByDescending(x => x.grossSales)
                .ToList();

            if (!string.IsNullOrEmpty(search))
            {
                categoryStats = categoryStats
                    .Where(c => c.name.ToLower().Contains(search.ToLower()))
                    .ToList();
            }

            return Json(categoryStats);
        }
        [HttpGet]
        public async Task<IActionResult> ShopComplaintRates(string period = "alltime")
        {
            period = "alltime";
            try
            {
                DateTime startDate = GetStartDateByPeriod(period);
                DateTime endDate = DateTime.Now;

                // Lấy tất cả cửa hàng
                var shops = await _storedetail.ListAsync();

                // Lấy tất cả sản phẩm
                var allProducts = await _product.ListAsync();

                // Lấy tất cả variant
                var allProductTypes = await _variantService.ListAsync();

                // Lấy tất cả OrderDetail (có thể lọc theo status nếu muốn, ở đây lấy tất cả)
                var allOrderDetails = await _orderDetail.ListAsync();

                // Lấy khiếu nại trong period
                var complaints = await _complaintService.ListAsync(c =>
                    c.CreatedDate >= startDate && c.CreatedDate <= endDate);

                var shopStats = shops.Select(shop =>
                {
                    var productIds = allProducts.Where(p => p.StoreID == shop.ID).Select(p => p.ID).ToList();
                    var productTypeIds = allProductTypes.Where(pt => productIds.Contains(pt.ProductID)).Select(pt => pt.ID).ToList();

                    // Tổng đơn của shop (không lọc theo period, mà là tổng số OrderDetail của shop)
                    var odOfShop = allOrderDetails.Where(od => productTypeIds.Contains(od.ProductTypesID)).ToList();
                    int totalOrders = odOfShop.Count;

                    // Số khiếu nại của shop (trong period)
                    var odIdsOfShop = odOfShop.Select(od => od.ID).ToList();
                    var complaintsOfShop = complaints.Where(c => odIdsOfShop.Contains(c.OrderDetailID)).ToList();
                    int totalComplaints = complaintsOfShop.Count;

                    double complaintRate = (totalOrders > 0) ? Math.Round(totalComplaints * 100.0 / totalOrders, 1) : 0;

                    return new
                    {
                        shopName = shop.Name,
                        totalOrders,
                        totalComplaints,
                        complaintRate
                    };
                })
                .Where(x => x.totalOrders > 0)
                .OrderByDescending(x => x.complaintRate)
                .ThenByDescending(x => x.totalComplaints)
                .ToList();

                // Đẩy ra format yêu cầu (series: số khiếu nại, percentages: % khiếu nại trên tổng đơn, labels: tên shop)
                var series = shopStats.Select(x => x.totalComplaints).ToList();
                var percentages = shopStats.Select(x => x.complaintRate).ToList();
                var labels = shopStats.Select(x => x.shopName).ToList();
                int total = shopStats.Sum(x => x.totalOrders);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        series = series,
                        percentages = percentages,
                        labels = labels,
                        total = total
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "An error occurred: " + ex.Message });

            }
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> RecentShops(string period = "today")
        {
            try
            {
                DateTime startDate = GetStartDateByPeriod(period);
                DateTime endDate = DateTime.Now;

                // Lấy tất cả shop
                var shops = await _storedetail.ListAsync();

                // Lấy tất cả sản phẩm
                var allProducts = await _product.ListAsync();
                // Lấy tất cả variant
                var allProductTypes = await _variantService.ListAsync();
                // Lấy tất cả OrderDetail trong period
                var allOrderDetails = period.ToLower() == "alltime"
                    ? await _orderDetail.ListAsync()
                    : await _orderDetail.ListAsync(od => od.CreatedDate >= startDate && od.CreatedDate <= endDate);

                // Lấy order IDs
                var orderIds = allOrderDetails.Select(od => od.OrderID).Distinct().ToList();
                var orders = orderIds.Any() ? await _order.ListAsync(o => orderIds.Contains(o.ID)) : new List<Order>();

                // Lấy user chủ shop
                var userIds = shops.Select(s => s.UserID).Distinct().ToList();
                var users = await _userManager.Users
                    .Where(u => userIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => new { u.UserName, u.FirstName, u.LastName });

                // Xác định shop nào thỏa điều kiện:
                // 1. Được tạo trong filter
                // 2. Hoặc có đơn phát sinh trong filter
                var shopIdWithOrder = allProducts
                    .Join(allProductTypes, p => p.ID, pt => pt.ProductID, (p, pt) => new { p.StoreID, pt.ID })
                    .Join(allOrderDetails, x => x.ID, od => od.ProductTypesID, (x, od) => x.StoreID)
                    .Distinct()
                    .ToHashSet();

                var result = shops
                    .Where(shop =>
                        // shop tạo trong filter, hoặc có đơn trong filter
                        (shop.CreatedDate >= startDate && shop.CreatedDate <= endDate)
                        || shopIdWithOrder.Contains(shop.ID)
                    )
                    .Select(shop =>
                    {
                        // Sản phẩm của shop
                        var productsOfShop = allProducts.Where(p => p.StoreID == shop.ID).ToList();
                        int totalProducts = productsOfShop.Count;

                        // Lọc variant (ProductType) thuộc sản phẩm của shop
                        var productTypeIds = allProductTypes
                            .Where(pt => productsOfShop.Select(p => p.ID).Contains(pt.ProductID))
                            .Select(pt => pt.ID)
                            .ToList();

                        // Lọc OrderDetail trong period thuộc shop
                        var odOfShop = allOrderDetails
                            .Where(od => productTypeIds.Contains(od.ProductTypesID))
                            .ToList();

                        // OrderId của shop trong period
                        var orderIdsOfShop = odOfShop.Select(od => od.OrderID).Distinct().ToList();

                        // Orders của shop trong period
                        var ordersOfShop = orders.Where(o => orderIdsOfShop.Contains(o.ID)).ToList();

                        // Tổng doanh thu của shop trong period
                        decimal grossSales = ordersOfShop.Sum(o => o.TotalPrice);

                        // Seller
                        var user = users.ContainsKey(shop.UserID) ? users[shop.UserID] : null;
                        var sellerName = user != null
                            ? (!string.IsNullOrEmpty(user.FirstName) || !string.IsNullOrEmpty(user.LastName)
                                ? $"{user.FirstName} {user.LastName}".Trim()
                                : user.UserName)
                            : "Unknown";

                        return new
                        {
                            id = shop.ID,
                            name = shop.Name,
                            customer = new { name = sellerName },
                            quantity = totalProducts,           // Nếu shop chưa có sp thì là 0
                            amount = grossSales,                // Nếu shop chưa có đơn thì là 0
                            status = shop.IsActive ? "Active" : "Banned"
                        };
                    })
                    .OrderByDescending(x => x.amount)
                    .ToList();

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "An error occurred: " + ex.Message });

            }
        }


        [AllowAnonymous]
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

        public async Task<IActionResult> ViewStoreDetail(Guid id)
        {
            var model = await _storeService.GetStoreDetailAsync(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        public async Task<IActionResult> RecipeDetail(Guid id)
        {
            var recipe = await _recipeService.GetAsyncById(id);
            if (recipe == null) return NotFound();

            var typeOfDish = await _typeOfDishService.GetAsyncById(recipe.TypeOfDishID);

            // Các tag đã chọn (từ bảng liên kết)
            var selectedTags = (await _recipeIngredientTagIngredientTagIngredientTagSerivce
                .ListAsync(rt => rt.RecipeID == recipe.ID, null, include => include.Include(x => x.IngredientTag)))
                .ToList();

            // Toàn bộ tag từ hệ thống
            var allTags = (await _ingredienttag.ListAsync()).ToList();

            var viewModel = new RecipeViewModels
            {
                ID = recipe.ID,
                Title = recipe.Title,
                ShortDescriptions = recipe.ShortDescriptions,
                PreparationTime = recipe.PreparationTime,
                CookTime = recipe.CookTime,
                TotalTime = recipe.TotalTime,
                DifficultyLevel = recipe.DifficultyLevel,
                Servings = recipe.Servings,
                CreatedDate = recipe.CreatedDate,
                IsActive = recipe.IsActive,
                CateID = recipe.CateID,
                ThumbnailImage = recipe.ThumbnailImage,
                TypeOfDishName = typeOfDish?.Name ?? "Unknown",
                TypeOfDishID = recipe.TypeOfDishID,
                CookingStep = recipe.CookingStep,
                Ingredient = recipe.Ingredient,
                // ⚠️ CHỈNH SỬA Ở ĐÂY
                IngredientTags = allTags, // tất cả tag
                SelectedIngredientTags = selectedTags.Select(x => x.IngredientTagID).ToList(), // chỉ ID đã chọn

                typeOfDishes = (await _typeOfDishService.ListAsync()).ToList(),
                Categories = (await _categoryService.ListAsync()).ToList(),
            };

            return View("RecipeDetail", viewModel);
        }

        [HttpGet]
        public async Task<JsonResult> CheckNameExists(string name)
        {
            var list = await _categoryService.ListAsync(c => c.Name.ToLower() == name.Trim().ToLower());
            bool exists = list.Any();
            return Json(new { exists });
        }

        [HttpGet]
        public async Task<JsonResult> CheckNameExistsForUpdate(string name, Guid id)
        {
            var exists = (await _categoryService.ListAsync(
                c => c.ID != id && c.Name.ToLower().Trim() == name.Trim().ToLower()
            )).Any();

            return Json(new { exists });
        }




    }


}

