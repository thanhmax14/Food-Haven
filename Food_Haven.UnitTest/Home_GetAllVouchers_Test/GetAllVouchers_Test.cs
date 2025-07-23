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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Home_GetAllVouchers_Test
{
    public class GetAllVouchers_Test
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
        public async Task GetAllVouchers_ValidProductVariantWithVouchers_ReturnsJson()
        {
            var variantId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var storeId = Guid.NewGuid();

            var productVariant = new ProductTypes { ID = variantId, ProductID = productId };
            var product = new Product { ID = productId, StoreID = storeId };

            var vouchers = new List<Voucher>
    {
        new Voucher
        {
            ID = Guid.NewGuid(),
            Code = "SALE10",
            DiscountType = "Percent",
            DiscountAmount = 10,
            MinOrderValue = 50000,
            ExpirationDate = DateTime.Now.AddDays(5),
            MaxUsage = 100,
            CurrentUsage = 10,
            IsGlobal = false,
            StoreID = storeId,
            IsActive = true
        }
    };

            _productVariantServiceMock.Setup(s => s.FindAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>()))
                .ReturnsAsync(productVariant);
            _productServiceMock.Setup(s => s.FindAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(product);
            _voucherServiceMock.Setup(s => s.GetAll())
                .Returns(vouchers.AsQueryable());

            var result = await _controller.GetAllVouchers(variantId) as JsonResult;

            Assert.IsNotNull(result);
            var list = result.Value as IEnumerable<object>;
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Any());
        }
        [Test]
        public async Task GetAllVouchers_ValidProductVariantButNoVouchers_ReturnsEmptyJsonList()
        {
            var variantId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var storeId = Guid.NewGuid();

            var productVariant = new ProductTypes { ID = variantId, ProductID = productId };
            var product = new Product { ID = productId, StoreID = storeId };

            var vouchers = new List<Voucher>(); // No voucher

            _productVariantServiceMock.Setup(s => s.FindAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>()))
                .ReturnsAsync(productVariant);
            _productServiceMock.Setup(s => s.FindAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(product);
            _voucherServiceMock.Setup(s => s.GetAll())
                .Returns(vouchers.AsQueryable());

            var result = await _controller.GetAllVouchers(variantId) as JsonResult;

            Assert.IsNotNull(result);
            var list = result.Value as IEnumerable<object>;
            Assert.IsNotNull(list);
            Assert.IsEmpty(list);
        }
        [Test]
        public async Task GetAllVouchers_InvalidProductVariant_ReturnsNotFound()
        {
            var invalidId = Guid.NewGuid();

            _productVariantServiceMock.Setup(s => s.FindAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>()))
                .ReturnsAsync((ProductTypes)null); // không tồn tại

            var result = await _controller.GetAllVouchers(invalidId) as NotFoundObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);

            // Cách xử lý message
            var dict = result.Value?.GetType()
                          .GetProperty("message")
                          ?.GetValue(result.Value, null)?.ToString();

            Assert.AreEqual("Product variant not found", dict);
        }

        [Test]
        public async Task GetAllVouchers_ExceptionThrown_ReturnsException()
        {
            var id = Guid.NewGuid();

            _productVariantServiceMock.Setup(s => s.FindAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>()))
                .ThrowsAsync(new Exception("DB error"));

            Assert.ThrowsAsync<Exception>(async () =>
            {
                await _controller.GetAllVouchers(id);
            });
        }

    }
}
