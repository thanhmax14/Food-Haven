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
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Hone_ResetPassword_Test
{
    public class ResetPasswordTest
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
            _expertRecipeServicesMock = new Mock<IExpertRecipeServices>();
            _recipeViewHistoryServicesMock = new Mock<IRecipeViewHistoryServices>();
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
                payOSMock,
                voucherMock.Object,
                storeReportMock.Object,
                storeFollowersMock.Object,
                recipeSearchMock,
                 _expertRecipeServicesMock.Object, // <-- Add this argument
 _recipeViewHistoryServicesMock.Object,
  hubContextMock.Object // <-- Add this argument// <-- Add this argument
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
        public async Task TC01_InvalidRequest_ModelStateInvalid()
        {
            _controller.ModelState.AddModelError("Password", "Invalid password format.");
            var model = new ResetPasswordViewModel();

            var result = await _controller.ResetPassword(model) as JsonResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);

            var json = JsonSerializer.Serialize(result.Value);
            var data = JsonSerializer.Deserialize<JsonElement>(json);

            Assert.AreEqual("error", data.GetProperty("status").GetString());
            Assert.AreEqual("Invalid password format.", data.GetProperty("msg").GetString());
        }
        [Test]
        public async Task TC02_EmailDoesNotExist_ReturnsError()
        {
            var model = new ResetPasswordViewModel
            {
                Email = Uri.EscapeDataString(EncryptData.Encrypt("nonexist@mail.com", "Xinchao123@")),
                Token = Uri.EscapeDataString(EncryptData.Encrypt("dummyToken", "Xinchao123@")),
                Password = "Test123!",
                ConfirmPassword = "Test123!"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync("nonexist@mail.com")).ReturnsAsync((AppUser)null);

            var result = await _controller.ResetPassword(model) as JsonResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);

            var json = JsonSerializer.Serialize(result.Value);
            var data = JsonSerializer.Deserialize<JsonElement>(json);

            Assert.AreEqual("error", data.GetProperty("status").GetString());
            Assert.AreEqual("Email does not exist in the system.", data.GetProperty("msg").GetString());
        }
        [Test]
        public async Task TC03_ResetSuccess_ReturnsSuccess()
        {
            var email = "user@mail.com";
            var token = "validtoken";
            var user = new AppUser { Email = email };

            var model = new ResetPasswordViewModel
            {
                Email = Uri.EscapeDataString(EncryptData.Encrypt(email, "Xinchao123@")),
                Token = Uri.EscapeDataString(EncryptData.Encrypt(token, "Xinchao123@")),
                Password = "NewPass123!",
                ConfirmPassword = "NewPass123!"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ResetPasswordAsync(user, token, model.Password))
                            .ReturnsAsync(IdentityResult.Success);

            var result = await _controller.ResetPassword(model) as JsonResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);

            var json = JsonSerializer.Serialize(result.Value);
            var data = JsonSerializer.Deserialize<JsonElement>(json);

            Assert.AreEqual("success", data.GetProperty("status").GetString());
            Assert.AreEqual("Password has been changed successfully.", data.GetProperty("msg").GetString());
        }
        [Test]
        public async Task TC04_TokenExpired_ReturnsLinkExpired()
        {
            var email = "user@mail.com";
            var token = "expiredtoken";
            var user = new AppUser { Email = email };

            var model = new ResetPasswordViewModel
            {
                Email = Uri.EscapeDataString(EncryptData.Encrypt(email, "Xinchao123@")),
                Token = Uri.EscapeDataString(EncryptData.Encrypt(token, "Xinchao123@")),
                Password = "NewPass123!",
                ConfirmPassword = "NewPass123!"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ResetPasswordAsync(user, token, model.Password))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Invalid token." }));

            var result = await _controller.ResetPassword(model) as JsonResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);

            var json = JsonSerializer.Serialize(result.Value);
            var data = JsonSerializer.Deserialize<JsonElement>(json);

            Assert.AreEqual("error", data.GetProperty("status").GetString());
            Assert.AreEqual("The password reset link has expired!", data.GetProperty("msg").GetString());
        }
        [Test]
        public async Task TC05_InvalidPasswordFormat_ReturnsPasswordFormatError()
        {
            _controller.ModelState.AddModelError("Password", "Invalid password format.");
            var model = new ResetPasswordViewModel();

            var result = await _controller.ResetPassword(model) as JsonResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);

            var json = JsonSerializer.Serialize(result.Value);
            var data = JsonSerializer.Deserialize<JsonElement>(json);

            Assert.AreEqual("error", data.GetProperty("status").GetString());
            Assert.AreEqual("Invalid password format.", data.GetProperty("msg").GetString());
        }
        [Test]
        public async Task TC06_PasswordMismatch_ReturnsConfirmPasswordError()
        {
            _controller.ModelState.AddModelError("ConfirmPassword", "Re-entered password does not match.");
            var model = new ResetPasswordViewModel();

            var result = await _controller.ResetPassword(model) as JsonResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);

            var json = JsonSerializer.Serialize(result.Value);
            var data = JsonSerializer.Deserialize<JsonElement>(json);

            Assert.AreEqual("error", data.GetProperty("status").GetString());
            Assert.AreEqual("Re-entered password does not match.", data.GetProperty("msg").GetString());
        }
        [Test]
        public async Task TC07_GenericFailure_ReturnsGenericMessage()
        {
            var email = "user@mail.com";
            var token = "validtoken";
            var user = new AppUser { Email = email };

            var model = new ResetPasswordViewModel
            {
                Email = Uri.EscapeDataString(EncryptData.Encrypt(email, "Xinchao123@")),
                Token = Uri.EscapeDataString(EncryptData.Encrypt(token, "Xinchao123@")),
                Password = "Test123!",
                ConfirmPassword = "Test123!"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ResetPasswordAsync(user, token, model.Password))
                            .ReturnsAsync(IdentityResult.Failed());

            var result = await _controller.ResetPassword(model) as JsonResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);

            var json = JsonSerializer.Serialize(result.Value);
            var data = JsonSerializer.Deserialize<JsonElement>(json);

            Assert.AreEqual("error", data.GetProperty("status").GetString());
            Assert.AreEqual("Password update failed.", data.GetProperty("msg").GetString());
        }


    }
}
