using BusinessLogic.Hash;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.RecipeServices;
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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Hone_ConfirmEmail_Test
{
    public class ConfirmEmailTest
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<SignInManager<AppUser>> _signInManagerMock;
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
                recipeSearchMock
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
        public async Task TC01_EmailConfirmationSuccess_ReturnsSuccessMessage()
        {
            var encryptedUserId = Uri.EscapeDataString(EncryptData.Encrypt("user123", "Xinchao123@"));
            var encryptedToken = Uri.EscapeDataString(EncryptData.Encrypt("token456", "Xinchao123@"));
            var user = new AppUser { Id = "user123", UserName = "testuser" };

            _userManagerMock.Setup(x => x.FindByIdAsync("user123")).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ConfirmEmailAsync(user, "token456")).ReturnsAsync(IdentityResult.Success);

            var result = await _controller.ConfirmEmail(encryptedUserId, encryptedToken) as JsonResult;

            Assert.IsNotNull(result);

            var json = JsonConvert.SerializeObject(result.Value);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(json);

            Assert.IsTrue((bool)data.success.Value);
            Assert.AreEqual("Email confirmed successfully!", (string)data.message.Value);
        }

        [Test]
        public async Task TC02_NullUserIdOrToken_ReturnsInvalidRequest()
        {
            var result = await _controller.ConfirmEmail(null, null) as JsonResult;

            Assert.IsNotNull(result);

            var json = JsonConvert.SerializeObject(result.Value);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(json);

            Assert.IsFalse((bool)data.success.Value);
            Assert.AreEqual("Invalid request.", (string)data.message.Value);
        }

        [Test]
        public async Task TC03_UserNotFound_ReturnsUserNotFound()
        {
            var encryptedUserId = Uri.EscapeDataString(EncryptData.Encrypt("user123", "Xinchao123@"));
            var encryptedToken = Uri.EscapeDataString(EncryptData.Encrypt("token456", "Xinchao123@"));

            _userManagerMock.Setup(x => x.FindByIdAsync("user123")).ReturnsAsync((AppUser)null);

            var result = await _controller.ConfirmEmail(encryptedUserId, encryptedToken) as JsonResult;

            Assert.IsNotNull(result);

            var json = JsonConvert.SerializeObject(result.Value);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(json);

            Assert.IsFalse((bool)data.success.Value);
            Assert.AreEqual("User not found.", (string)data.message.Value);
        }

        [Test]
        public async Task TC04_EmailConfirmationFails_ReturnsLinkExpired()
        {
            var encryptedUserId = Uri.EscapeDataString(EncryptData.Encrypt("user123", "Xinchao123@"));
            var encryptedToken = Uri.EscapeDataString(EncryptData.Encrypt("token456", "Xinchao123@"));
            var user = new AppUser { Id = "user123", UserName = "testuser" };

            _userManagerMock.Setup(x => x.FindByIdAsync("user123")).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ConfirmEmailAsync(user, "token456"))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Invalid token." }));

            var result = await _controller.ConfirmEmail(encryptedUserId, encryptedToken) as JsonResult;

            Assert.IsNotNull(result);

            var json = JsonConvert.SerializeObject(result.Value);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(json);

            Assert.IsFalse((bool)data.success.Value);
            Assert.AreEqual("The link has expired or is invalid.", (string)data.message.Value);
        }
        [Test]
        public async Task TC05_EmptyInput_ReturnsInvalidRequest()
        {
            var result = await _controller.ConfirmEmail(null, null) as JsonResult;

            Assert.IsNotNull(result);

            var json = JsonConvert.SerializeObject(result.Value);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(json);

            Assert.IsFalse((bool)data.success.Value);
            Assert.AreEqual("Invalid request.", (string)data.message.Value);
        }

    }
}
