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
using Microsoft.EntityFrameworkCore.Query;
using Models;
using Moq;
using Net.payOS;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Home_UserInformation_Test
{
    public class UserInformation_Test
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
            _expertRecipeServicesMock = new Mock<IExpertRecipeServices>();
            _recipeViewHistoryServicesMock = new Mock<IRecipeViewHistoryServices>();
            // Nếu RecipeSearchService cần được mock, bạn nên refactor thành interface IRecipeSearchService và mock nó
            var recipeSearchService = new RecipeSearchService(""); // Hoặc dùng Mock<IRecipeSearchService>()
            var hubContextMock = new Mock<IHubContext<ChatHub>>();

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
                 _expertRecipeServicesMock.Object, // <-- Add this argument
 _recipeViewHistoryServicesMock.Object,
                 hubContextMock.Object
            // <-- Add this argument
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
        public async Task TC01_ValidUserId_ShouldReturnViewWithViewModel()
        {
            // Arrange
            var userId = "8e91c798-bc78-46a9-89a4-5d0aaea77f5f";
            var user = new AppUser
            {
                Id = userId,
                UserName = "testuser",
                JoinedDate = DateTime.Now,
                ImageUrl = "test.jpg"
            };

            var store = new StoreDetails { ID = Guid.NewGuid(), Name = "Test Store", UserID = userId };
            var product = new Product { ID = Guid.NewGuid(), StoreDetails = store, IsActive = true };
            var recipe = new Recipe { UserID = userId };
            var productType = new ProductTypes { Product = product };
            var order = new Order { UserID = userId };
            var orderDetail = new OrderDetail { ProductTypes = productType, Order = order, Quantity = 5 };

            _userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);
            _storeDetailServiceMock.Setup(s => s.GetStoreByUserIdAsync(userId)).ReturnsAsync(store);
            _recipeServiceMock.Setup(r => r.ListAsync()).ReturnsAsync(new List<Recipe> { recipe });
            _productServiceMock.Setup(p => p.ListAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                null,
                It.IsAny<Func<IQueryable<Product>, IIncludableQueryable<Product, object>>>()))
                .ReturnsAsync(new List<Product> { product });

            _orderDetailServiceMock.Setup(o => o.ListAsync(
                null,
                null,
                It.IsAny<Func<IQueryable<OrderDetail>, IIncludableQueryable<OrderDetail, object>>>()))
                .ReturnsAsync(new List<OrderDetail> { orderDetail });

            _ordersServiceMock.Setup(o => o.ListAsync()).ReturnsAsync(new List<Order> { order });

            // Act
            var result = await _controller.UserInformation(userId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as SellerViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(userId, model.UserId);
            Assert.AreEqual("Test Store", model.StoreName);
            Assert.AreEqual("1 Orders", model.ProductPurchased);
        }
        [Test]
        public async Task TC02_InvalidUserId_ShouldReturnNotFound()
        {
            // Arrange
            var invalidId = "8e91c798-bc78-46a9-89a4-5d0aaea77abc";
            _userManagerMock.Setup(m => m.FindByIdAsync(invalidId)).ReturnsAsync((AppUser)null);

            // Act
            var result = await _controller.UserInformation(invalidId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFound = result as NotFoundObjectResult;
            Assert.AreEqual("User not found.", notFound.Value);
        }
        [Test]
        public async Task TC03_ServerException_ShouldReturn500()
        {
            // Arrange
            var userId = "8e91c798-bc78-46a9-89a4-5d0aaea77f5f";
            _userManagerMock.Setup(m => m.FindByIdAsync(userId)).ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _controller.UserInformation(userId);

            // Assert
            var statusResult = result as ObjectResult;
            Assert.IsNotNull(statusResult);
            Assert.AreEqual(500, statusResult.StatusCode);
            StringAssert.Contains("Sever error", statusResult.Value.ToString());
        }

    }
}
