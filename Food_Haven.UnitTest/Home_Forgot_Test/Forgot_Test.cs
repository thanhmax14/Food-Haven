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

namespace Food_Haven.UnitTest.Home_Forgot_Test
{
    public class Forgot_Test
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

            // 🔹 Quan trọng: Khởi tạo hai mock này trước khi truyền vào controller
            expertRecipeServicesMock = new Mock<IExpertRecipeServices>();
            recipeViewHistoryServicesMock = new Mock<IRecipeViewHistoryServices>();

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
                payOS,
                voucherMock.Object,
                storeReportMock.Object,
                storeFollowersMock.Object,
                recipeSearchMock,
                expertRecipeServicesMock.Object,
                recipeViewHistoryServicesMock.Object,
                hubContextMock.Object
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
        public async Task TC01_ForgotPasswordSuccess()
        {
            // Arrange
            var email = "huytgcs170073@fpt.edu.vn";
            var user = new AppUser { Email = email };

            _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("token");

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns("https://test.com/reset");
            _controller.Url = urlHelperMock.Object;

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await _controller.Forgot(email);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = jsonResult.Value;
            var status = data.GetType().GetProperty("status")?.GetValue(data)?.ToString();
            var msg = data.GetType().GetProperty("msg")?.GetValue(data)?.ToString();

            Assert.AreEqual("success", status);
            Assert.AreEqual("A password reset email has been sent. Please check your email.", msg);
        }
        [Test]
        public async Task TC02_EmailDoesNotExist()
        {
            // Arrange
            var email = "abc@gmail.com";
            _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((AppUser)null);

            // Act
            var result = await _controller.Forgot(email);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = jsonResult.Value;
            var status = data.GetType().GetProperty("status")?.GetValue(data)?.ToString();
            var msg = data.GetType().GetProperty("msg")?.GetValue(data)?.ToString();

            Assert.AreEqual("error", status);
            Assert.AreEqual("Email does not exist in the system.", msg);
        }
        [Test]
        public async Task TC03_ExceptionThrown_ReturnsUnknownError()
        {
            // Arrange
            var email = "abc@gmail.com";
            _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await _controller.Forgot(email);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = jsonResult.Value;
            var status = data.GetType().GetProperty("status")?.GetValue(data)?.ToString();
            var msg = data.GetType().GetProperty("msg")?.GetValue(data)?.ToString();

            Assert.AreEqual("error", status);
            Assert.AreEqual("Unknown error, please try again.", msg);
        }

    }
}
