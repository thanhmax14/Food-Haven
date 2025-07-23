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

namespace Food_Haven.UnitTest.Home_ProductDetail_Test
{
    public class Home_ProductDetail_Test
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
        public async Task ProductDetail_ValidId_ReturnsViewWithProductDetails()
        {
            // Arrange
            var productId = Guid.Parse("D7DFAEE2-9CFF-40F8-A7D8-3C630008B217");
            var storeId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var userId = "user-123";

            var product = new Product
            {
                ID = productId,
                Name = "Test Product",
                StoreID = storeId,
                CategoryID = categoryId,
                IsActive = true,
                ShortDescription = "Short",
                LongDescription = "Long",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
            var store = new StoreDetails
            {
                ID = storeId,
                Name = "Test Store",
                UserID = userId,
                IsActive = true
            };
            var appUser = new AppUser { Id = userId, UserName = "testuser" };
            var category = new Categories { ID = categoryId, Name = "Test Category" };
            var productImages = new List<ProductImage> { new ProductImage { ProductID = productId } };
            var productTypes = new List<ProductTypes> { new ProductTypes { ProductID = productId, IsActive = true } };
            var reviews = new List<Review> { new Review { ProductID = productId, UserID = userId } };
            var wishlists = new List<Wishlist> { new Wishlist { ProductID = productId, UserID = userId } };
            var orderDetails = new List<OrderDetail> { new OrderDetail { Quantity = 2, ProductTypes = new ProductTypes { ProductID = productId } } };
            var categories = new List<Categories> { category };
            var allProducts = new List<Product> { product };

            // Setup mocks
            _productServiceMock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>()))
                .ReturnsAsync(product);
            _storeDetailServiceMock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<StoreDetails, bool>>>()))
                .ReturnsAsync(store);
            _userManagerMock.Setup(s => s.FindByIdAsync(userId)).ReturnsAsync(appUser);
            _categoryServiceMock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Categories, bool>>>()))
                .ReturnsAsync(category);
            _productImageServiceMock.Setup(s => s.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ProductImage, bool>>>(), null, null))
                .ReturnsAsync(productImages);
            _productVariantServiceMock.Setup(s => s.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ProductTypes, bool>>>(), null, null))
                .ReturnsAsync(productTypes);
            _reviewServiceMock.Setup(s => s.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Review, bool>>>(), null, null))
                .ReturnsAsync(reviews);
            _userManagerMock.Setup(s => s.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(appUser);
            _wishlistServiceMock.Setup(s => s.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Wishlist, bool>>>(), null, null))
                .ReturnsAsync(wishlists);
            _orderDetailServiceMock.Setup(s => s.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OrderDetail, bool>>>(), null, null))
                .ReturnsAsync(orderDetails);
            _categoryServiceMock.Setup(s => s.ListAsync(null, It.IsAny<Func<IQueryable<Categories>, IOrderedQueryable<Categories>>>(), null))
                .ReturnsAsync(categories);
            _productServiceMock.Setup(s => s.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>(), It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(), null))
                .ReturnsAsync(allProducts);
            // Setup for same category products, variants, etc. (empty for simplicity)
            _productVariantServiceMock.Setup(s => s.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ProductTypes, bool>>>(), null, null))
                .ReturnsAsync(new List<ProductTypes>());
            _productImageServiceMock.Setup(s => s.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ProductImage, bool>>>(), null, null))
                .ReturnsAsync(new List<ProductImage>());
            _reviewServiceMock.Setup(s => s.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Review, bool>>>(), null, null))
                .ReturnsAsync(new List<Review>());
            _wishlistServiceMock.Setup(s => s.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Wishlist, bool>>>(), null, null))
                .ReturnsAsync(new List<Wishlist>());
            _storeDetailServiceMock.Setup(s => s.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<StoreDetails, bool>>>(), null, null))
                .ReturnsAsync(new List<StoreDetails>());
            _categoryServiceMock.Setup(s => s.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Categories, bool>>>(), null, null))
                .ReturnsAsync(new List<Categories>());

            // Act
            var result = await _controller.ProductDetail(productId);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult.Model);
            var model = viewResult.Model as ProductDetails;
            Assert.IsNotNull(model);
            Assert.AreEqual(productId, model.ID);
            Assert.AreEqual("Test Product", model.Name);
            Assert.AreEqual("Test Store", model.storeDetails.Name);
            Assert.AreEqual("Test Category", model.categories.Name);
            Assert.AreEqual(2, model.totalsell); // From orderDetails.Sum(x => x.Quantity)
                                                 // Assert.IsTrue(model.IsWishList);
        }

        [Test]
        public async Task ProductDetail_ProductNotFound_ReturnsRedirectToNotFoundPage()
        {
            // Arrange
            var productId = Guid.Parse("D7DFAEE2-9CFF-40F8-A7D8-3C630008B217");
            _productServiceMock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>()))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _controller.ProductDetail(productId);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("NotFoundPage", redirect.ActionName);
        }

        //[Test]
        //public async Task ProductDetail_StoreNotFound_ReturnsRedirectToNotFoundPage()
        //{
        //    // Arrange
        //    var productId = Guid.Parse("D7DFAEE2-9CFF-40F8-A7D8-3C630008B217");
        //    var storeId = Guid.NewGuid();
        //    var categoryId = Guid.NewGuid();
        //    var product = new Product
        //    {
        //        ID = productId,
        //        Name = "Test Product",
        //        StoreID = storeId,
        //        CategoryID = categoryId,
        //        IsActive = true
        //    };
        //    _productServiceMock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>()))
        //        .ReturnsAsync(product);
        //    _storeDetailServiceMock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<StoreDetails, bool>>>()))
        //        .ReturnsAsync((StoreDetails)null);

        //    // Act
        //    var result = await _controller.ProductDetail(productId);

        //    // Assert
        //    Assert.IsInstanceOf<RedirectToActionResult>(result);
        //    var redirect = (RedirectToActionResult)result;
        //    Assert.AreEqual("NotFoundPage", redirect.ActionName);
        //}
    }
}
