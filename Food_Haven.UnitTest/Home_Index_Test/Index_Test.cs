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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Home_Index_Test
{
    public class Index_Test
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
                _recipeViewHistoryServicesMock.Object
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
        public async Task TC01_Index_GuestUser_WithValidData_ReturnsViewWithData()
        {
            // Arrange
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((AppUser)null);

            var category = new Categories { ID = Guid.NewGuid(), IsActive = true };
            var product = new Product { ID = Guid.NewGuid(), CategoryID = category.ID, StoreID = Guid.NewGuid(), IsActive = true };
            var variant = new ProductTypes { ProductID = product.ID, IsActive = true };
            var store = new StoreDetails { ID = product.StoreID };

            _categoryServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Categories, bool>>>(), null, null))
                .ReturnsAsync(new List<Categories> { category });

            _productServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(), null))
                .ReturnsAsync(new List<Product> { product });

            _productVariantServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>(), null, null))
                .ReturnsAsync(new List<ProductTypes> { variant });

            _storeDetailServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>(), null, null))
                .ReturnsAsync(new List<StoreDetails> { store });

            _wishlistServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Wishlist, bool>>>(), null, null))
                .ReturnsAsync(new List<Wishlist>());

            _productImageServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<ProductImage, bool>>>(), null, null))
                .ReturnsAsync(new List<ProductImage>());

            _reviewServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Review, bool>>>(), null, null))
                .ReturnsAsync(new List<Review>());

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as HomeViewModel;
            Assert.IsNotNull(model);
            Assert.IsNotEmpty(model.Products);
        }
        [Test]
        public async Task TC02_Index_ProductWithoutActiveVariant_ReturnsEmptyProductList()
        {
            // Arrange
            var category = new Categories { ID = Guid.NewGuid(), IsActive = true };
            var product = new Product { ID = Guid.NewGuid(), CategoryID = category.ID, StoreID = Guid.NewGuid(), IsActive = true };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((AppUser)null);

            _categoryServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Categories, bool>>>(), null, null))
                .ReturnsAsync(new List<Categories> { category });

            _productServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(), null))
                .ReturnsAsync(new List<Product> { product });

            _productVariantServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>(), null, null))
                .ReturnsAsync(new List<ProductTypes>()); // không có variant active

            _storeDetailServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>(), null, null))
                .ReturnsAsync(new List<StoreDetails>());

            _wishlistServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Wishlist, bool>>>(), null, null))
                .ReturnsAsync(new List<Wishlist>());

            _productImageServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<ProductImage, bool>>>(), null, null))
                .ReturnsAsync(new List<ProductImage>());

            _reviewServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Review, bool>>>(), null, null))
                .ReturnsAsync(new List<Review>());

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as HomeViewModel;
            Assert.IsNotNull(model);
            Assert.IsEmpty(model.Products); // danh sách rỗng
        }

        [Test]
        public void TC03_Index_ServiceThrowsException_ThrowsException()
        {
            // Arrange
            _categoryServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Categories, bool>>>(), null, null))
                .ThrowsAsync(new Exception("Unknown error, please try again."));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _controller.Index());
            Assert.That(ex.Message, Is.EqualTo("Unknown error, please try again."));
        }

    }
}
