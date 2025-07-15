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
using NUnit.Framework;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.LoginTests
{
    public class HomeControllerLoginTests
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
        public async Task Login_WithUnconfirmedEmail_ShouldReturnError()
        {
            // Arrange
            var user = new AppUser
            {
                UserName = "unconfirmed@gmail.com",
                Email = "unconfirmed@gmail.com",
                EmailConfirmed = false
            };

            _userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                            .ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(u => u.FindByEmailAsync("unconfirmed@gmail.com"))
                            .ReturnsAsync(user);
            _userManagerMock.Setup(u => u.IsEmailConfirmedAsync(user))
                            .ReturnsAsync(false);
            var result = await _controller.Login("unconfirmed@gmail.com", "Password123!", false);
            var jsonResult = result as JsonResult;
            Assert.NotNull(jsonResult);
            Console.WriteLine($"JsonResult.Value: {jsonResult.Value}");
            Console.WriteLine($"JsonResult.Value.GetType(): {jsonResult.Value?.GetType()}");

            var data = jsonResult.Value;
            var statusProp = data.GetType().GetProperty("status");
            var msgProp = data.GetType().GetProperty("msg");

            Assert.NotNull(statusProp);
            Assert.NotNull(msgProp);

            Assert.AreEqual("error", statusProp.GetValue(data)?.ToString());
            Assert.IsTrue(msgProp.GetValue(data)?.ToString().Contains("verify your email"));
        }


        [Test]
        public async Task Login_WithEmptyUsername_ShouldReturnError()
        {
            // Arrange
            string username = "";
            string password = "Password123!";

            // Act
            var result = await _controller.Login(username, password, false);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = jsonResult.Value;
            var statusProp = data.GetType().GetProperty("status");
            var msgProp = data.GetType().GetProperty("msg");

            Assert.AreEqual("error", statusProp.GetValue(data)?.ToString());
            Assert.AreEqual("Username cannot be empty", msgProp.GetValue(data)?.ToString());
        }

        [Test]
        public async Task Login_WithEmptyPassword_ShouldReturnError()
        {
            // Arrange
            string username = "testuser";
            string password = "";

            // Act
            var result = await _controller.Login(username, password, false);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = jsonResult.Value;
            var statusProp = data.GetType().GetProperty("status");
            var msgProp = data.GetType().GetProperty("msg");

            Assert.AreEqual("error", statusProp.GetValue(data)?.ToString());
            Assert.AreEqual("Password cannot be empty", msgProp.GetValue(data)?.ToString());
        }

        [Test]
        public async Task Login_WithNonExistentUser_ShouldReturnError()
        {
            // Arrange
            string username = "nonexistent@example.com";
            string password = "Password123!";

            _userManagerMock.Setup(u => u.FindByNameAsync(username))
                            .ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(u => u.FindByEmailAsync(username))
                            .ReturnsAsync((AppUser)null);

            // Act
            var result = await _controller.Login(username, password, false);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = jsonResult.Value;
            var statusProp = data.GetType().GetProperty("status");
            var msgProp = data.GetType().GetProperty("msg");

            Assert.AreEqual("error", statusProp.GetValue(data)?.ToString());
            Assert.AreEqual("Account does not exist", msgProp.GetValue(data)?.ToString());
        }

        [Test]
        public async Task Login_WithBannedUser_ShouldReturnError()
        {
            // Arrange
            var user = new AppUser
            {
                UserName = "banneduser@example.com",
                Email = "banneduser@example.com",
                EmailConfirmed = true,
                IsBannedByAdmin = true
            };

            _userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                            .ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(u => u.FindByEmailAsync("banneduser@example.com"))
                            .ReturnsAsync(user);

            // Act
            var result = await _controller.Login("banneduser@example.com", "Password123!", false);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = jsonResult.Value;
            var statusProp = data.GetType().GetProperty("status");
            var msgProp = data.GetType().GetProperty("msg");

            Assert.AreEqual("error", statusProp.GetValue(data)?.ToString());
            Assert.AreEqual("Your account has been locked by the administrator.", msgProp.GetValue(data)?.ToString());
        }
        [Test]
        public async Task Login_WithIncorrectPassword_ShouldReturnError()
        {
            // Arrange
            var user = new AppUser
            {
                UserName = "testuser@example.com",
                Email = "testuser@example.com",
                EmailConfirmed = true,
                IsBannedByAdmin = false
            };

            _userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                            .ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(u => u.FindByEmailAsync("testuser@example.com"))
                            .ReturnsAsync(user);
            _userManagerMock.Setup(u => u.IsEmailConfirmedAsync(user))
                            .ReturnsAsync(true);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, "WrongPassword"))
                            .ReturnsAsync(false);
            _userManagerMock.Setup(u => u.AccessFailedAsync(user))
                            .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.GetAccessFailedCountAsync(user))
                            .ReturnsAsync(1);
            _userManagerMock.Setup(u => u.IsLockedOutAsync(user))
                            .ReturnsAsync(false);

            // Act
            var result = await _controller.Login("testuser@example.com", "WrongPassword", false);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = jsonResult.Value;
            var statusProp = data.GetType().GetProperty("status");
            var msgProp = data.GetType().GetProperty("msg");

            Assert.AreEqual("error", statusProp.GetValue(data)?.ToString());
            Assert.IsTrue(msgProp.GetValue(data)?.ToString().Contains("Incorrect password"));
            Assert.IsTrue(msgProp.GetValue(data)?.ToString().Contains("4 attempts left"));
        }

        [Test]
        public async Task Login_WithTooManyFailedAttempts_ShouldReturnLockoutError()
        {
            // Arrange
            var user = new AppUser
            {
                UserName = "testuser@example.com",
                Email = "testuser@example.com",
                EmailConfirmed = true,
                IsBannedByAdmin = false
            };

            _userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                            .ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(u => u.FindByEmailAsync("testuser@example.com"))
                            .ReturnsAsync(user);
            _userManagerMock.Setup(u => u.IsEmailConfirmedAsync(user))
                            .ReturnsAsync(true);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, "WrongPassword"))
                            .ReturnsAsync(false);
            _userManagerMock.Setup(u => u.AccessFailedAsync(user))
                            .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.GetAccessFailedCountAsync(user))
                            .ReturnsAsync(5);
            _userManagerMock.Setup(u => u.IsLockedOutAsync(user))
                            .ReturnsAsync(true);

            // Act
            var result = await _controller.Login("testuser@example.com", "WrongPassword", false);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = jsonResult.Value;
            var statusProp = data.GetType().GetProperty("status");
            var msgProp = data.GetType().GetProperty("msg");

            Assert.AreEqual("error", statusProp.GetValue(data)?.ToString());
            Assert.AreEqual("Your account has been locked due to too many failed login attempts.", msgProp.GetValue(data)?.ToString());
        }

        [Test]
        public async Task Login_WithTwoFactorEnabled_ShouldReturnVerifyStatus()
        {
            // Arrange
            var user = new AppUser
            {
                UserName = "testuser@example.com",
                Email = "testuser@example.com",
                EmailConfirmed = true,
                IsBannedByAdmin = false
            };

            _userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                            .ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(u => u.FindByEmailAsync("testuser@example.com"))
                            .ReturnsAsync(user);
            _userManagerMock.Setup(u => u.IsEmailConfirmedAsync(user))
                            .ReturnsAsync(true);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, "Password123!"))
                            .ReturnsAsync(true);
            _userManagerMock.Setup(u => u.ResetAccessFailedCountAsync(user))
                            .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.GetTwoFactorEnabledAsync(user))
                            .ReturnsAsync(true);
            _userManagerMock.Setup(u => u.GenerateTwoFactorTokenAsync(user, "Authenticator"))
                            .ReturnsAsync("123456");

            _signInManagerMock.Setup(s => s.PasswordSignInAsync(user, "Password123!", false, true))
                             .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await _controller.Login("testuser@example.com", "Password123!", false);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = jsonResult.Value;
            var statusProp = data.GetType().GetProperty("status");
            var msgProp = data.GetType().GetProperty("msg");
            var tokenProp = data.GetType().GetProperty("token");

            Assert.AreEqual("verify", statusProp.GetValue(data)?.ToString());
            Assert.AreEqual("Enter the verification code from your authenticator app.", msgProp.GetValue(data)?.ToString());
            Assert.AreEqual("123456", tokenProp.GetValue(data)?.ToString());
        }

        [Test]
        public async Task Login_WithValidCredentials_ShouldReturnSuccess()
        {
            // Arrange
            var user = new AppUser
            {
                UserName = "testuser@example.com",
                Email = "testuser@example.com",
                EmailConfirmed = true,
                IsBannedByAdmin = false
            };

            _userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                            .ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(u => u.FindByEmailAsync("testuser@example.com"))
                            .ReturnsAsync(user);
            _userManagerMock.Setup(u => u.IsEmailConfirmedAsync(user))
                            .ReturnsAsync(true);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, "Password123!"))
                            .ReturnsAsync(true);
            _userManagerMock.Setup(u => u.ResetAccessFailedCountAsync(user))
                            .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.GetTwoFactorEnabledAsync(user))
                            .ReturnsAsync(false);

            _signInManagerMock.Setup(s => s.PasswordSignInAsync(user, "Password123!", false, true))
                             .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await _controller.Login("testuser@example.com", "Password123!", false);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var data = okResult.Value;
            var statusProp = data.GetType().GetProperty("status");
            var msgProp = data.GetType().GetProperty("msg");
            var redirectUrlProp = data.GetType().GetProperty("redirectUrl");

            Assert.AreEqual("success", statusProp.GetValue(data)?.ToString());
            Assert.AreEqual("Login successful", msgProp.GetValue(data)?.ToString());
            Assert.AreEqual("/", redirectUrlProp.GetValue(data)?.ToString());
        }

        [Test]
        public async Task Login_WithSignInResultLockedOut_ShouldReturnError()
        {
            // Arrange
            var user = new AppUser
            {
                UserName = "testuser@example.com",
                Email = "testuser@example.com",
                EmailConfirmed = true,
                IsBannedByAdmin = false
            };

            _userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                            .ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(u => u.FindByEmailAsync("testuser@example.com"))
                            .ReturnsAsync(user);
            _userManagerMock.Setup(u => u.IsEmailConfirmedAsync(user))
                            .ReturnsAsync(true);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, "Password123!"))
                            .ReturnsAsync(true);
            _userManagerMock.Setup(u => u.ResetAccessFailedCountAsync(user))
                            .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.GetTwoFactorEnabledAsync(user))
                            .ReturnsAsync(false);

            _signInManagerMock.Setup(s => s.PasswordSignInAsync(user, "Password123!", false, true))
                             .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

            // Act
            var result = await _controller.Login("testuser@example.com", "Password123!", false);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = jsonResult.Value;
            var statusProp = data.GetType().GetProperty("status");
            var msgProp = data.GetType().GetProperty("msg");

            Assert.AreEqual("error", statusProp.GetValue(data)?.ToString());
            Assert.AreEqual("Your account has been locked.", msgProp.GetValue(data)?.ToString());
        }

        [Test]
        public async Task Login_WithException_ShouldReturnUnknownError()
        {
            // Arrange
            _userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                            .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Login("testuser@example.com", "Password123!", false);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = jsonResult.Value;
            var statusProp = data.GetType().GetProperty("status");
            var msgProp = data.GetType().GetProperty("msg");

            Assert.AreEqual("error", statusProp.GetValue(data)?.ToString());
            Assert.AreEqual("Unknown error, please try again.", msgProp.GetValue(data)?.ToString());
        }

        [Test]
        public async Task Login_WithCustomReturnUrl_ShouldSetCorrectRedirectUrl()
        {
            // Arrange
            var user = new AppUser
            {
                UserName = "testuser@example.com",
                Email = "testuser@example.com",
                EmailConfirmed = true,
                IsBannedByAdmin = false
            };

            _userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                            .ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(u => u.FindByEmailAsync("testuser@example.com"))
                            .ReturnsAsync(user);
            _userManagerMock.Setup(u => u.IsEmailConfirmedAsync(user))
                            .ReturnsAsync(true);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, "Password123!"))
                            .ReturnsAsync(true);
            _userManagerMock.Setup(u => u.ResetAccessFailedCountAsync(user))
                            .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.GetTwoFactorEnabledAsync(user))
                            .ReturnsAsync(false);

            _signInManagerMock.Setup(s => s.PasswordSignInAsync(user, "Password123!", false, true))
                             .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Mock URL helper
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(u => u.IsLocalUrl("/dashboard")).Returns(true);
            urlHelperMock.Setup(u => u.Action("Login", "Home")).Returns("/Home/Login");
            urlHelperMock.Setup(u => u.Action("Register", "Home")).Returns("/Home/Register");
            urlHelperMock.Setup(u => u.Action("Forgot", "Home")).Returns("/Home/Forgot");
            urlHelperMock.Setup(u => u.Action("ResetPassword", "Home")).Returns("/Home/ResetPassword");

            _controller.Url = urlHelperMock.Object;

            // Act
            var result = await _controller.Login("testuser@example.com", "Password123!", false, "/dashboard");

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var data = okResult.Value;
            var redirectUrlProp = data.GetType().GetProperty("redirectUrl");

            Assert.AreEqual("/dashboard", redirectUrlProp.GetValue(data)?.ToString());
        }

    }
}
