using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.RecipeServices;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using BusinessLogic.Services.StoreFollowers;
using BusinessLogic.Services.StoreReports;
using BusinessLogic.Services.VoucherServices;
using BusinessLogic.Services.Wishlists;
using Food_Haven.Web.Controllers;
using Food_Haven.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
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

namespace Food_Haven.UnitTest.Home_GetAllStore_Test
{
    public class GetAllStore_Test
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
                recipeSearchService
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
        public async Task TC01_SearchWithKeyword_ShouldReturnViewWithList()
        {
            // Arrange
            var stores = new List<StoreDetails>
    {
        new StoreDetails { ID = Guid.NewGuid(), Name = "Test Store", IsActive = true, CreatedDate = DateTime.Now }
    };

            _storeDetailServiceMock.Setup(s => s.ListAsync(
                It.IsAny<Expression<Func<StoreDetails, bool>>>(),
                It.IsAny<Func<IQueryable<StoreDetails>, IOrderedQueryable<StoreDetails>>>(),
                null)).ReturnsAsync(stores);

            // Act
            var result = await _controller.GetAllStore("store") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as List<StoreViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("Test Store", model[0].Name);
        }
        [Test]
        public async Task TC02_NullSearch_ShouldReturnFullList()
        {
            // Arrange
            var stores = new List<StoreDetails>
    {
        new StoreDetails { ID =Guid.NewGuid(), Name = "Another Store", IsActive = true, CreatedDate = DateTime.Now }
    };

            _storeDetailServiceMock.Setup(s => s.ListAsync(
                It.IsAny<Expression<Func<StoreDetails, bool>>>(),
                It.IsAny<Func<IQueryable<StoreDetails>, IOrderedQueryable<StoreDetails>>>(),
                null)).ReturnsAsync(stores);

            // Act
            var result = await _controller.GetAllStore(null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as List<StoreViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
        }
        [Test]
        public async Task TC03_KeywordNotMatch_ShouldReturnEmptyList()
        {
            // Arrange
            _storeDetailServiceMock.Setup(s => s.ListAsync(
                It.IsAny<Expression<Func<StoreDetails, bool>>>(),
                It.IsAny<Func<IQueryable<StoreDetails>, IOrderedQueryable<StoreDetails>>>(),
                null)).ReturnsAsync(new List<StoreDetails>());

            // Act
            var result = await _controller.GetAllStore("NotFound") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as List<StoreViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Count);
        }
        [Test]
        public async Task TC04_ExceptionThrown_ShouldReturnEmptyList()
        {
            // Arrange
            _storeDetailServiceMock.Setup(s => s.ListAsync(
                It.IsAny<Expression<Func<StoreDetails, bool>>>(),
                It.IsAny<Func<IQueryable<StoreDetails>, IOrderedQueryable<StoreDetails>>>(),
                null)).ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetAllStore("abc") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as List<StoreViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Count); // controller trả View(list), list = new List<StoreViewModel>()
        }

    }
}
