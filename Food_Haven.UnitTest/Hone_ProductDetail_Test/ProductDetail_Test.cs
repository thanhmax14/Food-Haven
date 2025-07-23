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

namespace Food_Haven.UnitTest.Hone_ProductDetail_Test
{
    public class ProductDetail_Test
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

            var recipeSearchService = new RecipeSearchService("");
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
        public async Task TC01_ProductDetail_ValidId_ReturnsProductDetailsView()
        {
            // Arrange
            var productId = Guid.Parse("D7DFAEE2-9CFF-40F8-A7D8-3C630008B217");
            var categoryId = Guid.NewGuid();
            var storeId = Guid.NewGuid();

            var product = new Product
            {
                ID = productId,
                Name = "Test Product",
                IsActive = true,
                StoreID = storeId,
                CategoryID = categoryId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            var store = new StoreDetails { ID = storeId, IsActive = true, UserID = "user1" };
            var category = new Categories { ID = categoryId };
            var variant = new ProductTypes { ProductID = productId, IsActive = true };

            _productServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(product);

            _storeDetailServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync(store);

            _userManagerMock.Setup(x => x.FindByIdAsync("user1"))
                .ReturnsAsync(new AppUser { Id = "user1" });

            _categoryServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Categories, bool>>>()))
                .ReturnsAsync(category);

            _productImageServiceMock.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<ProductImage, bool>>>(),
                null,
                null))
                .ReturnsAsync(new List<ProductImage>());

            _productVariantServiceMock.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<ProductTypes, bool>>>(),
                null,
                null))
                .ReturnsAsync(new List<ProductTypes> { variant });

            _reviewServiceMock.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Review, bool>>>(),
                null,
                null))
                .ReturnsAsync(new List<Review>());

            _wishlistServiceMock.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Wishlist, bool>>>(),
                null,
                null))
                .ReturnsAsync(new List<Wishlist>());

            _orderDetailServiceMock.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<OrderDetail, bool>>>(),
                null,
                null))
                .ReturnsAsync(new List<OrderDetail>());

            _categoryServiceMock.Setup(x => x.ListAsync(
                null,
                It.IsAny<Func<IQueryable<Categories>, IOrderedQueryable<Categories>>>(),
                null))
                .ReturnsAsync(new List<Categories> { category });

            _productServiceMock.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                null))
                .ReturnsAsync(new List<Product>());

            _storeDetailServiceMock.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<StoreDetails, bool>>>(),
                null,
                null))
                .ReturnsAsync(new List<StoreDetails>());

            _categoryServiceMock.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Categories, bool>>>(),
                null,
                null))
                .ReturnsAsync(new List<Categories>());

            // Act
            var result = await _controller.ProductDetail(productId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ProductDetails>(result.Model);
        }

        [Test]
        public async Task TC02_ProductDetail_InvalidProductId_ReturnsNotFoundRedirect()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _productServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _controller.ProductDetail(productId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("NotFoundPage", redirectResult.ActionName);
        }
        [Test]
        public void TC03_ProductDetail_ServiceThrowsException_ThrowsException()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _productServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ThrowsAsync(new Exception("Unknown error, please try again."));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _controller.ProductDetail(productId));
            Assert.That(ex.Message, Is.EqualTo("Unknown error, please try again."));
        }

    }
}
