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
using BusinessLogic.Services.RecipeIngredientTagIngredientTagServices;
using System.Drawing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using BusinessLogic.Services.MessageImages;
using BusinessLogic.Services.Message; // nhớ import
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BusinessLogic.Services.RecipeReviewReviews;
using BusinessLogic.Services.FavoriteFavoriteRecipes; // nhớ import

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
        private readonly IMessageImageService _messageImageService;
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IRecipeReviewService _recipeReviewService;
        private readonly IFavoriteRecipeService _iFavoriteRecipe;

        public UsersController(UserManager<AppUser> userManager, HttpClient client, IBalanceChangeService balance, IHttpContextAccessor httpContextAccessor, IProductService product, ICartService cart, IProductVariantService productWarian, IProductImageService img, IOrdersServices order, IOrderDetailService orderDetailService, PayOS payos, ManageTransaction managetrans, IReviewService review, IRecipeService recipeService, ICategoryService categoryService, IIngredientTagService ingredientTagService, ITypeOfDishService typeOfDishService, IComplaintImageServices complaintImageServices, IComplaintServices complaintService, IRecipeIngredientTagIngredientTagSerivce recipeIngredientTagIngredientTagIngredientTagSerivce, IMessageImageService messageImageService, IMessageService messageService, IHubContext<ChatHub> hubContext, IRecipeReviewService recipeReviewService, IFavoriteRecipeService iFavoriteRecipe)
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
            _messageImageService = messageImageService;
            _messageService = messageService;
            _hubContext = hubContext;
            _recipeReviewService = recipeReviewService;
            _iFavoriteRecipe = iFavoriteRecipe;
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
        public async Task<IActionResult> Buy([FromForm] List<Guid> productIds, [FromForm] string voucherCode)
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
                    productID = product.ID,

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
            if (!string.IsNullOrWhiteSpace(voucherCode))
            {
                temInfo.voucher = voucherCode;
            }

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
                var fileName = Guid.NewGuid().ToString() + ".jpg";
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = ThumbnailImage.OpenReadStream())
                using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(stream))
                {
                    image.Mutate(x => x.Resize(1280, 720)); // Resize ảnh

                    using (var outStream = new FileStream(filePath, FileMode.Create))
                    {
                        var encoder = new JpegEncoder { Quality = 85 }; // Chất lượng JPEG
                        image.Save(outStream, encoder); // Lưu ảnh xuống file
                    }
                }

                imagePath = "/uploads/" + fileName;
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
                IsActive = obj.IsActive,
                CateID = obj.CateID,
                TypeOfDishID = obj.TypeOfDishID,
                ThumbnailImage = imagePath,
                status = "Pending", // Trạng thái mặc định
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
        [AllowAnonymous]
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
                if (item.IsActive == true && item.status.ToUpper() == "Accept".ToUpper())
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
            }
            var pagedList = list.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }
        [HttpGet]
        public async Task<IActionResult> MyViewRecipe(int? page)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Home");

            int pageNumber = page ?? 1;
            int pageSize = 5;
            var recipes = new List<RecipeViewModels>();

            var allRecipes = await _recipeService.ListAsync();
            var ingredientTags = await _ingredientTagService.ListAsync();

            foreach (var item in allRecipes.Where(r => r.UserID == user.Id))
            {
                var typeOfDish = await _typeOfDishService.GetAsyncById(item.TypeOfDishID);


                var selectedTags = (await _recipeIngredientTagIngredientTagIngredientTagSerivce
                      .ListAsync(rt => rt.RecipeID == item.ID, null, include => include.Include(x => x.IngredientTag)))
                      .ToList();
                recipes.Add(new RecipeViewModels
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
                    TypeOfDishID = item.TypeOfDishID,
                    UserID = item.UserID,
                    IngredientTags = selectedTags.Select(x => x.IngredientTag).ToList(),
                    SelectedIngredientTags = selectedTags.Select(x => x.IngredientTagID).ToList()

                });
            }

            var viewModel = new MyViewRecipePageViewModel
            {
                Recipes = recipes.ToPagedList(pageNumber, pageSize),
                Categories = (await _categoryService.ListAsync()).ToList(),
                TypeOfDishes = (await _typeOfDishService.ListAsync()).ToList(),
                IngredientTags = ingredientTags.ToList(),
            };

            return View(viewModel);
        }



        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> RecipeDetail(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            var recipe = await _recipeService.GetAsyncById(id); // Tối ưu thay vì duyệt List
            if (recipe == null) return NotFound();

            var typeOfDish = await _typeOfDishService.GetAsyncById(recipe.TypeOfDishID);

            // Lấy tất cả review theo RecipeID
            var reviews = await _recipeReviewService.ListAsync(x => x.RecipeID == id);
            var reviewViewModel = new List<RecipeReviewViewModel>();
            var recipeOwner = await _userManager.FindByIdAsync(recipe.UserID);

            foreach (var r in reviews)
            {
                var reviewer = await _userManager.FindByIdAsync(r.UserID);
                reviewViewModel.Add(new RecipeReviewViewModel
                {
                    ID = r.ID,
                    Comment = r.Comment,
                    Reply = r.Reply,
                    CreatedDate = r.CreatedDate,
                    ReplyDate = r.ReplyDate,
                    Rating = r.Rating,
                    IsActive = r.IsActive,
                    RecipeID = r.RecipeID,
                    UserID = reviewer?.UserName ?? "Ẩn danh"
                });
            }

            var recipeViewModel = new RecipeViewModels
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
                TypeOfDishName = typeOfDish?.Name,
                CookingStep = recipe.CookingStep,
                Ingredient = recipe.Ingredient,
                Username = user?.UserName,
                Email = user?.Email,
                RecipeReviewViewModels = reviewViewModel,
                RecipeOwnerUserName = recipeOwner?.UserName ?? "Ẩn danh"

            };

            return View(recipeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> HideRecipe(Guid ID, bool isActive)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Please log in." });
                }

                var recipee = await _recipeService.FindAsync(x => x.ID == ID && x.UserID == user.Id);
                if (recipee == null)
                {
                    return Json(new { success = false, message = "Recipe not found." });
                }

                recipee.IsActive = isActive;
                await _recipeService.UpdateAsync(recipee);
                await _recipeService.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = isActive ? "Recipe is now visible." : "Recipe has been hidden."
                });
            }
            catch (Exception ex)
            {
                // Ghi log nếu cần, ví dụ: _logger.LogError(ex, "Error in HideRecipe");
                return Json(new
                {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRecipe(RecipeViewModels obj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Home");

            // if (!ModelState.IsValid)
            // {
            //     obj.Categories = (await _categoryService.ListAsync()).ToList();
            //     obj.typeOfDishes = (await _typeOfDishService.ListAsync()).ToList();
            //     obj.IngredientTags = (await _ingredientTagService.ListAsync()).ToList();
            //     return RedirectToAction("MyViewRecipe");
            // }

            var existingRecipe = await _recipeService.GetAsyncById(obj.ID);
            if (existingRecipe == null || existingRecipe.UserID != user.Id) return NotFound();

            // Cập nhật các trường cơ bản
            existingRecipe.Title = obj.Title;
            existingRecipe.ShortDescriptions = obj.ShortDescriptions;
            existingRecipe.PreparationTime = obj.PreparationTime;
            existingRecipe.CookTime = obj.CookTime;
            existingRecipe.TotalTime = obj.TotalTime;
            existingRecipe.DifficultyLevel = obj.DifficultyLevel;
            existingRecipe.Servings = obj.Servings;
            existingRecipe.CateID = obj.CateID;
            existingRecipe.TypeOfDishID = obj.TypeOfDishID;
            existingRecipe.CookingStep = obj.CookingStep;
            existingRecipe.Ingredient = obj.Ingredient;
            existingRecipe.ModifiedDate = DateTime.Now;
            existingRecipe.IsActive = obj.IsActive;
            existingRecipe.status = "Pending"; // Giữ nguyên trạng thái Pending
            // XỬ LÝ ẢNH
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    // Xóa ảnh cũ nếu tồn tại
                    if (!string.IsNullOrEmpty(existingRecipe.ThumbnailImage))
                    {
                        var oldImagePath = Path.Combine(uploadsDir, Path.GetFileName(existingRecipe.ThumbnailImage));
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    var newFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var newFilePath = Path.Combine(uploadsDir, newFileName);

                    using (var stream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    existingRecipe.ThumbnailImage = "/uploads/" + newFileName;
                }
            }

            await _recipeService.UpdateAsync(existingRecipe);

            // XỬ LÝ TAGS
            var oldTags = await _recipeIngredientTagIngredientTagIngredientTagSerivce
                            .ListAsync(t => t.RecipeID == obj.ID);
            foreach (var oldTag in oldTags)
            {
                await _recipeIngredientTagIngredientTagIngredientTagSerivce.DeleteAsync(oldTag);
            }

            if (obj.SelectedIngredientTags != null && obj.SelectedIngredientTags.Any())
            {
                foreach (var tagId in obj.SelectedIngredientTags)
                {
                    var newTag = new RecipeIngredientTag
                    {
                        RecipeID = obj.ID,
                        IngredientTagID = tagId
                    };
                    await _recipeIngredientTagIngredientTagIngredientTagSerivce.AddAsync(newTag);
                    await _recipeIngredientTagIngredientTagIngredientTagSerivce.SaveChangesAsync();
                }
            }

            // ✅ Chuyển về trang danh sách sau khi update thành công
            return RedirectToAction("MyViewRecipe");
        }



        public async Task<IActionResult> ChatList()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetUserList()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var currentUserId = user.Id;
            var adminId = "af32f202-1cb6-4191-8293-00da0aae4d2d";

            // Lấy tất cả tin nhắn 1 lần
            var allMessages = _messageService.GetAll();

            // Lấy danh sách các user đã nhắn tin với mình
            var chatUserIds = allMessages
                .Where(m => m.FromUserId == currentUserId || m.ToUserId == currentUserId)
                .Select(m => m.FromUserId == currentUserId ? m.ToUserId : m.FromUserId)
                .Distinct()
                .ToList();

            // Luôn hiển thị admin (nếu khác current user)
            if (adminId != currentUserId && !chatUserIds.Contains(adminId))
            {
                chatUserIds.Insert(0, adminId);
            }

            // Thời điểm hiện tại
            var now = DateTime.Now;

            var users = _userManager.Users
      .Where(u => chatUserIds.Contains(u.Id) && u.Id != currentUserId)
      .Select(u => new
      {
          id = u.Id,
          name = u.UserName,
          profile = u.ImageUrl,
          nickname = u.UserName,
          status = ChatHub.IsUserOnline(u.Id) ? "online" : "offline",
          lastSeen = u.LastAccess.ToString("yyyy-MM-dd HH:mm:ss"),
          messagecount = allMessages.Count(m => m.FromUserId == u.Id && m.ToUserId == currentUserId && !m.IsRead)
      })
      .ToList();

            return Json(users);
        }


        [HttpGet]
        public async Task<IActionResult> GetChatHistory(string userId, string otherId)
        {
            var query = _messageService.GetAll()
                .Where(m => (m.FromUserId == userId && m.ToUserId == otherId) ||
                           (m.FromUserId == otherId && m.ToUserId == userId))
                .Include(m => m.Images)
                .OrderBy(m => m.SentAt);

            var unreadMessages = query
                .Where(m => m.ToUserId == userId && !m.IsRead)
                .ToList();

            foreach (var msg in unreadMessages)
            {
                msg.IsRead = true;
                msg.ReadAt = DateTime.Now;
                await _messageService.UpdateAsync(msg);
            }

            if (unreadMessages.Any())
            {
                await _messageService.SaveChangesAsync();

                // Thông báo cho người gửi rằng tin nhắn đã được đọc
                var readMessageIds = unreadMessages.Select(m => m.ID.ToString()).ToList();
                await _hubContext.Clients.User(otherId).SendAsync("MessagesRead", readMessageIds, userId);
            }

            var messages = query
                .Select(m => new
                {
                    id = m.ID,
                    from_id = m.FromUserId,
                    to_id = m.ToUserId,
                    msg = m.MessageText,
                    has_dropDown = true,
                    datetime = m.SentAt.ToString("hh:mm tt"),
                    isReplied = m.RepliedToMessageId ?? Guid.Empty,
                    is_read = m.IsRead,
                    has_images = m.Images != null && m.Images.Any() ? m.Images.Select(i => i.ImageUrl).ToList() : null,
                })
                .ToList();

            return Json(new { chats = messages });
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageModel model)
        {
            var getGui = Guid.TryParse(model.id + "", out var guid) ? guid : Guid.NewGuid();


            var message = new Message
            {
                ID = getGui,
                FromUserId = model.from_id,
                ToUserId = model.to_id,
                MessageText = model.msg,
                SentAt = DateTime.Now,
                HasDropDown = true,
                IsRead = false,
                RepliedToMessageId = model.isReplied,
                Images = model.has_images?.Select(url => new MessageImage { ID = Guid.NewGuid(), ImageUrl = url }).ToList()
            };

            await _messageService.AddAsync(message);
            await _messageService.SaveChangesAsync();

            // Lấy thông tin người gửi
            var fromUser = await _userManager.FindByIdAsync(model.from_id);

            // Tạo dữ liệu tin nhắn để gửi qua SignalR
            var messageData = new
            {
                id = message.ID,
                from_id = message.FromUserId,
                to_id = message.ToUserId,
                msg = message.MessageText,
                has_dropDown = message.HasDropDown,
                datetime = message.SentAt.ToString("hh:mm tt"),
                isReplied = message.RepliedToMessageId,
                is_read = message.IsRead,
                has_images = message.Images?.Select(i => i.ImageUrl).ToList(),
                senderName = fromUser?.UserName ?? "Unknown",
                senderAvatar = fromUser?.ImageUrl
            };

            // Gửi tin nhắn qua SignalR
            await _hubContext.Clients.User(model.to_id).SendAsync("ReceiveMessage", messageData);

            // Gửi confirm cho người gửi
            await _hubContext.Clients.User(model.from_id).SendAsync("MessageSent", messageData);

            return Json(messageData);
        }
        [HttpGet]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            var unread = _messageService.GetAll().Count(m => m.FromUserId == user.Id && m.ToUserId == currentUserId && !m.IsRead);

            return Json(new
            {
                id = user.Id,
                name = user.UserName,
                profile = user.ImageUrl,
                nickname = user.UserName,
                messagecount = unread
            });
        }

        public class ChatMessageModel
        {
            public object id { get; set; }          // Guid hoặc int đều được, miễn khớp js
            public string from_id { get; set; }     // string (Guid) hoặc int
            public string to_id { get; set; }
            public string msg { get; set; }
            public bool has_dropDown { get; set; }
            public string datetime { get; set; }
            public Guid? isReplied { get; set; }   // Guid hoặc int, tùy hệ thống
            public List<string> has_images { get; set; } = new List<string>();
        }

        [HttpPost]
        public async Task<IActionResult> addCommentAndStart([FromBody] RecipeReviewViewModel obj)

        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { success = false, message = "You are not logged in." });
                if (string.IsNullOrEmpty(obj.Comment))
                {
                    return Json(new { success = false, message = "Comment cannot be empty." });
                }
                var viewModel = new RecipeReview
                {
                    ID = Guid.NewGuid(),
                    Comment = obj.Comment,
                    Reply = null,
                    CreatedDate = DateTime.Now,
                    ReplyDate = null,
                    Rating = obj.Rating,
                    IsActive = true,
                    RecipeID = obj.RecipeID,
                    UserID = user.Id
                };

                await _recipeReviewService.AddAsync(viewModel);
                await _recipeReviewService.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Your comment has been posted successfully.",
                    username = user.UserName,
                    comment = obj.Comment,
                    createdDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> RelyComment([FromBody] RecipeReviewViewModel obj)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { success = false, message = "You must be logged in." });

                var rely = await _recipeReviewService.FindAsync(x => x.ID == obj.ID);
                if (rely == null)
                    return Json(new { success = false, message = "Comment not found." });

                // 👉 Nếu không phải sửa mà đã có reply → chặn
                if (!string.IsNullOrEmpty(rely.Reply) && !obj.IsEdit)
                {
                    return Json(new { success = false, message = "This comment has already been replied to." });
                }

                // Cập nhật nội dung trả lời và ngày
                rely.Reply = obj.Reply;
                rely.ReplyDate = DateTime.Now;

                await _recipeReviewService.UpdateAsync(rely);
                await _recipeReviewService.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = obj.IsEdit ? "Reply edited successfully." : "Reply sent successfully.",
                    username = user.UserName
                });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while processing the reply." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> FavoriteRecipes(int? page)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Home");

            try
            {
                var list = new List<FavoriteRecipeViewModel>();
                var obj = await _iFavoriteRecipe.ListAsync();
                var userFavorites = obj.Where(f => f.UserID == user.Id);

                foreach (var item in userFavorites)
                {

                    var recipe = await _recipeService.FindAsync(a => a.ID == item.RecipeID);
                    var typeOfDish = await _typeOfDishService.GetAsyncById(recipe.TypeOfDishID);

                    if (recipe == null) continue;

                    var viewModel = new FavoriteRecipeViewModel
                    {
                        ID = item.ID,
                        CreatedDate = item.CreatedDate,
                        RecipeID = item.RecipeID,
                        UserID = item.UserID,
                        ThumbnailImage = recipe.ThumbnailImage,
                        Title = recipe.Title,
                        DifficultyLevel = recipe.DifficultyLevel,
                        status = recipe.status,
                        ShortDescriptions = recipe.ShortDescriptions,
                        TypeOfDishName = typeOfDish?.Name ?? "Unknown"

                    };
                    list.Add(viewModel);
                }

                int pageSize = 10;
                int pageNumber = page ?? 1;

                var pagedList = list.ToPagedList(pageNumber, pageSize);
                return View(pagedList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddFavoriteRecipes([FromBody] FavoriteRecipeViewModel obj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Please login to add favorite." });
            var isExist = await _iFavoriteRecipe.FindAsync(x => x.RecipeID == obj.RecipeID && x.UserID == user.Id);
            if (isExist != null)
            {
                return Json(new { success = false, message = "You have already favorited this recipe." });
            }
            try
            {
                var viewModel = new FavoriteRecipe
                {
                    ID = Guid.NewGuid(),
                    CreatedDate = DateTime.UtcNow,
                    RecipeID = obj.RecipeID,
                    UserID = user.Id
                };

                await _iFavoriteRecipe.AddAsync(viewModel);
                await _iFavoriteRecipe.SaveChangesAsync();

                return Json(new { success = true, message = "Added to favorites." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteFavoriteRecipe([FromBody] Guid id)
        {
            try
            {
                var favorite = await _iFavoriteRecipe.FindAsync(f => f.ID == id);
                if (favorite == null)
                    return Json(new { success = false, message = "Favorite not found." });

                await _iFavoriteRecipe.DeleteAsync(favorite);
                await _iFavoriteRecipe.SaveChangesAsync();

                return Json(new { success = true, message = "Favorite removed successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



    }


}



