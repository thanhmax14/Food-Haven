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

namespace Food_Haven.UnitTest.Home_SearchProductList_Test
{
    public class Home_SearchProductList_Test
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
        public async Task SearchProductList_NoSearchName_RedirectsToIndex()
        {
            var result = await _controller.SearchProductList(null);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [Test]
        public async Task SearchProductList_WithSearchName_ReturnsViewWithResults()
        {
            // Arrange
            var products = new List<Product> { new Product { ID = System.Guid.NewGuid(), Name = "rice", IsActive = true } };
            var productTypes = new List<ProductTypes> { new ProductTypes { ProductID = products[0].ID, IsActive = true } };
            _productServiceMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>(), null, null))
                .ReturnsAsync(products);
            _productServiceMock.Setup(x => x.ListAsync()).ReturnsAsync(products);
            _productVariantServiceMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ProductTypes, bool>>>(), null, null))
                .ReturnsAsync(productTypes);

            _categoryServiceMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Categories, bool>>>(), null, null))
                .ReturnsAsync(new List<Categories>());
            _storeDetailServiceMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<StoreDetails, bool>>>(), null, null))
                .ReturnsAsync(new List<StoreDetails>());
            _productImageServiceMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ProductImage, bool>>>(), null, null))
                .ReturnsAsync(new List<ProductImage>());
            _reviewServiceMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Review, bool>>>(), null, null))
                .ReturnsAsync(new List<Review>());
            _wishlistServiceMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Wishlist, bool>>>(), null, null))
                .ReturnsAsync(new List<Wishlist>());
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync((AppUser)null);

            // Act
            var result = await _controller.SearchProductList("rice");

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOf<HomeViewModel>(viewResult.Model);
            var model = (HomeViewModel)viewResult.Model;
            Assert.IsNotNull(model.SearchResults);
        }

        [Test]
        public async Task SearchProductList_WithSearchName_NoResults_ReturnsEmptyResults()
        {
            _productServiceMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>(), null, null))
                .ReturnsAsync(new List<Product>());
            _productServiceMock.Setup(x => x.ListAsync()).ReturnsAsync(new List<Product>());
            _productVariantServiceMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ProductTypes, bool>>>(), null, null))
                .ReturnsAsync(new List<ProductTypes>());

            _categoryServiceMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Categories, bool>>>(), null, null))
                .ReturnsAsync(new List<Categories>());
            _storeDetailServiceMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<StoreDetails, bool>>>(), null, null))
                .ReturnsAsync(new List<StoreDetails>());
            _productImageServiceMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ProductImage, bool>>>(), null, null))
                .ReturnsAsync(new List<ProductImage>());
            _reviewServiceMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Review, bool>>>(), null, null))
                .ReturnsAsync(new List<Review>());
            _wishlistServiceMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Wishlist, bool>>>(), null, null))
                .ReturnsAsync(new List<Wishlist>());
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync((AppUser)null);

            var result = await _controller.SearchProductList("NotFound");
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOf<HomeViewModel>(viewResult.Model);
            var model = (HomeViewModel)viewResult.Model;
            Assert.IsEmpty(model.SearchResults);
        }
    }
}
