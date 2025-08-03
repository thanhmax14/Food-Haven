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

namespace Food_Haven.UnitTest.Home_GetStoreDetail
{
    public class GetStoreDetail_Test
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
                 hubContextMock.Object // <-- Add this argument
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
        public async Task GetStoreDetail_ValidId_ReturnsViewWithStoreDetails()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var userId = "user123";
            var categoryId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var store = new StoreDetails
            {
                ID = storeId,
                Name = "Test Store",
                Address = "123 Street",
                Phone = "0123456789",
                ImageUrl = "image.jpg",
                ShortDescriptions = "Short desc",
                LongDescriptions = "Long desc",
                CreatedDate = DateTime.UtcNow,
                UserID = userId
            };

            var user = new AppUser { Id = userId, UserName = "storeowner", Email = "test@mail.com" };

            var product = new Product
            {
                ID = productId,
                Name = "Test Product",
                StoreID = storeId,
                CategoryID = categoryId,
                IsActive = true,
                IsOnSale = true,
                ShortDescription = "short",
                LongDescription = "long",
                ManufactureDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            var variant = new ProductTypes
            {
                ProductID = productId,
                SellPrice = 100000,
                IsActive = true
            };

            var category = new Categories
            {
                ID = categoryId,
                Name = "Category A"
            };

            var imageList = new List<ProductImage>
    {
        new ProductImage { ProductID = productId, ImageUrl = "img1.jpg" },
        new ProductImage { ProductID = productId, ImageUrl = "img2.jpg" }
    };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _storeDetailServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync(store);

            _userManagerMock.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _productServiceMock.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                null))
                .ReturnsAsync(new List<Product> { product });

            _productVariantServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>()))
                .ReturnsAsync(variant);

            _categoryServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Categories, bool>>>()))
                .ReturnsAsync(category);

            _productImageServiceMock.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<ProductImage, bool>>>(),
                null,
                null))
                .ReturnsAsync(imageList);

            // Act
            var result = await _controller.GetStoreDetail(storeId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<StoreDetailsViewModels>(result.Model);
            var model = result.Model as StoreDetailsViewModels;
            Assert.AreEqual("Test Store", model.Name);
            Assert.AreEqual("storeowner", model.UserName);
            Assert.AreEqual(1, model.ProductViewModel.Count);
        }
        [Test]
        public async Task GetStoreDetail_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var storeId = Guid.NewGuid();

            _storeDetailServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync((StoreDetails)null);

            // Act
            var result = await _controller.GetStoreDetail(storeId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFound = result as NotFoundObjectResult;
            Assert.AreEqual("Store not found", notFound.Value);
        }
        [Test]
        public async Task GetStoreDetail_ValidStoreButNoProducts_ReturnsViewWithEmptyProductList()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var userId = "user123";

            var store = new StoreDetails { ID = storeId, Name = "Test Store", UserID = userId };
            var user = new AppUser { Id = userId, UserName = "storeowner", Email = "owner@mail.com" };

            _storeDetailServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync(store);

            _userManagerMock.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _productServiceMock.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                null))
                .ReturnsAsync(new List<Product>()); // Empty product list

            // Act
            var result = await _controller.GetStoreDetail(storeId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<StoreDetailsViewModels>(result.Model);
            var model = result.Model as StoreDetailsViewModels;
            Assert.AreEqual(0, model.ProductViewModel.Count);
        }

    }
}
