using System.Net.Http.Headers;
using AutoMapper;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
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

        public AdminController(UserManager<AppUser> userManager, IStoreDetailService storeService, IMapper mapper, IWebHostEnvironment webHostEnvironment, StoreDetailsRepository storeRepository, IBalanceChangeService balance, ICategoryService categoryService)
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

    }
}
