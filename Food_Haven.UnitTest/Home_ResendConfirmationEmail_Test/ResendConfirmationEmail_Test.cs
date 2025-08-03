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
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.SignalR;
using Models;
using Moq;
using Net.payOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Home_ResendConfirmationEmail_Test
{
    public class ResendConfirmationEmail_Test
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
        [TestCase("giahuy", true)] //TC_01
        [TestCase("abc@gmail.com", false)] //TC_04
        public async Task TC01_TC04_ResendEmail_Success(string input, bool isUsername)
        {
            // Arrange
            var user = new AppUser { Id = "123", UserName = "giahuy", Email = "abc@gmail.com" };

            if (isUsername)
            {
                _userManagerMock.Setup(x => x.FindByNameAsync(input)).ReturnsAsync(user);
            }
            else
            {
                _userManagerMock.Setup(x => x.FindByNameAsync(input)).ReturnsAsync((AppUser)null);
                _userManagerMock.Setup(x => x.FindByEmailAsync(input)).ReturnsAsync(user);
            }

            _userManagerMock.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(false);
            _userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(user)).ReturnsAsync("token");

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns("https://test.com/confirm");
            _controller.Url = urlHelperMock.Object;

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await _controller.ResendConfirmationEmail(input);

            // Assert
            var okResult = result as JsonResult;
            Assert.IsNotNull(okResult);

            var data = okResult.Value;
            var status = data.GetType().GetProperty("status")?.GetValue(data)?.ToString();
            var msg = data.GetType().GetProperty("msg")?.GetValue(data)?.ToString();

            Assert.AreEqual("success", status);
            Assert.AreEqual("A new confirmation email has been sent.", msg);
        }

        [TestCase("abc")]               // TC02
        [TestCase("abc@gmail.com")]     // TC05
        public async Task TC02_TC05_AccountDoesNotExist(string input)
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByNameAsync(input)).ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(x => x.FindByEmailAsync(input)).ReturnsAsync((AppUser)null);

            // Act
            var result = await _controller.ResendConfirmationEmail(input);

            // Assert
            var badResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badResult);

            var data = badResult.Value;
            var status = data.GetType().GetProperty("status")?.GetValue(data)?.ToString();
            var msg = data.GetType().GetProperty("msg")?.GetValue(data)?.ToString();

            Assert.AreEqual("error", status);
            Assert.AreEqual("Account does not exist.", msg);
        }

        [Test]
        public async Task TC06_EmailAlreadyConfirmed()
        {
            // Arrange
            var user = new AppUser { UserName = "giahuy", Email = "abc@gmail.com" };
            _userManagerMock.Setup(x => x.FindByEmailAsync("abc@gmail.com")).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);

            // Act
            var result = await _controller.ResendConfirmationEmail("abc@gmail.com");

            // Assert
            var badResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badResult);

            var data = badResult.Value;
            var status = data.GetType().GetProperty("status")?.GetValue(data)?.ToString();
            var msg = data.GetType().GetProperty("msg")?.GetValue(data)?.ToString();

            Assert.AreEqual("error", status);
            Assert.AreEqual("Email has already been confirmed.", msg);
        }



        [Test]
        public async Task TC07_UnexpectedException()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByNameAsync("abc")).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.ResendConfirmationEmail("abc");

            // Assert
            var okResult = result as JsonResult;
            Assert.IsNotNull(okResult);

            var data = okResult.Value;
            var status = data.GetType().GetProperty("status")?.GetValue(data)?.ToString();
            var msg = data.GetType().GetProperty("msg")?.GetValue(data)?.ToString();

            Assert.AreEqual("error", status);
            Assert.AreEqual("Unknown error, please try again.", msg);
        }

    }
}
