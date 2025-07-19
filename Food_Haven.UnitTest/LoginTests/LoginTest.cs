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

        [TestCase("giahuy", "Password123!")] // TC01
        [TestCase("huytgcs170073@fpt.edu.vn", "Password123!")] // TC07 
        public async Task TC01_TC07_Login_WithValidCredentials_ReturnsSuccess(string userInput, string password)
        {
            var user = userInput.Contains("@")
                ? new AppUser
                {
                    Email = userInput,
                    EmailConfirmed = true,
                    IsBannedByAdmin = false
                }
                : new AppUser
                {
                    UserName = userInput,
                    EmailConfirmed = true,
                    IsBannedByAdmin = false
                };

            // Mock lookup
            _userManagerMock.Setup(u => u.FindByNameAsync(userInput))
                            .ReturnsAsync(userInput.Contains("@") ? null : user);

            _userManagerMock.Setup(u => u.FindByEmailAsync(userInput))
                            .ReturnsAsync(userInput.Contains("@") ? user : null);

            _userManagerMock.Setup(u => u.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, password)).ReturnsAsync(true);
            _userManagerMock.Setup(u => u.ResetAccessFailedCountAsync(user)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.GetTwoFactorEnabledAsync(user)).ReturnsAsync(false);

            _signInManagerMock.Setup(s => s.PasswordSignInAsync(user, password, false, true))
                              .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await _controller.Login(userInput, password, false);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var data = okResult.Value;
            var status = data.GetType().GetProperty("status")?.GetValue(data)?.ToString();
            var msg = data.GetType().GetProperty("msg")?.GetValue(data)?.ToString();

            Assert.AreEqual("success", status);
            Assert.AreEqual("Login successful", msg);
        }


        [TestCase("giahuy")] // TC02
        [TestCase("huytgcs170073@fpt.edu.vn")] // TC08 
        public async Task TC02_TC08_Login_WithBannedUser_ReturnsLockedByAdminError(string userInput)
        {
            var user = userInput.Contains("@")
                ? new AppUser
                {
                    Email = userInput,
                    EmailConfirmed = true,
                    IsBannedByAdmin = true
                }
                : new AppUser 
                {
                    UserName = userInput,
                    EmailConfirmed = true,
                    IsBannedByAdmin = true
                };
            _userManagerMock.Setup(u => u.FindByNameAsync(userInput))
                            .ReturnsAsync(userInput.Contains("@") ? null : user);

            _userManagerMock.Setup(u => u.FindByEmailAsync(userInput))
                            .ReturnsAsync(userInput.Contains("@") ? user : null);

            var result = await _controller.Login(userInput, "Password123!", false);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            var data = jsonResult.Value;
            var msg = data.GetType().GetProperty("msg")?.GetValue(data)?.ToString();

            Assert.AreEqual("Your account has been locked by the administrator.", msg);
        }

        [TestCase("giahuy", "Password123!")] // TC03 
        [TestCase("huytgcs170073@fpt.edu.vn", "Password123!")] // TC09 
        public async Task TC03_TC09_Login_WithUnconfirmedEmail_ReturnsEmailVerificationError(string userOrEmail, string password)
        {
            var user = new AppUser
            {
                UserName = userOrEmail,
                Email = userOrEmail,
                EmailConfirmed = false
            };

            _userManagerMock.Setup(u => u.FindByNameAsync(userOrEmail)).ReturnsAsync((AppUser)null); 
            _userManagerMock.Setup(u => u.FindByEmailAsync(userOrEmail)).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.IsEmailConfirmedAsync(user)).ReturnsAsync(false);

            var result = await _controller.Login(userOrEmail, password, false);
            var json = result as JsonResult;

            var msg = json.Value.GetType().GetProperty("msg")?.GetValue(json.Value)?.ToString();

            Assert.IsTrue(msg.Contains("verify your email"));
        }

        [TestCase("giahuy", "abc")] // TC04
        [TestCase("huytgcs170073@fpt.edu.vn", "abc")] // TC10
        public async Task TC04_TC10_Login_WithTooManyFailedAttempts_ReturnsLockoutError(string userInput, string password)
        {
            var user = userInput.Contains("@")
                ? new AppUser { Email = userInput, EmailConfirmed = true }
                : new AppUser { UserName = userInput, EmailConfirmed = true };

            _userManagerMock.Setup(u => u.FindByNameAsync(userInput))
                            .ReturnsAsync(userInput.Contains("@") ? null : user);

            _userManagerMock.Setup(u => u.FindByEmailAsync(userInput))
                            .ReturnsAsync(userInput.Contains("@") ? user : null);

            _userManagerMock.Setup(u => u.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, password)).ReturnsAsync(false);
            _userManagerMock.Setup(u => u.AccessFailedAsync(user)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.GetAccessFailedCountAsync(user)).ReturnsAsync(5);
            _userManagerMock.Setup(u => u.IsLockedOutAsync(user)).ReturnsAsync(true);

            var result = await _controller.Login(userInput, password, false);
            var json = result as JsonResult;

            Assert.IsNotNull(json);
            var msg = json.Value.GetType().GetProperty("msg")?.GetValue(json.Value)?.ToString();

            Assert.AreEqual("Your account has been locked due to too many failed login attempts.", msg);
        }

        [TestCase("abc", "Password123!")] // TC05 
        [TestCase("abc@gmail.com", "Password123!")] // TC11 
        public async Task TC05_TC11_Login_WithNonExistentUser_ReturnsAccountNotExistError(string userInput, string password)
        {
   
            _userManagerMock.Setup(u => u.FindByNameAsync(userInput))
                            .ReturnsAsync((AppUser)null);

            _userManagerMock.Setup(u => u.FindByEmailAsync(userInput))
                            .ReturnsAsync((AppUser)null);

            // Act
            var result = await _controller.Login(userInput, password, false);
            var json = result as JsonResult;

            Assert.IsNotNull(json);
            var msg = json.Value.GetType().GetProperty("msg")?.GetValue(json.Value)?.ToString();

            // Assert
            Assert.AreEqual("Account does not exist", msg);
        }


        [TestCase("giahuy", "abc")] // TC06
        [TestCase("huytgcs170073@fpt.edu.vn", "abc")] // TC12
        public async Task TC06_TC12_Login_WithIncorrectPassword_ReturnsFailedAttemptMessage(string userInput, string wrongPassword)
        {
            var user = userInput.Contains("@")
                ? new AppUser { Email = userInput, EmailConfirmed = true }
                : new AppUser { UserName = userInput, EmailConfirmed = true };

            _userManagerMock.Setup(u => u.FindByNameAsync(userInput))
                            .ReturnsAsync(userInput.Contains("@") ? null : user);

            _userManagerMock.Setup(u => u.FindByEmailAsync(userInput))
                            .ReturnsAsync(userInput.Contains("@") ? user : null);

            _userManagerMock.Setup(u => u.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, wrongPassword)).ReturnsAsync(false);
            _userManagerMock.Setup(u => u.AccessFailedAsync(user)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.GetAccessFailedCountAsync(user)).ReturnsAsync(1); // => 4 lần còn lại
            _userManagerMock.Setup(u => u.IsLockedOutAsync(user)).ReturnsAsync(false);

            var result = await _controller.Login(userInput, wrongPassword, false);
            var json = result as JsonResult;

            Assert.IsNotNull(json);
            var msg = json.Value.GetType().GetProperty("msg")?.GetValue(json.Value)?.ToString();

            Assert.IsTrue(msg.Contains("Incorrect password"));
            Assert.IsTrue(msg.Contains("4 attempts left"));
        }

        [Test]
        public async Task TC13_Login_ThrowsException_ReturnsUnknownError()
        {
            _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                            .ThrowsAsync(new Exception("DB error"));

            var result = await _controller.Login("testuser@example.com", "Password123!", false);
            var json = result as JsonResult;

            var msg = json.Value.GetType().GetProperty("msg")?.GetValue(json.Value)?.ToString();

            Assert.AreEqual("Unknown error, please try again.", msg);
        }

    }
}
