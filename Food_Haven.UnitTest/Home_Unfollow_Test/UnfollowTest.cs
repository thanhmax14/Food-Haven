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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Home_Unfollow_Test
{
    public class UnfollowTest
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<SignInManager<AppUser>> _signInManagerMock;
        private HomeController _controller;
        private Mock<IExpertRecipeServices> _expertRecipeServicesMock;
        private Mock<IRecipeViewHistoryServices> _recipeViewHistoryServicesMock;
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
            var payOSMock = payOS;
            var voucherMock = new Mock<IVoucherServices>();
            var storeReportMock = new Mock<IStoreReportServices>();
            var storeFollowersMock = new Mock<IStoreFollowersService>();
            var recipeSearchMock = new RecipeSearchService("");
            var hubContextMock = new Mock<IHubContext<ChatHub>>();

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
                payOSMock,
                voucherMock.Object,
                storeReportMock.Object,
                storeFollowersMock.Object,
                recipeSearchMock,
                 _expertRecipeServicesMock.Object, // <-- Add this argument
 _recipeViewHistoryServicesMock.Object,
                 hubContextMock.Object
            // <-- Add this argument
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
        private void ReplaceField<T>(object target, string fieldName, T newValue)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(target, newValue);
        }

        [Test]
        public async Task UnfollowStore_SuccessfullyUnfollowed_ReturnsSuccessJson()
        {
            // Arrange
            var user = new AppUser { Id = "user123" };
            var storeId = Guid.Parse("0521BE69-FC8D-4C9D-80DC-8D4DEA3B814C");
            var storeFollower = new StoreFollower { StoreID = storeId, UserID = user.Id };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var storeFollowersMock = new Mock<IStoreFollowersService>();
            storeFollowersMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreFollower, bool>>>()))
                              .ReturnsAsync(storeFollower);
            storeFollowersMock.Setup(x => x.DeleteAsync(storeFollower)).Returns(Task.CompletedTask);
            storeFollowersMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            ReplaceField(_controller, "_storeFollowersService", storeFollowersMock.Object);

            // Act
            var result = await _controller.UnfollowStore(storeId) as JsonResult;

            // Assert
            Assert.IsNotNull(result);

            // Serialize to JSON and deserialize back to a dictionary
            var json = JsonSerializer.Serialize(result.Value);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            Assert.AreEqual(true, bool.Parse(data["success"].ToString()));
            Assert.AreEqual("Successfully unfollowed the store.", data["message"].ToString());
        }

        [Test]
        public async Task UnfollowStore_StoreNotFollowed_ReturnsNotFollowingMessage()
        {
            // Arrange
            var user = new AppUser { Id = "user123" };
            var storeId = Guid.NewGuid(); // random Guid hợp lệ

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var storeFollowersMock = new Mock<IStoreFollowersService>();
            storeFollowersMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreFollower, bool>>>()))
                              .ReturnsAsync((StoreFollower)null); // simulate not followed

            ReplaceField(_controller, "_storeFollowersService", storeFollowersMock.Object);

            // Act
            var result = await _controller.UnfollowStore(storeId) as JsonResult;

            // Assert
            var json = JsonSerializer.Serialize(result.Value);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            Assert.AreEqual(false, bool.Parse(data["success"].ToString()));
            Assert.AreEqual("You are not following this store.", data["message"].ToString());
        }
        [Test]
        public async Task UnfollowStore_UnexpectedException_ReturnsErrorMessage()
        {
            // Arrange
            var user = new AppUser { Id = "user123" };
            var storeId = Guid.NewGuid();
            var storeFollower = new StoreFollower { StoreID = storeId, UserID = user.Id };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var storeFollowersMock = new Mock<IStoreFollowersService>();
            storeFollowersMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreFollower, bool>>>()))
                              .ReturnsAsync(storeFollower);
            storeFollowersMock.Setup(x => x.DeleteAsync(It.IsAny<StoreFollower>()))
                              .ThrowsAsync(new Exception("Simulated failure"));

            ReplaceField(_controller, "_storeFollowersService", storeFollowersMock.Object);

            // Act
            var result = await _controller.UnfollowStore(storeId) as JsonResult;

            // Fix casting using JsonElement
            var json = JsonSerializer.Serialize(result.Value);
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            // Assert
            Assert.IsFalse(dict["success"].GetBoolean());
            Assert.AreEqual("An error occurred while trying to unfollow the store. Please try again later.", dict["message"].GetString());
            Assert.AreEqual("Simulated failure", dict["error"].GetString());
        }


    }
}
