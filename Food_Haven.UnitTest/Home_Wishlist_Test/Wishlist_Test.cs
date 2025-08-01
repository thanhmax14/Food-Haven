using BusinessLogic.Hash;
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
using Microsoft.EntityFrameworkCore.Query;
using Models;
using Moq;
using Net.payOS;
using Newtonsoft.Json;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace Food_Haven.UnitTest.Home_Wishlist_Test
{
    public class Wishlist_Test
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
            _expertRecipeServicesMock = new Mock<IExpertRecipeServices>();
            _recipeViewHistoryServicesMock = new Mock<IRecipeViewHistoryServices>();
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
                recipeSearchService,
                 _expertRecipeServicesMock.Object, // <-- Add this argument
 _recipeViewHistoryServicesMock.Object // <-- Add this argument
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
        public async Task TC01_ValidUserWithWishlistItems_ShouldReturnViewWithList()
        {
            // Arrange
            var userId = "8e91c798-bc78-46a9-89a4-5d0aaea77f5f";
            var user = new AppUser { Id = userId };
            var wishlistItem = new Wishlist { ID = Guid.NewGuid(), ProductID = Guid.NewGuid(), UserID = userId, CreateDate = DateTime.Now };
            var product = new Product { ID = wishlistItem.ProductID, Name = "Test Product" };
            var price = new ProductTypes { ProductID = product.ID, SellPrice = 50000 };
            var image = new ProductImage { ProductID = product.ID, ImageUrl = "image.jpg" };

            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _wishlistServiceMock.Setup(w => w.ListAsync(It.IsAny<Expression<Func<Wishlist, bool>>>(), It.IsAny<Func<IQueryable<Wishlist>, IOrderedQueryable<Wishlist>>>(), null)).ReturnsAsync(new List<Wishlist> { wishlistItem });
            _productServiceMock.Setup(p => p.FindAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(product);
            _productVariantServiceMock.Setup(pv => pv.FindAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>())).ReturnsAsync(price);
            _productImageServiceMock.Setup(pi => pi.ListAsync(It.IsAny<Expression<Func<ProductImage, bool>>>(), null, null)).ReturnsAsync(new List<ProductImage> { image });

            // Act
            var result = await _controller.Wishlist() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as List<wishlistViewModels>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("Test Product", model[0].name);
            Assert.AreEqual(50000, model[0].price);
            Assert.AreEqual("image.jpg", model[0].img);
        }
        [Test]
        public async Task TC02_ValidUserWithNoWishlistItems_ShouldReturnViewWithEmptyList()
        {
            // Arrange
            var userId = "00f56274-2b84-4965-bf88-bc522e2e17ed";
            var user = new AppUser { Id = userId };

            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _wishlistServiceMock.Setup(w => w.ListAsync(It.IsAny<Expression<Func<Wishlist, bool>>>(), It.IsAny<Func<IQueryable<Wishlist>, IOrderedQueryable<Wishlist>>>(), null)).ReturnsAsync(new List<Wishlist>());

            // Act
            var result = await _controller.Wishlist() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as List<wishlistViewModels>;
            Assert.IsNotNull(model);
            Assert.IsEmpty(model);
        }
        [Test]
        public async Task TC04_ExceptionThrown_ShouldCatchException()
        {
            // Arrange
            var userId = "8e91c798-bc78-46a9-89a4-5d0aaea77f5f";
            var user = new AppUser { Id = userId, UserName = "testuser" };

            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var wishlist = new List<Wishlist>
    {
        new Wishlist { ProductID = Guid.NewGuid(), UserID = userId, CreateDate = DateTime.Now }
    };

            _wishlistServiceMock.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Wishlist, bool>>>(),
                It.IsAny<Func<IQueryable<Wishlist>, IOrderedQueryable<Wishlist>>>(),
                null
            )).ReturnsAsync(wishlist);

            _productServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Product, bool>>>())).ThrowsAsync(new Exception("Unexpected error"));

            // Act + Assert with try-catch
            try
            {
                var result = await _controller.Wishlist();
                Assert.Fail("Expected an exception, but none was thrown.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Unexpected error", ex.Message);
            }
        }





    }
}
