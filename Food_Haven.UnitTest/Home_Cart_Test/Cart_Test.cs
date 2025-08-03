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
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Home_Cart_Test
{
    public class Cart_Test
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
                 _expertRecipeServicesMock.Object, // <-- Add this argument
 _recipeViewHistoryServicesMock.Object,
 hubContextMock.Object// <-- Add this argument
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
        public async Task Cart_UTCD01_ReturnsViewWithData()
        {
            // Arrange
            var user = new AppUser { Id = "user123" };

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var productTypeId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var storeId = Guid.NewGuid();

            // Mock CartService
            _cartServiceMock.Setup(m => m.ListAsync(
                    It.IsAny<Expression<Func<Models.Cart, bool>>>(),
                    null,
                    null))
                .ReturnsAsync(new List<Models.Cart>
                {
            new Models.Cart { ID = Guid.NewGuid(), UserID = user.Id, ProductTypesID = productTypeId, Quantity = 2 }
                });

            // Mock ProductVariants
            _productVariantServiceMock.Setup(m => m.ListAsync(
                    It.IsAny<Expression<Func<Models.ProductTypes, bool>>>(),
                    null,
                    null))
                .ReturnsAsync(new List<Models.ProductTypes>
                {
            new Models.ProductTypes { ID = productTypeId, ProductID = productId, SellPrice = 100, Stock = 10, IsActive = true, Name = "Loại A" }
                });

            // Mock Products
            _productServiceMock.Setup(m => m.ListAsync(
                    It.IsAny<Expression<Func<Models.Product, bool>>>(),
                    null,
                    null))
                .ReturnsAsync(new List<Models.Product>
                {
            new Models.Product { ID = productId, Name = "Bánh mì", StoreID = storeId }
                });

            // Mock StoreDetails
            _storeDetailServiceMock.Setup(m => m.ListAsync(
                    It.IsAny<Expression<Func<Models.StoreDetails, bool>>>(),
                    null,
                    null))
                .ReturnsAsync(new List<Models.StoreDetails>
                {
            new Models.StoreDetails { ID = storeId, Name = "Cửa hàng ABC" }
                });

            // Mock ProductImage
            _productImageServiceMock.Setup(m => m.FindAsync(
                    It.IsAny<Expression<Func<Models.ProductImage, bool>>>()))
                .ReturnsAsync(new Models.ProductImage { ImageUrl = "/images/bread.jpg" });

            // Act
            var result = await _controller.Cart();

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            var model = viewResult.Model as List<StoreCartViewModel>;
            Assert.NotNull(model);
            Assert.IsNotEmpty(model);
        }


        [Test]
        public async Task Cart_UTCD02_ReturnsEmptyList_WhenNoMatchingProductTypes()
        {
            // Arrange
            var user = new AppUser { Id = "user456" };
            var productTypeId = Guid.NewGuid();

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _cartServiceMock.Setup(m => m.ListAsync(
                It.IsAny<Expression<Func<Models.Cart, bool>>>(),
                null,
                null))
            .ReturnsAsync(new List<Models.Cart>
            {
        new Models.Cart
        {
            ID = Guid.NewGuid(),
            UserID = user.Id,
            ProductTypesID = productTypeId,
            Quantity = 1
        }
            });

            _productVariantServiceMock.Setup(m => m.ListAsync(
                It.IsAny<Expression<Func<Models.ProductTypes, bool>>>(),
                null,
                null))
            .ReturnsAsync(new List<Models.ProductTypes>()); // Không có ProductTypes khớp

            // Act
            var result = await _controller.Cart();

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            var model = viewResult.Model as List<StoreCartViewModel>;
            Assert.NotNull(model);
            Assert.IsEmpty(model); // Không có ProductTypes tương ứng => không hiển thị
        }




        [Test]
        public async Task Cart_UTCD03_ShouldReturnExceptionMessage()
        {
            // Arrange
            var user = new AppUser { Id = "user789" };

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _cartServiceMock.Setup(m => m.ListAsync(
                It.IsAny<Expression<Func<Models.Cart, bool>>>(),
                null,
                null)) // thêm các tham số null cho đúng với method signature
                .ThrowsAsync(new Exception("An unknown error occurred…"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _controller.Cart());
            Assert.AreEqual("An unknown error occurred…", ex.Message);
        }



    }
}
