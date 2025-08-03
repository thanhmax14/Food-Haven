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
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Home_AddToCart_Test
{
    public class AddToCart_Test
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

            // Nếu RecipeSearchService cần được mock, bạn nên refactor thành interface IRecipeSearchService và mock nó
            var recipeSearchService = new RecipeSearchService(""); // Hoặc dùng Mock<IRecipeSearchService>()

            var payOS = new PayOS("client-id", "api-key", "https://callback.url");
            var hubContextMock = new Mock<IHubContext<ChatHub>>(); // Add this line

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
        [Test]
        public async Task TC01_ProductVariantNotFound_ShouldReturnError()
        {
            var user = new AppUser { Id = Guid.NewGuid()+"" };
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            var vm = new CartViewModels { ProductTypeID = Guid.NewGuid(), quantity = 5 };
            _productVariantServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>()))
                .ReturnsAsync((ProductTypes)null);

            var result = await _controller.AddToCart(vm) as JsonResult;
            var json = JObject.FromObject(result.Value);

            Assert.IsFalse(json["success"]!.Value<bool>());
            Assert.AreEqual("Product does not exist!", json["message"]!.Value<string>());
        }

        [Test]
        public async Task TC02_QuantityExceedsStock_ShouldReturnError()
        {
            var user = new AppUser { Id = Guid.NewGuid() + "" };
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            var variant = new ProductTypes { ID = Guid.NewGuid(), Stock = 5 };
            _productVariantServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>()))
                .ReturnsAsync(variant);

            var vm = new CartViewModels { ProductTypeID = variant.ID, quantity = 6 };
            var result = await _controller.AddToCart(vm) as JsonResult;
            var json = JObject.FromObject(result.Value);

            Assert.IsFalse(json["success"]!.Value<bool>());
            Assert.AreEqual("Quantity exceeds stock! Only 5 items left.", json["message"]!.Value<string>());
        }

        [Test]
        public async Task TC03_TotalQuantityExceedsStock_ShouldReturnError()
        {
            var user = new AppUser { Id = Guid.NewGuid() + "" };
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            var variant = new ProductTypes { ID = Guid.NewGuid(), Stock = 10 };
            var existingCart = new Cart { ProductTypesID = variant.ID, Quantity = 5 };

            _productVariantServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>()))
                .ReturnsAsync(variant);
            _cartServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Cart, bool>>>()))
                .ReturnsAsync(existingCart);

            var vm = new CartViewModels { ProductTypeID = variant.ID, quantity = 6 };
            var result = await _controller.AddToCart(vm) as JsonResult;
            var json = JObject.FromObject(result.Value);

            Assert.IsFalse(json["success"]!.Value<bool>());
            Assert.AreEqual("Quantity exceeds stock! Only 10 items available.", json["message"]!.Value<string>());
        }

        [Test]
        public async Task TC04_AddNewToCart_Success()
        {
            var user = new AppUser { Id = Guid.NewGuid() + "" };
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            var variant = new ProductTypes { ID = Guid.NewGuid(), Stock = 10 };

            _productVariantServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>()))
                .ReturnsAsync(variant);
            _cartServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Cart, bool>>>()))
                .ReturnsAsync((Cart)null);
            _cartServiceMock.Setup(x => x.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
            _cartServiceMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);       
            var mockHubContext = new Mock<IHubContext<CartHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockUserClient = new Mock<IClientProxy>();

            mockHubContext.Setup(x => x.Clients).Returns(mockClients.Object);
            mockClients.Setup(x => x.User(It.IsAny<string>())).Returns(mockUserClient.Object);
            mockUserClient.Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IHubContext<CartHub>)))
                .Returns(mockHubContext.Object);
            var context = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            var vm = new CartViewModels { ProductTypeID = variant.ID, quantity = 2 };
            var result = await _controller.AddToCart(vm) as JsonResult;
            var json = JObject.FromObject(result!.Value);

            Assert.IsTrue(json["success"]!.Value<bool>());
            Assert.AreEqual("Added to cart successfully!", json["message"]!.Value<string>());
        }
        [Test]
        public async Task TC05_ExceptionThrown_ShouldReturnInternalServerError()
        {
            // Arrange
            var user = new AppUser { Id = "test-user-id" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _productVariantServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>())).ReturnsAsync(new ProductTypes
            {
                ID = Guid.NewGuid(),
                Stock = 10
            });

            _cartServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Cart, bool>>>())).ReturnsAsync((Cart)null);

            // Simulate exception when adding to cart
            _cartServiceMock.Setup(x => x.AddAsync(It.IsAny<Cart>())).ThrowsAsync(new Exception("Unexpected error"));

            var model = new CartViewModels
            {
                ProductTypeID = Guid.NewGuid(),
                quantity = 1
            };

            // Act + Assert
            var ex =  Assert.ThrowsAsync<Exception>(() => _controller.AddToCart(model));

            Assert.AreEqual("Unexpected error", ex.Message);
        }



    }
}
