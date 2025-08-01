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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Home_RemoveWish_Test
{
    public class RemoveWish_Test
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
        public async Task TC01_RemoveWishSuccess_ShouldReturnSuccessJson()
        {
            var productId = Guid.NewGuid();
            var user = new AppUser { Id = "A90722CE-DA34-461D-9CD3-1867A2AA5CF6" };
            var product = new Product { ID = productId, Name = "Test Product" };
            var wishlist = new Wishlist { ID = Guid.NewGuid(), ProductID = productId, UserID = user.Id };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _productServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>())).ReturnsAsync(product);
            _wishlistServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Wishlist, bool>>>())).ReturnsAsync(wishlist);

            _wishlistServiceMock.Setup(x => x.DeleteAsync(wishlist)).Returns(Task.CompletedTask);
            _wishlistServiceMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _controller.RemoveWish(Guid.Parse("A90722CE-DA34-461D-9CD3-1867A2AA5CF6")) as JsonResult;

            var json = JObject.FromObject(result.Value);

            Assert.IsTrue(json["success"]!.Value<bool>());
            Assert.AreEqual("Deleted successfully!", json["message"]!.Value<string>());
        }

        [Test]
        public async Task TC02_ProductNotFound_ShouldReturnError()
        {
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new AppUser { Id = "A90722CE-DA34-461D-9CD3-1867A2AA5CF6abc" });

            _productServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync((Product)null);

            var result = await _controller.RemoveWish(Guid.NewGuid()) as JsonResult;
            var json = JObject.FromObject(result.Value);

            Assert.IsFalse(json["success"]!.Value<bool>());
            Assert.AreEqual("Product does not exist!", json["message"]!.Value<string>());
        }

      

   
        [Test]
        public async Task TC03_RemoveWishFails_ThrowsException_ShouldReturnDeleteFailed()
        {
            var productId = Guid.NewGuid();
            var user = new AppUser { Id = "user123" };
            var product = new Product { ID = productId, Name = "Test Product" };
            var wishlist = new Wishlist { ID = Guid.NewGuid(), ProductID = productId, UserID = user.Id };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _productServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(product);
            _wishlistServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Wishlist, bool>>>())).ReturnsAsync(wishlist);

            _wishlistServiceMock.Setup(x => x.DeleteAsync(wishlist)).ThrowsAsync(new Exception("DB Error"));

            var result = await _controller.RemoveWish(productId) as JsonResult;
            var json = JObject.FromObject(result.Value);

            Assert.IsFalse(json["success"]!.Value<bool>());
            Assert.AreEqual("Delete failed!", json["message"]!.Value<string>());
        }
    }
}
