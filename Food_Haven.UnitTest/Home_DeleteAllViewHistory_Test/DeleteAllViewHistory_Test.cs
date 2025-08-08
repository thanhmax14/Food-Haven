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
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Home_DeleteAllViewHistory_Test
{
    public class DeleteAllViewHistory_Test
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
            var recipeSearchService = new RecipeSearchService(""); // Nếu bạn dùng interface thì nên mock

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
        public async Task DeleteAllViewHistory_UserIsNull_ReturnsUnauthorized()
        {
            // Arrange
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((AppUser)null);

            // Act
            var result = await _controller.DeleteAllViewHistory();

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task DeleteAllViewHistory_Success_ReturnsOk()
        {
            // Arrange
            var user = new AppUser { Id = "user123" };

            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var mockHistoryList = new List<RecipeViewHistory>
            {
                new RecipeViewHistory { ID = Guid.NewGuid(), UserID = "user123" },
                new RecipeViewHistory { ID = Guid.NewGuid(), UserID = "user123" }
            };

            _recipeViewHistoryServicesMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<RecipeViewHistory, bool>>>(),
                    null,
                    null))
                .ReturnsAsync(mockHistoryList);

            _recipeViewHistoryServicesMock
                .Setup(s => s.DeleteAsync(It.IsAny<RecipeViewHistory>()))
                .Returns(Task.CompletedTask);

            _recipeViewHistoryServicesMock
                .Setup(s => s.SaveChangesAsync())
                .ReturnsAsync(1);

            var mockClientProxy = new Mock<IClientProxy>();
            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
            _hubContextMock.Setup(c => c.Clients).Returns(mockClients.Object);

            mockClientProxy
                .Setup(c => c.SendCoreAsync("ReceiveDeleteExperRecipe", It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteAllViewHistory();

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
            _recipeViewHistoryServicesMock.Verify(s => s.DeleteAsync(It.IsAny<RecipeViewHistory>()), Times.Exactly(2));
            _recipeViewHistoryServicesMock.Verify(s => s.SaveChangesAsync(), Times.Once);
            mockClientProxy.Verify(c => c.SendCoreAsync("ReceiveDeleteExperRecipe", It.IsAny<object[]>(), default), Times.Once);
        }

        [Test]
        public async Task DeleteAllViewHistory_ExceptionThrown_Returns500()
        {
            // Arrange
            var user = new AppUser { Id = "user123" };

            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _recipeViewHistoryServicesMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<RecipeViewHistory, bool>>>(),
                    null,
                    null))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _controller.DeleteAllViewHistory();

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while deleting all view history.", objectResult.Value);
        }
    }
}
