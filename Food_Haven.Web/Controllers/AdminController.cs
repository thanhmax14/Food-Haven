using System.Net.Http.Headers;
using AutoMapper;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.BalanceChange;
using Repository.StoreDetails;
using Repository.ViewModels;

namespace Food_Haven.Web.Controllers
{
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

        public AdminController(UserManager<AppUser> userManager, IStoreDetailService storeService, IMapper mapper, IWebHostEnvironment webHostEnvironment, StoreDetailsRepository storeRepository, IBalanceChangeService balance, ICategoryService categoryService, ManageTransaction managetrans)
        {
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

    }
}
