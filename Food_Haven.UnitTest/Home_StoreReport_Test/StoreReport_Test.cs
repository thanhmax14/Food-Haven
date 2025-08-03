using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.ExpertRecipes;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.RecipeServices;
using BusinessLogic.Services.RecipeViewHistorys;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using BusinessLogic.Services.StoreFollowers;
using BusinessLogic.Services.StoreReports;
using BusinessLogic.Services.VoucherServices;
using BusinessLogic.Services.Wishlists;
using Food_Haven.Web.Controllers;
using Food_Haven.Web.Hubs;
using Food_Haven.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models;
using Moq;
using Net.payOS;
using Newtonsoft.Json.Linq;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Home_StoreReport_Test
{
    public class StoreReport_Test
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<SignInManager<AppUser>> _signInManagerMock;
        private Mock<ICategoryService> _categoryServiceMock;
        private Mock<IEmailSender> _emailSenderMock;
        private Mock<IOrderDetailService> _orderDetailServiceMock;
        private Mock<IRecipeService> _recipeServiceMock;
        private Mock<IStoreDetailService> _storeDetailServiceMock;
        private Mock<ICartService> _cartServiceMock;
        private Mock<IWishlistServices> _wishlistServiceMock;
        private Mock<IProductService> _productServiceMock;
        private Mock<IProductImageService> _productImageServiceMock;
        private Mock<IProductVariantService> _productVariantServiceMock;
        private Mock<IReviewService> _reviewServiceMock;
        private Mock<IBalanceChangeService> _balanceChangeServiceMock;
        private Mock<IOrdersServices> _ordersServiceMock;
        private Mock<IVoucherServices> _voucherServiceMock;
        private Mock<IStoreReportServices> _storeReportServiceMock;
        private Mock<IStoreFollowersService> _storeFollowersServiceMock;
        private Mock<IExpertRecipeServices> _expertRecipeServicesMock;
        private Mock<IRecipeViewHistoryServices> _recipeViewHistoryServicesMock;

        // Nếu muốn mock luôn RecipeSearchService, bạn cần tạo interface cho nó
        // private Mock<IRecipeSearchService> _recipeSearchServiceMock;

        private HomeController _controller;

        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(
                new Mock<IUserStore<AppUser>>().Object,
                null, null, null, null, null, null, null, null);

            _signInManagerMock = new Mock<SignInManager<AppUser>>(
                _userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
                null, null, null, null);

            _categoryServiceMock = new Mock<ICategoryService>();
            _emailSenderMock = new Mock<IEmailSender>();
            _orderDetailServiceMock = new Mock<IOrderDetailService>();
            _recipeServiceMock = new Mock<IRecipeService>();
            _storeDetailServiceMock = new Mock<IStoreDetailService>();
            _cartServiceMock = new Mock<ICartService>();
            _wishlistServiceMock = new Mock<IWishlistServices>();
            _productServiceMock = new Mock<IProductService>();
            _productImageServiceMock = new Mock<IProductImageService>();
            _productVariantServiceMock = new Mock<IProductVariantService>();
            _reviewServiceMock = new Mock<IReviewService>();
            _balanceChangeServiceMock = new Mock<IBalanceChangeService>();
            _ordersServiceMock = new Mock<IOrdersServices>();
            _voucherServiceMock = new Mock<IVoucherServices>();
            _storeReportServiceMock = new Mock<IStoreReportServices>();
            _storeFollowersServiceMock = new Mock<IStoreFollowersService>();
            var hubContextMock = new Mock<IHubContext<ChatHub>>();

            // Nếu RecipeSearchService cần được mock, bạn nên refactor thành interface IRecipeSearchService và mock nó
            var recipeSearchService = new RecipeSearchService(""); // Hoặc dùng Mock<IRecipeSearchService>()

            var payOS = new PayOS("client-id", "api-key", "https://callback.url");

            _controller = new HomeController(
                _signInManagerMock.Object,
                _orderDetailServiceMock.Object,
                _recipeServiceMock.Object,
                _userManagerMock.Object,
                _categoryServiceMock.Object,
                _storeDetailServiceMock.Object,
                _emailSenderMock.Object,
                _cartServiceMock.Object,
                _wishlistServiceMock.Object,
                _productServiceMock.Object,
                _productImageServiceMock.Object,
                _productVariantServiceMock.Object,
                _reviewServiceMock.Object,
                _balanceChangeServiceMock.Object,
                _ordersServiceMock.Object,
                payOS,
                _voucherServiceMock.Object,
                _storeReportServiceMock.Object,
                _storeFollowersServiceMock.Object,
                recipeSearchService,
                _expertRecipeServicesMock.Object,
                _recipeViewHistoryServicesMock.Object,
                hubContextMock.Object

            );

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }
        private JObject ParseJsonResult(JsonResult result)
        {
            return JObject.FromObject(result.Value!);
        }

        [Test]
        public async Task TC01_StoreReport_UserNotLoggedIn_ReturnsError()
        {
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                            .ReturnsAsync((AppUser)null);

            var model = new StoreReportViewModel
            {
                StoreID = Guid.NewGuid(),
                Reason = "Illegal products",
                Message = "Some issue"
            };

            var result = await _controller.StoreReport(model) as JsonResult;

            var json = ParseJsonResult(result!);
            Assert.IsFalse(json["success"]!.Value<bool>());
            Assert.AreEqual("You must be logged in to report.", json["message"]!.Value<string>());
        }

        [Test]
        public async Task TC02_StoreReport_EmptyOrInvalidReason_ReturnsError()
        {
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                            .ReturnsAsync(new AppUser { Id = "test-user-id" });

            var model = new StoreReportViewModel
            {
                StoreID = Guid.NewGuid(),
                Reason = "-- Select a reason --",
                Message = "Violation"
            };

            var result = await _controller.StoreReport(model) as JsonResult;

            var json = ParseJsonResult(result!);
            Assert.IsFalse(json["success"]!.Value<bool>());
            Assert.AreEqual("Please select a reason for reporting.", json["message"]!.Value<string>());
        }

        [Test]
        public async Task TC03_StoreReport_EmptyMessage_ReturnsError()
        {
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                            .ReturnsAsync(new AppUser { Id = "test-user-id" });

            var model = new StoreReportViewModel
            {
                StoreID = Guid.NewGuid(),
                Reason = "Illegal products",
                Message = ""
            };

            var result = await _controller.StoreReport(model) as JsonResult;

            var json = ParseJsonResult(result!);
            Assert.IsFalse(json["success"]!.Value<bool>());
            Assert.AreEqual("Message is required.", json["message"]!.Value<string>());
        }

        [Test]
        public async Task TC04_StoreReport_ValidInput_ReturnsSuccess()
        {
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                            .ReturnsAsync(new AppUser { Id = "test-user-id" });

            _storeReportServiceMock.Setup(x => x.AddAsync(It.IsAny<StoreReport>()))
                                   .Returns(Task.CompletedTask);
            _storeReportServiceMock.Setup(x => x.SaveChangesAsync())
                                   .ReturnsAsync(1);

            var model = new StoreReportViewModel
            {
                StoreID = Guid.NewGuid(),
                Reason = "Illegal products",
                Message = "Counterfeit goods"
            };

            var result = await _controller.StoreReport(model) as JsonResult;

            var json = ParseJsonResult(result!);
            Assert.IsTrue(json["success"]!.Value<bool>());
            Assert.AreEqual("Report submitted successfully.", json["message"]!.Value<string>());
        }

        [Test]
        public async Task TC05_StoreReport_ExceptionThrown_ReturnsServerError()
        {
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                            .ReturnsAsync(new AppUser { Id = "test-user-id" });

            _storeReportServiceMock.Setup(x => x.AddAsync(It.IsAny<StoreReport>()))
                                   .ThrowsAsync(new Exception("Some DB failure"));

            var model = new StoreReportViewModel
            {
                StoreID = Guid.NewGuid(),
                Reason = "Illegal products",
                Message = "Details"
            };

            var result = await _controller.StoreReport(model) as JsonResult;

            var json = ParseJsonResult(result!);
            Assert.IsFalse(json["success"]!.Value<bool>());
            Assert.AreEqual("An error occurred. Please try again later.", json["message"]!.Value<string>());
        }


    }
}
