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
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.RecipeServices;
using BusinessLogic.Services.StoreDetail;
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



        public AdminController(UserManager<AppUser> userManager, ITypeOfDishService typeOfDishService, IIngredientTagService ingredientTagService, IStoreDetailService storeService, IMapper mapper, IWebHostEnvironment webHostEnvironment, StoreDetailsRepository storeRepository, IBalanceChangeService balance, ICategoryService categoryService, ManageTransaction managetrans, IComplaintServices complaintService, IOrderDetailService orderDetail, IOrdersServices order, IProductVariantService variantService, IComplaintImageServices complaintImage, IStoreDetailService storeDetailService, IProductService product, IVoucherServices voucher, IRecipeService recipeService)

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
        }
        public async Task<IActionResult> Index()
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



    }


}

