using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
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
using Food_Haven.Web.Hubs;
using Food_Haven.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Net.payOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Home_AddViewHistory_Test
{
    public class AddViewHistory_Test
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
        private Mock<IHubContext<ChatHub>> _hubContextMock;

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
            _hubContextMock = new Mock<IHubContext<ChatHub>>();

            var payOS = new PayOS("client-id", "api-key", "https://callback.url");
            var recipeSearchService = new RecipeSearchService("");
            _hubContextMock = new Mock<IHubContext<ChatHub>>();
            var mockClientProxy = new Mock<IClientProxy>();

            _hubContextMock
                .Setup(x => x.Clients.All)
                .Returns(mockClientProxy.Object);

            mockClientProxy
                .Setup(x => x.SendCoreAsync(
                    It.IsAny<string>(),
                    It.IsAny<object[]>(),
                    default
                ))
                .Returns(Task.CompletedTask);

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
                _hubContextMock.Object
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
        public async Task AddViewHistory_ExistingHistory_UpdatesIt()
        {
            // Arrange
            var recipeId = Guid.NewGuid();
            var matchedIngredients = "Tomato, Cheese";

            var user = new AppUser { Id = "user123" };

            _userManagerMock
                .Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var existingHistory = new RecipeViewHistory
            {
                ID = Guid.NewGuid(),
                UserID = user.Id,
                ExpertRecipeId = recipeId,
                ViewedAt = DateTime.Now.AddDays(-1),
                MatchedIngredients = "Old Ingredients"
            };

            _recipeViewHistoryServicesMock
                .Setup(s => s.FindAsync(It.IsAny<Expression<Func<RecipeViewHistory, bool>>>()))
                .ReturnsAsync(existingHistory);

            // Act
            var result = await _controller.AddViewHistory(recipeId, matchedIngredients);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);

            _recipeViewHistoryServicesMock.Verify(s => s.UpdateAsync(It.Is<RecipeViewHistory>(
                h => h.UserID == user.Id &&
                     h.ExpertRecipeId == recipeId &&
                     h.MatchedIngredients == matchedIngredients
            )), Times.Once);

            _recipeViewHistoryServicesMock.Verify(s => s.SaveChangesAsync(), Times.Once);
            _recipeViewHistoryServicesMock.Verify(s => s.AddAsync(It.IsAny<RecipeViewHistory>()), Times.Never);
        }

        [Test]
        public async Task AddViewHistory_NewHistory_AddsIt()
        {
            // Arrange
            var recipeId = Guid.NewGuid();
            var matchedIngredients = "Beef, Pepper";

            var user = new AppUser { Id = "user456" };

            _userManagerMock
                .Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _recipeViewHistoryServicesMock
                .Setup(s => s.FindAsync(It.IsAny<Expression<Func<RecipeViewHistory, bool>>>()))
                .ReturnsAsync((RecipeViewHistory)null);

            // Act
            var result = await _controller.AddViewHistory(recipeId, matchedIngredients);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);

            _recipeViewHistoryServicesMock.Verify(s => s.AddAsync(It.Is<RecipeViewHistory>(
                h => h.UserID == user.Id &&
                     h.ExpertRecipeId == recipeId &&
                     h.MatchedIngredients == matchedIngredients
            )), Times.Once);

            _recipeViewHistoryServicesMock.Verify(s => s.SaveChangesAsync(), Times.Once);
            _recipeViewHistoryServicesMock.Verify(s => s.UpdateAsync(It.IsAny<RecipeViewHistory>()), Times.Never);
        }

        [Test]
        public async Task AddViewHistory_UserNotLoggedIn_ReturnsUnauthorized()
        {
            // Arrange
            _userManagerMock
                .Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((AppUser)null);

            // Act
            var result = await _controller.AddViewHistory(Guid.NewGuid(), "test");

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
            _recipeViewHistoryServicesMock.Verify(s => s.AddAsync(It.IsAny<RecipeViewHistory>()), Times.Never);
            _recipeViewHistoryServicesMock.Verify(s => s.UpdateAsync(It.IsAny<RecipeViewHistory>()), Times.Never);
        }

        [Test]
        public async Task AddViewHistory_ThrowsException_ReturnsStatus500()
        {
            // Arrange
            var user = new AppUser { Id = "user789" };

            _userManagerMock
                .Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _recipeViewHistoryServicesMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<RecipeViewHistory, bool>>>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.AddViewHistory(Guid.NewGuid(), "Something");

            // Assert
            var statusResult = result as ObjectResult;
            Assert.IsNotNull(statusResult);
            Assert.AreEqual(500, statusResult.StatusCode);
        }
    }
}
