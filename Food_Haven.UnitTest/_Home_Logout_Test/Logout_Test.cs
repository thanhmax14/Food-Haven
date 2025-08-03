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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest._Home_Logout_Test
{
    public class Logout_Test
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<SignInManager<AppUser>> _signInManagerMock;
        private HomeController _controller;
        private Mock<IExpertRecipeServices> expertRecipeServicesMock;
        private Mock<IRecipeViewHistoryServices> recipeViewHistoryServicesMock;
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

            var payOS = new PayOS("client-id", "api-key", "https://callback.url");
            var orderDetailMock = new Mock<IOrderDetailService>();
            var recipeServiceMock = new Mock<IRecipeService>();
            var categoryServiceMock = new Mock<ICategoryService>();
            var storeDetailServiceMock = new Mock<IStoreDetailService>();
            var emailSenderMock = new Mock<IEmailSender>();
            var cartMock = new Mock<ICartService>();
            var wishlistMock = new Mock<IWishlistServices>();
            var productServiceMock = new Mock<IProductService>();
            var productImgServiceMock = new Mock<IProductImageService>();
            var productVariantMock = new Mock<IProductVariantService>();
            var reviewServiceMock = new Mock<IReviewService>();
            var balanceChangeMock = new Mock<IBalanceChangeService>();
            var ordersServiceMock = new Mock<IOrdersServices>();
            var voucherMock = new Mock<IVoucherServices>();
            var storeReportMock = new Mock<IStoreReportServices>();
            var storeFollowersMock = new Mock<IStoreFollowersService>();
            var recipeSearchMock = new RecipeSearchService("");
            var hubContextMock = new Mock<IHubContext<ChatHub>>(); // Add this line

            _controller = new HomeController(
                _signInManagerMock.Object,
                orderDetailMock.Object,
                recipeServiceMock.Object,
                _userManagerMock.Object,
                categoryServiceMock.Object,
                storeDetailServiceMock.Object,
                emailSenderMock.Object,
                cartMock.Object,
                wishlistMock.Object,
                productServiceMock.Object,
                productImgServiceMock.Object,
                productVariantMock.Object,
                reviewServiceMock.Object,
                balanceChangeMock.Object,
                ordersServiceMock.Object,
                payOS,
                voucherMock.Object,
                storeReportMock.Object,
                storeFollowersMock.Object,
                recipeSearchMock,
                expertRecipeServicesMock.Object,
                recipeViewHistoryServicesMock.Object,
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
        public async Task Logout_UserAuthenticated_UpdatesLastAccessAndSignsOutAndClearsSession()
        {
            // Arrange
            var userId = "test-user-id";
            var user = new AppUser { Id = userId };
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identityMock = new Mock<ClaimsIdentity>();
            identityMock.Setup(i => i.IsAuthenticated).Returns(true);
            identityMock.Setup(i => i.FindFirst(It.IsAny<string>())).Returns((string type) => claims[0]);
            var principalMock = new Mock<ClaimsPrincipal>();
            principalMock.Setup(p => p.Identity).Returns(identityMock.Object);
            principalMock.Setup(p => p.FindFirst(It.IsAny<string>())).Returns((string type) => claims[0]);

            var httpContext = new DefaultHttpContext();
            var sessionMock = new Mock<ISession>();
            httpContext.Session = sessionMock.Object;
            httpContext.User = principalMock.Object;
            _controller.ControllerContext.HttpContext = httpContext;

            _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
            _signInManagerMock.Setup(x => x.SignOutAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout();

            // Assert
            _userManagerMock.Verify(x => x.FindByIdAsync(userId), Times.Once);
            _userManagerMock.Verify(x => x.UpdateAsync(It.Is<AppUser>(u => u == user)), Times.Once);
            _signInManagerMock.Verify(x => x.SignOutAsync(), Times.Once);
            sessionMock.Verify(x => x.Clear(), Times.Once);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = result as RedirectToActionResult;
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [Test]
        public async Task Logout_UserNotAuthenticated_OnlySignsOutAndClearsSession()
        {
            // Arrange
            var identityMock = new Mock<ClaimsIdentity>();
            identityMock.Setup(i => i.IsAuthenticated).Returns(false);
            var principalMock = new Mock<ClaimsPrincipal>();
            principalMock.Setup(p => p.Identity).Returns(identityMock.Object);

            var httpContext = new DefaultHttpContext();
            var sessionMock = new Mock<ISession>();
            httpContext.Session = sessionMock.Object;
            httpContext.User = principalMock.Object;
            _controller.ControllerContext.HttpContext = httpContext;

            _signInManagerMock.Setup(x => x.SignOutAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout();

            // Assert
            _userManagerMock.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Never);
            _userManagerMock.Verify(x => x.UpdateAsync(It.IsAny<AppUser>()), Times.Never);
            _signInManagerMock.Verify(x => x.SignOutAsync(), Times.Once);
            sessionMock.Verify(x => x.Clear(), Times.Once);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = result as RedirectToActionResult;
            Assert.AreEqual("Index", redirect.ActionName);
        }
    }
}
