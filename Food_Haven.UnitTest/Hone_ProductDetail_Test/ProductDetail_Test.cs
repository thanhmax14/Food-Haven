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
        public async Task TC01_ProductDetail_ValidId_ReturnsView()
        {
            var id = Guid.NewGuid();
            var product = new Product { ID = id, IsActive = true, CategoryID = Guid.NewGuid(), StoreID = Guid.NewGuid() };
            var store = new StoreDetails { ID = product.StoreID, IsActive = true, UserID = "user1" };

            _productServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(product);
            _storeDetailServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync(store);
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new AppUser());

            _cartServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Categories, bool>>>()))
                .ReturnsAsync(new Categories());
            _cartServiceMock.Setup(x => x.ListAsync(It.IsAny<Func<IQueryable<Categories>, IOrderedQueryable<Categories>>>()))
                .ReturnsAsync(new List<Categories>());

            _productImageServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<ProductImage, bool>>>()))
                .ReturnsAsync(new List<ProductImage>());
            _productVariantServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<ProductVariant, bool>>>()))
                .ReturnsAsync(new List<ProductVariant>());
            _reviewServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Review, bool>>>()))
                .ReturnsAsync(new List<Review>());
            _wishlistServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Wishlist, bool>>>()))
                .ReturnsAsync(new List<Wishlist>());
            _orderDetailServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>()))
                .ReturnsAsync(new List<OrderDetail>());
            _productServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>()))
                .ReturnsAsync(new List<Product>());

            var result = await _controller.ProductDetail(id) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ProductDetails>(result.Model);
        }

        [Test]
        public async Task TC02_ProductDetail_InvalidId_ReturnsNotFound()
        {
            _productServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync((Product)null);

            var result = await _controller.ProductDetail(Guid.NewGuid()) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("NotFoundPage", result.ActionName);
        }

        [Test]
        public async Task TC03_ProductDetail_StoreNotFound_ReturnsNotFound()
        {
            var id = Guid.NewGuid();
            var product = new Product { ID = id, IsActive = true, StoreID = Guid.NewGuid() };

            _productServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(product);
            _storeDetailServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync((StoreDetails)null);

            var result = await _controller.ProductDetail(id) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("NotFoundPage", result.ActionName);
        }

        [Test]
        public async Task TC04_ProductDetail_ServiceFails_ReturnsMessage()
        {
            var id = Guid.NewGuid();
            _productServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ThrowsAsync(new Exception("Service failure"));

            try
            {
                await _controller.ProductDetail(id);
                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Service failure", ex.Message);
            }
        }

        [Test]
        public async Task TC05_ProductDetail_ThrowsException()
        {
            _productServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .Throws(new NullReferenceException("Unexpected null"));

            Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                await _controller.ProductDetail(Guid.NewGuid());
            });
        }

        [Test]
        public async Task TC06_ProductDetail_ServerConnectionFails_ReturnsMessage()
        {
            _productServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ThrowsAsync(new HttpRequestException("Cannot connect with server"));

            try
            {
                await _controller.ProductDetail(Guid.NewGuid());
                Assert.Fail("Expected exception not thrown");
            }
            catch (HttpRequestException ex)
            {
                Assert.That(ex.Message, Is.EqualTo("Cannot connect with server"));
            }
        }
    }
}
