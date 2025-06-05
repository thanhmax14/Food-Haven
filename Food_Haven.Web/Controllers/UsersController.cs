using System.Data;
using BusinessLogic.Hash;
using System.Text.RegularExpressions;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Net.payOS;
using Net.payOS.Types;
using Repository.BalanceChange;
using Repository.ViewModels;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Food_Haven.Web.Hubs;
using MailKit.Search;
using System.Collections.Generic;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.ProductVariantVariants;
using BusinessLogic.Services.ComplaintImages;
using BusinessLogic.Services.Complaints;
using Org.BouncyCastle.Asn1.X509;
using BusinessLogic.Services.RecipeServices;
using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.IngredientTagServices;
using BusinessLogic.Services.TypeOfDishServices;
using Azure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using X.PagedList;
using BusinessLogic.Services.RecipeIngredientTagIngredientTagServices; // nhớ import

namespace Food_Haven.Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private HttpClient client = null;
        private string _url;
        private readonly IBalanceChangeService _balance;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _product;
        public readonly ICartService _cart;
        public readonly IProductVariantService _productWarian;
        private readonly IProductImageService _img;
        private readonly IOrdersServices _order;
        private readonly IOrderDetailService _orderDetailService;
        private readonly PayOS _payos;
        private readonly ManageTransaction _managetrans;
        private readonly IReviewService _review;
        private readonly IRecipeService _recipeService;
        private readonly ICategoryService _categoryService;
        private readonly IIngredientTagService _ingredientTagService;
        private readonly ITypeOfDishService _typeOfDishService;
        private readonly IComplaintImageServices _complaintImageServices;
        private readonly IComplaintServices _complaintService;
        private readonly IRecipeIngredientTagIngredientTagSerivce _recipeIngredientTagIngredientTagIngredientTagSerivce;

        public UsersController(UserManager<AppUser> userManager, HttpClient client, IBalanceChangeService balance, IHttpContextAccessor httpContextAccessor, IProductService product, ICartService cart, IProductVariantService productWarian, IProductImageService img, IOrdersServices order, IOrderDetailService orderDetailService, PayOS payos, ManageTransaction managetrans, IReviewService review, IRecipeService recipeService, ICategoryService categoryService, IIngredientTagService ingredientTagService, ITypeOfDishService typeOfDishService, IComplaintImageServices complaintImageServices, IComplaintServices complaintService, IRecipeIngredientTagIngredientTagSerivce recipeIngredientTagIngredientTagIngredientTagSerivce)
        {
            _userManager = userManager;
            this.client = client;
            _balance = balance;
            _httpContextAccessor = httpContextAccessor;
            _product = product;
            _cart = cart;
            _productWarian = productWarian;
            _img = img;
            _order = order;
            _orderDetailService = orderDetailService;
            _payos = payos;
            _managetrans = managetrans;
            _review = review;
            _recipeService = recipeService;
            _categoryService = categoryService;
            _ingredientTagService = ingredientTagService;
            _typeOfDishService = typeOfDishService;
            _complaintImageServices = complaintImageServices;
            _complaintService = complaintService;
            _recipeIngredientTagIngredientTagIngredientTagSerivce = recipeIngredientTagIngredientTagIngredientTagSerivce;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            // Kiểm tra người dùng có đăng nhập hay không
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Lấy ID của user đăng nhập
            string userId = user.Id;
            var list = new IndexUserViewModels();
            try
            {
                var user1 = await _userManager.FindByIdAsync(userId);
                if (user1 == null)
                {
                    return BadRequest(new { message = "User ID không hợp lệ." });
                }

                /*   var stores = await IStoreDetailService.ListAsync(x => x.UserID == id);
                  var storeId = stores?.FirstOrDefault()?.ID ?? Guid.Empty; // Kiểm tra null */

                var UserModel = new UsersViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Birthday = user.Birthday,
                    Address = user.Address,
                    img = user.ImageUrl,
                    RequestSeller = user.RequestSeller,
                    isUpdateProfile = user.IsProfileUpdated,
                    ModifyUpdate = user.ModifyUpdate,
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.UserName,
                    Email = user.Email,
                    /*   StoreDeatilId = storeId */
                };
                list.userView = UserModel;
                var getOrder = await this._order.ListAsync(u => u.UserID == user.Id);
                getOrder = getOrder.OrderByDescending(x => x.CreatedDate).ToList();
                if (getOrder.Any())
                {
                    var count = 0;
                    foreach (var item in getOrder)
                    {
                        list.OrderViewodels.Add(new OrderViewModel
                        {
                            stt = count++,
                            DeliveryAddress = item.DeliveryAddress,
                            OrderDate = item.CreatedDate,
                            PaymentMethod = item.PaymentMethod,
                            Status = item.Status,
                            Total = item.TotalPrice,
                            OrderId = item.ID,
                            OrderTracking = item.OrderTracking,
                            DeliveryDate = item.ModifiedDate,
                            //  Desctiption = item.Description,
                            Note = item.Note,
                            Quantity = item.Quantity,
                            StatusPayment = item.PaymentStatus

                        }); ;
                    }
                }


                /*  var OrderId = getOrder.FirstOrDefault()?.ID;
                  var getOrderDetail = await _orderDetailService.ListAsync(x => x.OrderID == OrderId);
                  if (getOrderDetail.Any())
                  {
                      var productList = await _productWarian.ListAsync();

                      foreach (var item in getOrderDetail)
                      {
                          var product = productList.FirstOrDefault(x => x.ID == item.ID);
                          var productName = product?.Name;
                          list.orderDetailsViewModels.Add(new OrderDetailsViewModel
                          {
                              productName = productName,
                              ProductPrice = item.ProductPrice,
                              TotalPrice = item.TotalPrice,
                              Quantity = item.Quantity,
                              Status = item.Status,
                          });
                      }
                  }*/
                return View(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody] IndexUserViewModels obj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }
            if (!string.IsNullOrWhiteSpace(obj.userView.PhoneNumber))
            {
                var inputPhone = obj.userView.PhoneNumber.Trim();

                // Kiểm tra đúng 10 chữ số
                if (!Regex.IsMatch(inputPhone, @"^\d{10}$"))
                {
                    return Json(new { success = false, message = "Phone number must be exactly 10 digits." });
                }

                var existPhone = await _userManager.Users
                    .AsNoTracking()
                    .Where(x => !string.IsNullOrEmpty(x.PhoneNumber) &&
                                x.PhoneNumber == inputPhone &&
                                x.Id != user.Id)
                    .FirstOrDefaultAsync();

                if (existPhone != null)
                {
                    return Json(new { success = false, message = "Phone number already exists." });
                }

                user.PhoneNumber = inputPhone;
            }

            if (obj.userView.Birthday <= DateTime.Today)
            {
                user.Birthday = obj.userView.Birthday;
            }
            else
            {
                return Json(new { success = false, message = "Date of birth cannot be greater than the current date." });
            }
            user.Email = obj.userView.Email;
            user.Address = obj.userView.Address;
            user.FirstName = obj.userView.FirstName;
            user.LastName = obj.userView.LastName;
            user.IsProfileUpdated = true;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Profile updated successfully" });
            }
            else
            {
                return Json(new { success = false, message = result.Errors.FirstOrDefault()?.Description ?? "Update failed" });
            }

        }
        [HttpPost]
        public async Task<IActionResult> RegisterSeller(IndexUserViewModels obj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }
            else if (string.IsNullOrEmpty(user.Address) || string.IsNullOrEmpty(user.PhoneNumber) ||
            user.Birthday == null || user.Birthday == DateTime.MinValue || string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))
            {
                return Json(new
                {
                    success = false,
                    message = "Please complete all required information before registering as a seller."
                });
            }
            user.RequestSeller = "1";
            user.ModifyUpdate = DateTime.Now;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Register Seller successfully" });
            }
            else
            {
                return Json(new { success = false, message = result.Errors.FirstOrDefault()?.Description ?? "Register Seller failed" });

            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBalance(long number)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { notAuth = true, message = "Bạn phải đăng nhập thể thực hiện hành động này!" });
            }
            if (number < 100000)
            {
                return Json(new ErroMess { msg = "Nạp tối thiểu 100,000 VND" });
            }
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            this._url = "https://localhost:5555/Gateway/WalletService/CreatePayment";
            var temdata = new DepositViewModel
            {
                number = number,
                CalleURL = $"{baseUrl}/home/invoice",
                ReturnUrl = $"{baseUrl}/Users#wallet",
                UserID = user.Id
            };

            try
            {
                var getUser = await this._userManager.FindByIdAsync(temdata.UserID);
                if (getUser == null)
                    return Json(new ErroMess { msg = "Người dùng không tồn tại trong hệ thống" });
                var tien = await this._balance.GetBalance(getUser.Id);


                int orderCode = RandomCode.GenerateOrderCode();
                var check = await this._balance.FindAsync(u => u.OrderCode == orderCode);
                while (check != null)
                {
                    orderCode = RandomCode.GenerateOrderCode();
                    check = await this._balance.FindAsync(u => u.OrderCode == orderCode);
                }
                long expirationTimestamp = DateTimeOffset.Now.AddDays(1).ToUnixTimeSeconds();

                ItemData item = new ItemData($"Thực hiện nạp tiền vào tài khoản {getUser.UserName}:", 1, int.Parse(temdata.number + ""));
                List<ItemData> items = new List<ItemData> { item };
                PaymentData paymentData = new PaymentData(orderCode, int.Parse(temdata.number + ""), "", items, $"{temdata.CalleURL}/{orderCode}",
                   $"{temdata.ReturnUrl}/{orderCode}"
                , null, null, null, null, null, expirationTimestamp
                   );
                CreatePaymentResult createPayment = await this._payos.createPaymentLink(paymentData);
                var url = $"https://pay.payos.vn/web/{createPayment.paymentLinkId}/";
                await this._managetrans.ExecuteInTransactionAsync(async () =>
                {
                    var statime = DateTime.Now;
                    var temDongTien = new BalanceChange
                    {
                        MoneyBeforeChange = tien,
                        MoneyChange = temdata.number,
                        MoneyAfterChange = 0m,
                        Description = $"{url}",
                        Status = "PROCESSING",
                        Method = "Deposit",
                        OrderCode = orderCode,
                        StartTime = statime,
                        DueTime = statime,
                        CheckDone = false,

                        UserID = getUser.Id,

                    };

                    await this._balance.AddAsync(temDongTien);

                });
                await this._balance.SaveChangesAsync();

                return Json(new ErroMess { success = true, msg = $"{url}" }); ;
            }
            catch (System.Exception exception)
            {
                Console.WriteLine(exception);
                return Json(new { success = false, msg = "Lỗi không xác định, vui lòng thử lại." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateBalance()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json("Phai Dang Nhap");
            }
            var list = new List<BalanceListViewModels>();
            if (string.IsNullOrWhiteSpace(user.Id))
            {
                return Json(new ErroMess { msg = "Vui lòng nhập userID" });
            }

            try
            {
                var getUser = await this._userManager.FindByIdAsync(user.Id);
                if (getUser == null)
                    return Json(new ErroMess { msg = "Người dùng không tồn tại trong hệ thống" });
                else
                {
                    var getListBalance = await this._balance.ListAsync(
                   u => u.Display && getUser.Id == u.UserID,
                   orderBy: x => x.OrderByDescending(query => query.DueTime.HasValue)  // Ưu tiên bản ghi có DueTime
                                   .ThenByDescending(query => query.DueTime)           // Sắp xếp giảm dần theo DueTime
                                   .ThenByDescending(query => query.StartTime)         // Nếu DueTime = NULL, dùng StartTime
               );


                    if (getListBalance.Any())
                    {
                        var count = 0;
                        foreach (var item in getListBalance)
                        {
                            count++;
                            var getInvoce = RegexAll.ExtractPayosLink(item.Description);
                            if (getInvoce == null)
                                getInvoce = item.Description;
                            list.Add(new BalanceListViewModels
                            {
                                No = count,
                                After = item.MoneyAfterChange,
                                Before = item.MoneyBeforeChange,
                                Change = item.MoneyChange,
                                Date = item.DueTime ?? DateTime.Now,
                                Invoice = getInvoce,
                                Status = item.Status,
                                Types = item.Method
                            });
                        }
                    }
                    return Json(list);

                }
            }
            catch (Exception)
            {
                return Json("Lỗi Hệ Thống");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy([FromForm] List<Guid> productIds)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new ErroMess { msg = "Bạn chưa đăng nhập!!" });
            }

            if (productIds == null || !productIds.Any())
            {
                return Json(new ErroMess { msg = "Vui lòng chọn sản phẩm cần mua!" });
            }
            if (!user.IsProfileUpdated)
            {
                return Json(new ErroMess { msg = "Bạn phải thêm thông tin cá nhân trước khi mua hàng!." });
            }
            var listItem = new List<ListItems>();
            foreach (var id in productIds)
            {
                var product = await _productWarian.GetAsyncById(id);
                if (product == null)
                {
                    return Json(new ErroMess { msg = "Sản phẩm mua không tồn tại!" });
                }
                var checkcart = await this._cart.FindAsync(u => u.UserID == user.Id && u.ProductTypesID == id);
                if (checkcart == null)
                {
                    return Json(new ErroMess { msg = "Sản phẩm mua không tồn tại trong giỏ hàng!" });
                }
                var getQuatity = await this._productWarian.FindAsync(u => u.ID == id);
                if (checkcart.Quantity > getQuatity.Stock)
                {
                    return Json(new ErroMess { msg = "Số lượng sản phẩm mua vượt quá số lượng tồn kho!" });
                }

                var getImg = await this._img.FindAsync(u => u.ProductID == id && u.IsMain);

                var img = "https://nest-frontend-v6.vercel.app/assets/imgs/shop/product-1-1.jpg";
                if (getImg != null)
                {
                    img = getImg.ImageUrl;
                }


                listItem.Add(new ListItems
                {
                    ItemName = product.Name,
                    ItemImage = img,
                    ItemPrice = getQuatity.SellPrice,
                    ItemQuantity = checkcart.Quantity,
                    productID = product.ID
                });

            }

            var temInfo = new CheckOutView
            {
                address = user.Address,
                email = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName,
                phone = user.PhoneNumber,
                itemCheck = listItem
            };

            HttpContext.Session.Set("BillingTourInfo", JsonSerializer.SerializeToUtf8Bytes(temInfo));

            return Json(new { success = true, message = "Danh sách sản phẩm đã được xử lý.", selectedProducts = productIds, redirectUrl = "/Users/CheckOut" });
        }


        public async Task<IActionResult> CheckOut()
        {
            var a = base.HttpContext.Session.GetString("BillingTourInfo");


            if (HttpContext.Session.TryGetValue("BillingTourInfo", out byte[] data))
            {
                var billingInfo = JsonSerializer.Deserialize<CheckOutView>(data);
                if (billingInfo != null)
                {
                    return View(billingInfo);
                }
            }
            return RedirectToAction("NotFoundPage", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPayment(OrderInputModel model, string paymentOption)
        {
            ModelState.Remove("Note");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new ErroMess { msg = "Bạn chưa đăng nhập!!" });
            }
            if (string.IsNullOrWhiteSpace(paymentOption))
            {
                return Json(new ErroMess { msg = "Vui lòng chọn phương thức thanh toán!" });
            }

            if (!ModelState.IsValid)
            {
                var errorMsg = ModelState.Values
                                          .SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .FirstOrDefault();

                return Json(new ErroMess { msg = errorMsg ?? "Dữ liệu không hợp lệ!" });
            }
            if (paymentOption.ToLower() == "OnlineGateway".ToLower())
            {
                if (HttpContext.Session.TryGetValue("BillingTourInfo", out byte[] data))
                {
                    var billingInfo = JsonSerializer.Deserialize<CheckOutView>(data);
                    if (billingInfo != null)
                    {
                        var buyRequest = new BuyRequest();
                        foreach (var item in billingInfo.itemCheck)
                        {
                            buyRequest.Products.Add(item.productID, item.ItemQuantity);

                        }
                        var request = _httpContextAccessor.HttpContext.Request;
                        var baseUrl = $"{request.Scheme}://{request.Host}";
                        buyRequest.IsOnline = true;
                        buyRequest.UserID = user.Id;
                        buyRequest.SuccessUrl = $"{baseUrl}/home/invoice";
                        buyRequest.CalledUrl = $"{baseUrl}/home/invoice";






                        if (buyRequest.Products == null || !buyRequest.Products.Any())
                        {
                            return Json(new ErroMess { msg = "Vui lòng chọn sản phẩm cần mua!" });
                        }

                        var temOrderDeyail = new List<OrderDetail>();
                        decimal totelPrice = 0;


                        foreach (var id in buyRequest.Products)
                        {
                            var checkcart = await this._cart.FindAsync(u => u.UserID == buyRequest.UserID && u.ProductTypesID == id.Key);
                            if (checkcart != null)
                            {
                                var getQuatity = await this._productWarian.FindAsync(u => u.ID == id.Key && u.IsActive);

                                if (getQuatity == null)
                                {
                                    return NotFound(new ErroMess { msg = "Sản phẩm mua không tồn tại!" });
                                }

                                if (id.Value < getQuatity.Stock)
                                {
                                    totelPrice += getQuatity.SellPrice * id.Value;
                                }
                            }
                        }
                        var orderID = Guid.NewGuid();

                        foreach (var id in buyRequest.Products)
                        {
                            var product = await _product.GetAsyncById(id.Key);
                            if (product == null)
                            {
                                return Json(new ErroMess { msg = "Sản phẩm mua không tồn tại!" });
                            }
                            /* var checkcart = await this._cart.FindAsync(u => u.UserID == request.UserID && u.ProductID == id.Key);
                             if (checkcart == null)
                             {
                                 return NotFound(new ErroMess { msg = "Sản phẩm mua không tồn tại trong giỏ hàng!" });
                             }*/
                            var getQuatity = await this._productWarian.FindAsync(u => u.ID == id.Key);
                            if (id.Value > getQuatity.Stock)
                            {
                                return Json(new ErroMess { msg = "Số lượng sản phẩm mua vượt quá số lượng tồn kho!" });
                            }

                            temOrderDeyail.Add(new OrderDetail
                            {
                                ID = Guid.NewGuid(),
                                OrderID = orderID,
                                ProductTypesID = id.Key,
                                Quantity = id.Value,
                                ProductPrice = getQuatity.SellPrice,
                                TotalPrice = getQuatity.SellPrice * id.Value,
                                Status = "PROCESSING",
                                IsActive = false
                            });

                        }
                        int orderCode = RandomCode.GenerateOrderCode();
                        var check = await this._order.FindAsync(u => u.OrderCode == orderCode + "");
                        while (check != null)
                        {
                            orderCode = RandomCode.GenerateOrderCode();
                            check = await this._order.FindAsync(u => u.OrderCode == orderCode + "");
                        }
                        var order = new Order
                        {
                            ID = orderID,
                            UserID = buyRequest.UserID,
                            TotalPrice = totelPrice,
                            Status = "PROCESSING",
                            CreatedDate = DateTime.Now,
                            PaymentMethod = "wallet",
                            PaymentStatus = "PROCESSING",
                            Quantity = buyRequest.Products.Sum(u => u.Value),
                            OrderCode = "" + orderCode,
                            DeliveryAddress = model.Address,
                            Note = model.Note ?? ""

                        };
                        /*   var balan = new BalanceChange
                           {
                               UserID = user.Id,
                               MoneyChange = -totelPrice,
                               MoneyBeforeChange = await _balance.GetBalance(user.Id),
                               MoneyAfterChange = await _balance.GetBalance(user.Id) - totelPrice,
                               Method = "Buy",
                               Status = "PROCESSING",
                               DisPlay = true,
                               IsComplele = false,
                               checkDone = true,
                               StartTime = DateTime.Now
                           };*/


                        try
                        {
                            // await this._balance.AddAsync(balan);
                            await _order.AddAsync(order);
                            await this._balance.SaveChangesAsync();
                            await this._order.SaveChangesAsync();
                        }
                        catch
                        {
                            order.PaymentStatus = "Failed";
                            order.Status = "Failed";
                            /*  balan.Status = "Failed";
                              balan.DueTime = DateTime.Now;
                              balan.MoneyBeforeChange = await _balance.GetBalance(user.Id);
                              balan.MoneyAfterChange = await _balance.GetBalance(user.Id) + totelPrice;
                              balan.MoneyChange = totelPrice;*/
                            await this._order.SaveChangesAsync();
                            // await this._balance.SaveChangesAsync();
                            return BadRequest(new ErroMess { msg = "Đã xảy ra lỗi trông quá trình mua!11" });
                        }

                        try
                        {
                            var url = "";
                            var result = await _managetrans.ExecuteInTransactionAsync(async () =>
                            {
                                foreach (var item1 in temOrderDeyail)
                                {
                                    await _orderDetailService.AddAsync(item1);
                                }
                                long expirationTimestamp = DateTimeOffset.Now.AddMinutes(5).ToUnixTimeSeconds();
                                ItemData item = new ItemData($"Thực hiện mua hàng ở tài khoản {user.UserName}:", 1, (int)totelPrice);
                                List<ItemData> items = new List<ItemData> { item };
                                PaymentData paymentData = new PaymentData(orderCode, (int)totelPrice, "", items, $"{buyRequest.CalledUrl}/{orderCode}",
                                   $"{buyRequest.SuccessUrl}/{orderCode}"
                                , null, null, null, null, null, expirationTimestamp
                                   );
                                CreatePaymentResult createPayment = await this._payos.createPaymentLink(paymentData);
                                url = $"https://pay.payos.vn/web/{createPayment.paymentLinkId}/";
                            });
                            await this._orderDetailService.SaveChangesAsync();
                            if (result)
                            {
                                var productKeys = buyRequest.Products;

                                foreach (var productId in productKeys)
                                {

                                    var getWarian = await this._productWarian.FindAsync(u => u.ID == productId.Key);
                                    getWarian.Stock -= productId.Value;
                                    if (getWarian.Stock == 0 || getWarian.Stock < 0)
                                    {
                                        getWarian.Stock = 0;
                                        getWarian.IsActive = false;
                                    }
                                    await this._productWarian.UpdateAsync(getWarian);
                                    await this._productWarian.SaveChangesAsync();
                                }
                                return Json(new { success = true, msg = $"{url}", haveUrl = true, redirectUrl = $"{url}" }); ;
                            }
                            else
                            {
                                return Json(new ErroMess { msg = "Đã xảy ra lỗi trông quá trình mua!22" });
                            }
                        }
                        catch (Exception e)
                        {
                            order.PaymentStatus = "Failed";
                            order.Status = "Failed";
                            await this._order.SaveChangesAsync();
                            return Json(new ErroMess { msg = "Đã xảy ra lỗi trông quá trình mua!" });
                        }
                    }
                }
            }



            if (paymentOption.ToLower() == "Wallet".ToLower())
            {
                if (HttpContext.Session.TryGetValue("BillingTourInfo", out byte[] data))
                {
                    var billingInfo = JsonSerializer.Deserialize<CheckOutView>(data);
                    if (billingInfo != null)
                    {
                        var buyRequest = new BuyRequest();
                        foreach (var item in billingInfo.itemCheck)
                        {
                            buyRequest.Products.Add(item.productID, item.ItemQuantity);
                        }
                        var request = _httpContextAccessor.HttpContext.Request;
                        var baseUrl = $"{request.Scheme}://{request.Host}";
                        buyRequest.IsOnline = false;
                        buyRequest.UserID = user.Id;
                        buyRequest.SuccessUrl = $"{baseUrl}/home/invoice";
                        buyRequest.CalledUrl = $"{baseUrl}/home/invoice";


                        if (buyRequest.Products == null || !buyRequest.Products.Any())
                        {
                            return Json(new ErroMess { msg = "Vui lòng chọn sản phẩm cần mua!" });
                        }
                        var addedDetails = new List<OrderDetail>();
                        var temOrderDeyail = new List<OrderDetail>();
                        decimal totelPrice = 0;


                        foreach (var id in buyRequest.Products)
                        {
                            var checkcart = await this._cart.FindAsync(u => u.UserID == buyRequest.UserID && u.ProductTypesID == id.Key);
                            if (checkcart != null)
                            {
                                var getQuatity = await this._productWarian.FindAsync(u => u.ID == id.Key && u.IsActive);

                                if (getQuatity == null)
                                    return Json(new ErroMess { msg = "Sản phẩm mua không tồn tại!" });
                                if (checkcart.Quantity < getQuatity.Stock)
                                {
                                    totelPrice += getQuatity.SellPrice * id.Value;
                                }
                            }
                        }
                        var orderID = Guid.NewGuid();
                        if (await _balance.CheckMoney(user.Id, totelPrice) == false)
                        {
                            return Json(new ErroMess { msg = "Số dư trong tài khoản không đủ để mua hàng!" });
                        }
                        foreach (var id in buyRequest.Products)
                        {
                            var product = await _productWarian.GetAsyncById(id.Key);
                            if (product == null)
                            {
                                return Json(new ErroMess { msg = "Sản phẩm mua không tồn tại!" });
                            }
                            var checkcart = await this._cart.FindAsync(u => u.UserID == buyRequest.UserID && u.ProductTypesID == id.Key);
                            if (checkcart == null)
                            {
                                return Json(new ErroMess { msg = "Sản phẩm mua không tồn tại trong giỏ hàng!" });
                            }
                            var getQuatity = await this._productWarian.FindAsync(u => u.ID == id.Key);
                            if (checkcart.Quantity > getQuatity.Stock)
                            {
                                return Json(new ErroMess { msg = "Số lượng sản phẩm mua vượt quá số lượng tồn kho!" });
                            }

                            temOrderDeyail.Add(new OrderDetail
                            {
                                ID = Guid.NewGuid(),
                                OrderID = orderID,
                                ProductTypesID = id.Key,
                                Quantity = id.Value,
                                ProductPrice = getQuatity.SellPrice,
                                TotalPrice = getQuatity.SellPrice * id.Value,
                                Status = "Pending"
                            });

                        }

                        var order = new Order
                        {
                            ID = orderID,
                            UserID = buyRequest.UserID,
                            OrderTracking = RandomCode.GenerateUniqueCode(),
                            TotalPrice = totelPrice,
                            Status = "Pending",
                            CreatedDate = DateTime.Now,
                            PaymentMethod = "wallet",
                            PaymentStatus = "Pending",
                            Quantity = buyRequest.Products.Sum(u => u.Value),
                            OrderCode = "",
                            DeliveryAddress = model.Address,
                            Note = model.Note ?? "",
                            Description = $"Pending-{DateTime.Now}"


                        };
                        var balan = new BalanceChange
                        {
                            UserID = user.Id,
                            MoneyChange = -totelPrice,
                            MoneyBeforeChange = await _balance.GetBalance(user.Id),
                            MoneyAfterChange = await _balance.GetBalance(user.Id) - totelPrice,
                            Method = "Buy",
                            Status = "Pending",
                            Display = true,
                            IsComplete = false,
                            CheckDone = true,
                            StartTime = DateTime.Now
                        };

                        if (await _balance.CheckMoney(user.Id, totelPrice))
                        {
                            try
                            {
                                await this._balance.AddAsync(balan);
                                await _order.AddAsync(order);
                                await this._balance.SaveChangesAsync();
                                await this._order.SaveChangesAsync();
                            }
                            catch
                            {
                                foreach (var item in addedDetails)
                                {
                                    item.Status = "Refunded";
                                    item.ModifiedDate = DateTime.Now;
                                    await _orderDetailService.UpdateAsync(item); // Giả sử có hàm UpdateAsync
                                }
                                order.PaymentStatus = "Failed";
                                order.Status = "Refunded";
                                order.Description = $"Refunded-{DateTime.Now}";
                                balan.Status = "Failed";
                                balan.DueTime = DateTime.Now;
                                balan.MoneyBeforeChange = await _balance.GetBalance(user.Id);
                                balan.MoneyAfterChange = await _balance.GetBalance(user.Id) + totelPrice;
                                balan.MoneyChange = totelPrice;
                                await this._orderDetailService.SaveChangesAsync();
                                await this._order.SaveChangesAsync();
                                await this._balance.SaveChangesAsync();
                                return Json(new ErroMess { msg = "Đã xảy ra lỗi trông quá trình mua!" });
                            }
                        }
                        try
                        {

                            var result = await _managetrans.ExecuteInTransactionAsync(async () =>
                            {
                                foreach (var item in temOrderDeyail)
                                {
                                    await _orderDetailService.AddAsync(item);
                                    addedDetails.Add(item);
                                }
                            });
                            await this._orderDetailService.SaveChangesAsync();
                            if (result)
                            {
                                balan.Status = "Success";
                                order.PaymentStatus = "Success";
                                order.Status = "Pending";
                                balan.DueTime = DateTime.Now;
                                var productKeys = buyRequest.Products;

                                foreach (var productId in productKeys)
                                {
                                    var getCart = await this._cart.FindAsync(u => u.ProductTypesID == productId.Key);

                                    if (getCart != null)
                                    {
                                        await this._cart.DeleteAsync(getCart);
                                    }
                                    var getWarian = await this._productWarian.FindAsync(u => u.ID == productId.Key);
                                    getWarian.Stock -= productId.Value;
                                    if (getWarian.Stock == 0 || getWarian.Stock < 0)
                                    {
                                        getWarian.Stock = 0;
                                        getWarian.IsActive = false;
                                    }
                                    await this._productWarian.UpdateAsync(getWarian);
                                    await this._productWarian.SaveChangesAsync();
                                }
                                await this._cart.SaveChangesAsync();
                                await this._balance.SaveChangesAsync();
                                await this._order.SaveChangesAsync();
                                var hubContext1 = HttpContext.RequestServices.GetRequiredService<IHubContext<CartHub>>();
                                await hubContext1.Clients.User(user.Id).SendAsync("ReceiveCartUpdate");
                                return Json(new ErroMess { success = true, msg = "Đặt hàng thành công!, Trở về Order Sau 3s" });
                            }
                            else
                            {
                                foreach (var item in addedDetails)
                                {
                                    item.Status = "Refunded";
                                    item.ModifiedDate = DateTime.Now;
                                    await _orderDetailService.UpdateAsync(item);
                                }
                                order.PaymentStatus = "Failed";
                                order.Status = "Refunded";
                                order.Description = $"Failed-{DateTime.Now}";
                                balan.Status = "Failed";
                                balan.DueTime = DateTime.Now;
                                balan.MoneyBeforeChange = await _balance.GetBalance(user.Id);
                                balan.MoneyAfterChange = await _balance.GetBalance(user.Id) + totelPrice;
                                balan.MoneyChange = totelPrice;
                                await this._orderDetailService.SaveChangesAsync();
                                await this._order.SaveChangesAsync();
                                await this._balance.SaveChangesAsync();
                                return Json(new ErroMess { msg = "Đã xảy ra lỗi trông quá trình mua!" });
                            }
                        }
                        catch (Exception e)
                        {
                            foreach (var item in addedDetails)
                            {
                                item.Status = "Refunded";
                                item.ModifiedDate = DateTime.Now;
                                await _orderDetailService.UpdateAsync(item); // Giả sử có hàm UpdateAsync
                            }
                            order.PaymentStatus = "Failed";
                            order.Status = "Refunded";
                            order.Description = $"Refunded-{DateTime.Now}";
                            balan.Status = "Failed";
                            balan.DueTime = DateTime.Now;
                            balan.MoneyBeforeChange = await _balance.GetBalance(user.Id);
                            balan.MoneyAfterChange = await _balance.GetBalance(user.Id) + totelPrice;
                            balan.MoneyChange = totelPrice;
                            await this._orderDetailService.SaveChangesAsync();
                            await this._order.SaveChangesAsync();
                            await this._balance.SaveChangesAsync();

                            return Json(new ErroMess { msg = "Đã xảy ra lỗi trông quá trình mua!" });
                        }
                    }

                }
                return Json(new ErroMess { msg = "Đã xảy ra lỗi vui lòng liên hệ admin!>" });
            }
            return Json(new ErroMess { msg = "Phương thức thanh toán không hợp lệ!" });
        }
        [HttpPost]
        public async Task<IActionResult> GetOrderDetails([FromBody] string orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }
            if (string.IsNullOrWhiteSpace(orderId))
            {
                return Json(new { success = false, message = "Order ID không hợp lệ." });
            }

            var order = await _order.FindAsync(u => u.OrderTracking.ToLower() == orderId.ToLower());
            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
            }

            var getOrderDetail = await _orderDetailService.ListAsync(x => x.OrderID == order.ID);
            if (getOrderDetail == null || !getOrderDetail.Any())
            {
                return Json(new { success = false, message = "Không có sản phẩm trong đơn hàng." });
            }

            var productTypeIds = getOrderDetail.Select(x => x.ProductTypesID).Distinct().ToList();
            var productList = await _productWarian.ListAsync(p => productTypeIds.Contains(p.ID));

            var detailDtos = new List<object>();

            foreach (var item in getOrderDetail)
            {
                var product = productList.FirstOrDefault(p => p.ID == item.ProductTypesID);
                var hasFeedback = true;
                var hasComplaint = true;
                var getComplant = await this._complaintService.FindAsync(u => u.OrderDetailID == item.ID);
                if (getComplant == null && order.Status.ToUpper() == "CONFIRMED".ToUpper())
                {
                    hasComplaint = false;
                }
                if (!item.IsFeedback && order.Status.ToUpper() == "CONFIRMED".ToUpper())
                {
                    hasFeedback = false;
                }

                detailDtos.Add(new
                {
                    productName = product?.Name ?? "Không rõ",
                    productPrice = item.ProductPrice,
                    totalPrice = item.TotalPrice,
                    quantity = item.Quantity,
                    status = item.Status,
                    hasFeedback,
                    hasComplaint,
                    productId = item.ID,
                    ortracking = order.OrderTracking,
                });
            }

            return Json(new
            {
                success = true,
                orderStatus = order.Status,
                data = detailDtos
            });

        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder([FromBody] string orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }
            if (string.IsNullOrWhiteSpace(orderId))
                return Json(new { success = false, message = "Mã đơn hàng không hợp lệ." });

            try
            {
                var order = await _order.FindAsync(o => o.OrderTracking.ToLower() == orderId.ToLower());
                if (order == null)
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng." });

                if (!string.Equals(order.Status, "Pending", StringComparison.OrdinalIgnoreCase))
                    return Json(new { success = false, message = "Chỉ có thể hủy đơn hàng đang chờ xử lý." });

                var orderDetails = await _orderDetailService.ListAsync(d => d.OrderID == order.ID);
                if (orderDetails == null || !orderDetails.Any())
                    return Json(new { success = false, message = "Đơn hàng không có sản phẩm nào." });

                foreach (var item in orderDetails)
                {
                    item.Status = "Refunded";
                    item.ModifiedDate = DateTime.UtcNow;
                    await _orderDetailService.UpdateAsync(item);

                    var product = await _productWarian.FindAsync(p => p.ID == item.ProductTypesID);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                        product.IsActive = true;
                        await _productWarian.UpdateAsync(product);
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

                order.Status = "Cancelled by User";
                order.Description = string.IsNullOrEmpty(order.Description)
    ? $"CANCELLED BY USER-{DateTime.Now}"
    : $"{order.Description}#CANCELLED BY USER-{DateTime.Now}";

                order.PaymentStatus = "Refunded";
                order.ModifiedDate = DateTime.UtcNow;
                await _order.UpdateAsync(order);


                await _orderDetailService.SaveChangesAsync();
                await _productWarian.SaveChangesAsync();
                await _order.SaveChangesAsync();
                await _balance.SaveChangesAsync();

                return Json(new { success = true, message = "Đơn hàng đã được hủy và hoàn tiền thành công." });
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
        public async Task<IActionResult> SubmitReview([FromBody] ReviewRequest model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Không tìm thấy người dùng." });
            if (model.ProductId == Guid.Empty || model.Rating < 1 || model.Rating > 5 || string.IsNullOrWhiteSpace(model.Content))
                return Json(new { success = false, message = "Thông tin đánh giá không hợp lệ." });
            var orderDetail = await _orderDetailService
                .FindAsync(od => od.ID == model.ProductId && od.Order.UserID == user.Id);

            if (orderDetail == null)
                return Json(new { success = false, message = "Sản phẩm không thuộc đơn hàng của bạn." });



            var variantExists = await _productWarian.FindAsync(v => v.ID == orderDetail.ProductTypesID);
            if (variantExists == null)
                return Json(new { success = false, message = "Không tìm thấy phiên bản sản phẩm." });

            /*  var reviewed = await _review.FindAsync(u => u.ProductID == variantExists.ID && u.UserID == user.Id);
              if (reviewed!=null)
                  return Json(new { success = false, message = "Bạn đã đánh giá sản phẩm này rồi." });*/
            if (orderDetail.IsFeedback)
            {
                return Json(new { success = false, message = "Bạn đã đánh giá sản phẩm này rồi." });
            }

            try
            {
                var newReview = new Review
                {
                    ID = Guid.NewGuid(),
                    Comment = model.Content,
                    CommentDate = DateTime.UtcNow,
                    //Relay = model.Relay,
                    //DateRelay = model.DateRelay ?? DateTime.UtcNow,
                    Status = false, //hiện
                    Rating = model.Rating,
                    UserID = user.Id,
                    ProductID = variantExists.ProductID,

                };
                await _review.AddAsync(newReview);
                orderDetail.IsFeedback = true;
                await _orderDetailService.UpdateAsync(orderDetail);
                await this._review.SaveChangesAsync();
                await this._orderDetailService.SaveChangesAsync();

                return Json(new { success = true, message = "Gửi đánh giá thành công!" });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Không thể lưu đánh giá." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> CreateRecipe()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var category = await _categoryService.ListAsync();
            var iIngredientTag = await _ingredientTagService.ListAsync();
            var typeOfDish = await _typeOfDishService.ListAsync();
            var model = new RecipeViewModels
            {
                CateID = category.First().ID,
                UserID = user.Id,
                Categories = category.ToList(), // truyền danh mục vào ViewModel
                IngredientTags = iIngredientTag.ToList(),
                typeOfDishes = typeOfDish.ToList(),
                SelectedIngredientTags = new List<Guid>() // Khởi tạo danh sách rtyj
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipe(RecipeViewModels obj, IFormFile ThumbnailImage)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var category = await _categoryService.ListAsync();
            var firstCategory = category.FirstOrDefault();
            var typeOfDish = await _typeOfDishService.ListAsync();
            string? imagePath = null;

            if (ThumbnailImage != null && ThumbnailImage.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ThumbnailImage.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ThumbnailImage.CopyToAsync(stream);
                }

                imagePath = "/uploads/" + fileName; // Đường dẫn lưu trong DB
            }


            var recipe = new Recipe
            {
                ID = Guid.NewGuid(),
                UserID = user.Id,
                Title = obj.Title,
                CookingStep = !string.IsNullOrEmpty(obj.CookingStep) ? obj.CookingStep : "<p>No cooking steps provided</p>", // Giá trị mặc định
                ShortDescriptions = obj.ShortDescriptions,
                PreparationTime = obj.PreparationTime ?? "",
                CookTime = obj.CookTime ?? "",
                TotalTime = obj.TotalTime ?? "",
                DifficultyLevel = obj.DifficultyLevel,
                Ingredient = !string.IsNullOrEmpty(obj.Ingredient) ? obj.Ingredient : "<p>No ingredients provided</p>", // Giá trị mặc định
                Servings = obj.Servings,
                CreatedDate = DateTime.Now,
                IsActive = true,
                CateID = firstCategory.ID,
                TypeOfDishID = obj.TypeOfDishID,
                ThumbnailImage = imagePath,
            };
            await _recipeService.AddAsync(recipe);
            // Lưu các IngredientTag được chọn vào bảng liên kết
            if (obj.SelectedIngredientTags != null && obj.SelectedIngredientTags.Any())
            {
                foreach (var tagId in obj.SelectedIngredientTags)
                {
                    // Tạo bản ghi cho bảng liên kết
                    var recipeIngredientTag = new RecipeIngredientTag
                    {
                        RecipeID = recipe.ID,
                        IngredientTagID = tagId
                    };
                    await _recipeIngredientTagIngredientTagIngredientTagSerivce.AddAsync(recipeIngredientTag);
                }
            }
            await _recipeService.SaveChangesAsync();
            return RedirectToAction("ViewRecipe", "Users");
        }
        [HttpPost]
        public async Task<IActionResult> SubmitComplaint([FromForm] ComplaintViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để gửi khiếu nại." });
                }

                var allowedExtensions = new[]
                {
       ".xbm", ".tif", ".tiff", ".jfif", ".pjp", ".apng", ".jpeg",
       ".heif", ".ico", ".webp", ".svgz", ".jpg", ".heic", ".gif",
       ".svg", ".png", ".bmp", ".pjpeg", ".avif"
   };

                const long maxFileSize = 5 * 1024 * 1024; // 5MB

                if (string.IsNullOrWhiteSpace(model.Description))
                {
                    return Json(new { success = false, message = "Vui lòng nhập mô tả khiếu nại." });
                }

                if (model.Images == null || model.Images.Count == 0)
                {
                    return Json(new { success = false, message = "Vui lòng chọn ít nhất 1 ảnh." });
                }

                if (model.Images.Count > 3)
                {
                    return Json(new { success = false, message = "Chỉ được chọn tối đa 3 ảnh." });
                }

                var orderDetails = await _orderDetailService.FindAsync(d => d.ID == model.OrderDetailID);
                if (orderDetails == null)
                    return Json(new { success = false, message = "Đơn hàng không có sản phẩm nào." });
                foreach (var file in model.Images)
                {
                    var fileExt = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(fileExt))
                    {
                        return Json(new { success = false, message = $"Ảnh {file.FileName} không đúng định dạng cho phép." });
                    }

                    if (file.Length > maxFileSize)
                    {
                        return Json(new { success = false, message = $"Ảnh {file.FileName} vượt quá dung lượng 5MB." });
                    }
                }

                // Tạo khiếu nại
                var complaint = new Complaint
                {
                    ID = Guid.NewGuid(),
                    Description = model.Description,
                    Status = "Pending",
                    OrderDetailID = model.OrderDetailID,
                    CreatedDate = DateTime.Now
                };

                await _complaintService.AddAsync(complaint);

                foreach (var file in model.Images)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var savePath = Path.Combine("wwwroot", "uploads", "complaints", fileName);

                    var folder = Path.GetDirectoryName(savePath);
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var complaintImage = new ComplaintImage
                    {
                        ID = Guid.NewGuid(),
                        ComplaintID = complaint.ID,
                        ImageUrl = $"/uploads/complaints/{fileName}"

                    };

                    await _complaintImageServices.AddAsync(complaintImage);
                }

                await _complaintService.SaveChangesAsync();

                return Json(new { success = true, message = "Gửi khiếu nại thành công." });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Đã xảy ra lỗi khi gửi khiếu nại. Vui lòng thử lại sau.", detail = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> ViewRecipe(int? page)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.UserId = user?.Id;

            int pageNumber = page ?? 1;  // Trang hiện tại, nếu null thì lấy 1
            int pageSize = 5;            // Số món mỗi trang
            var list = new List<RecipeViewModels>();
            var obj = await _recipeService.ListAsync();
            foreach (var item in obj)
            {
                var typeOfDish = await _typeOfDishService.GetAsyncById(item.TypeOfDishID);

                var recipeViewModel = new RecipeViewModels
                {
                    ID = item.ID,
                    Title = item.Title,
                    ShortDescriptions = item.ShortDescriptions,
                    PreparationTime = item.PreparationTime,
                    CookTime = item.CookTime,
                    TotalTime = item.TotalTime,
                    DifficultyLevel = item.DifficultyLevel,
                    Servings = item.Servings,
                    CreatedDate = item.CreatedDate,
                    IsActive = item.IsActive,
                    CateID = item.CateID,
                    ThumbnailImage = item.ThumbnailImage,
                    TypeOfDishName = typeOfDish?.Name,
                    CookingStep = item.CookingStep, // Lấy tên, tránh null
                    Ingredient = item.Ingredient,

                };
                list.Add(recipeViewModel);
            }
            var pagedList = list.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }
        [HttpGet]
        public async Task<IActionResult> MyViewRecipe(int? page, string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int pageNumber = page ?? 1;
            int pageSize = 5;
            var list = new List<RecipeViewModels>();

            var obj = await _recipeService.ListAsync();
            foreach (var item in obj)
            {
                if (item.UserID == id) // Giả sử bạn có property `UserId` trong Recipe
                {
                    var typeOfDish = await _typeOfDishService.GetAsyncById(item.TypeOfDishID);

                    var recipeViewModel = new RecipeViewModels
                    {
                        ID = item.ID,
                        Title = item.Title,
                        ShortDescriptions = item.ShortDescriptions,
                        PreparationTime = item.PreparationTime,
                        CookTime = item.CookTime,
                        TotalTime = item.TotalTime,
                        DifficultyLevel = item.DifficultyLevel,
                        Servings = item.Servings,
                        CreatedDate = item.CreatedDate,
                        IsActive = item.IsActive,
                        CateID = item.CateID,
                        ThumbnailImage = item.ThumbnailImage,
                        TypeOfDishName = typeOfDish?.Name,
                        CookingStep = item.CookingStep,
                        Ingredient = item.Ingredient,
                    };
                    list.Add(recipeViewModel);
                }
            }

            var pagedList = list.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }


    }



}