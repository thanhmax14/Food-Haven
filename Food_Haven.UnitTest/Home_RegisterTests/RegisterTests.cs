

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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Moq;
using Net.payOS;
using Repository.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Food_Haven.UnitTest.Home_RegisterTests
{
    public class RegisterTests
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
            var identityOptions = new IdentityOptions();
            identityOptions.Password.RequiredLength = 6;
            identityOptions.Password.RequireDigit = true;
            identityOptions.Password.RequireUppercase = true;

            var optionsMock = new Mock<IOptions<IdentityOptions>>();
            optionsMock.Setup(o => o.Value).Returns(identityOptions);

            _userManagerMock = new Mock<UserManager<AppUser>>(
    Mock.Of<IUserStore<AppUser>>(),
    optionsMock.Object, // <-- IdentityOptions
    new PasswordHasher<AppUser>(),
    new IUserValidator<AppUser>[0],
    new IPasswordValidator<AppUser>[0],
    new UpperInvariantLookupNormalizer(),
    new IdentityErrorDescriber(),
    new Mock<IServiceProvider>().Object,
    new Mock<ILogger<UserManager<AppUser>>>().Object
);

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
        [Test]
        public async Task TC01_RegistrationSuccess()
        {
            var model = new RegisterViewModel
            {
                Username = "giahuy",
                Email = "huytgcs170073@fpt.edu.vn",
                Password = "Password123!",
                repassword = "Password123!"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(model.Username)).ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), model.Password)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), "User")).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<AppUser>())).ReturnsAsync("token");

            _controller.IsTesting = true; 

            var result = await _controller.Register(model) as JsonResult;

            Assert.IsNotNull(result);
            var data = result.Value;
            var status = data.GetType().GetProperty("status")?.GetValue(data)?.ToString();
            var msg = data.GetType().GetProperty("msg")?.GetValue(data)?.ToString();

            Assert.AreEqual("success", status);
            Assert.AreEqual("Registration successful, please confirm your email.", msg);
        }
        [Test]
        public async Task TC02_UsernameAlreadyExists()
        {
            var model = new RegisterViewModel
            {
                Username = "abc",
                Email = "huytgcs170073@fpt.edu.vn",
                Password = "Password123!",
                repassword = "Password123!"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(model.Username)).ReturnsAsync(new AppUser());

            var result = await _controller.Register(model) as JsonResult;

            Assert.IsNotNull(result);
            var data = result.Value;
            var status = data.GetType().GetProperty("status")?.GetValue(data)?.ToString();
            var msg = data.GetType().GetProperty("msg")?.GetValue(data)?.ToString();

            Assert.AreEqual("error", status);
            Assert.AreEqual("Username already exists. Please choose a different username.", msg);
        }
        [Test]
        public async Task TC03_InvalidPasswordFormat()
        {

            var model = new RegisterViewModel
            {
                Username = "giahuy",
                Email = "huytgcs170073@fpt.edu.vn",
                Password = "abc",
                repassword = "abc"
            };
            ValidateModel(model);
            _userManagerMock
    .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
    .ReturnsAsync(IdentityResult.Failed(
        new IdentityError { Code = "PasswordTooShort", Description = "Invalid password format." }
    ));

            var result = await _controller.Register(model) as JsonResult;
            var a = result;
            Assert.IsNotNull(result);
            var data = result.Value;
            var status = data.GetType().GetProperty("status")?.GetValue(data)?.ToString();
            var msg = data.GetType().GetProperty("msg")?.GetValue(data)?.ToString();

            Assert.AreEqual("error", status);
            Assert.AreEqual("Invalid password format.", msg);
        }

        [Test]
        public async Task TC06_InvalidEmailFormat()
        {
            var model = new RegisterViewModel
            {
                Username = "giahuy",
                Email = "abcgmail.com",
                Password = "Password123!",
                repassword = "Password123!"
            };

            // Trigger ModelState validation
            ValidateModel(model);

            var result = await _controller.Register(model) as JsonResult;

            Assert.IsNotNull(result);
            var data = result.Value;
            var status = data.GetType().GetProperty("status")?.GetValue(data)?.ToString();
            var msg = data.GetType().GetProperty("msg")?.GetValue(data)?.ToString();

            Assert.AreEqual("error", status);
            Assert.AreEqual("Invalid email format.", msg);
        }
    

        [Test]
        public async Task TC04_RePasswordMismatch()
        {
           
            var model = new RegisterViewModel
            {
                Username = "giahuy",
                Email = "huytgcs170073@fpt.edu.vn",
                Password = "Password123!",
                repassword = "abc"
            };
            ValidateModel(model);
            var result = await _controller.Register(model) as JsonResult;
            var a = result;
            Assert.IsNotNull(result);
            var data = result.Value;
            var status = data.GetType().GetProperty("status")?.GetValue(data)?.ToString();
            var msg = data.GetType().GetProperty("msg")?.GetValue(data)?.ToString();

            Assert.AreEqual("error", status);
            Assert.AreEqual("Passwords do not match.", msg);
        }

      

        [Test]
        public async Task TC05_EmailAlreadyRegistered()
        {
            var model = new RegisterViewModel
            {
                Username = "giahuy",
                Email = "abc@gmail.com",
                Password = "Password123!",
                repassword = "Password123!"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(model.Username)).ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(new AppUser());

            var result = await _controller.Register(model) as JsonResult;

            Assert.IsNotNull(result);
            var data = result.Value;
            var status = data.GetType().GetProperty("status")?.GetValue(data)?.ToString();
            var msg = data.GetType().GetProperty("msg")?.GetValue(data)?.ToString();

            Assert.AreEqual("error", status);
            Assert.AreEqual("Email has already been registered. Please use a different email.", msg);
        }

        [Test]
        public async Task TC06_ExceptionThrown_ReturnsGenericError()
        {
            var model = new RegisterViewModel
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "Password123!",
                repassword = "Password123!"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(model.Username)).ThrowsAsync(new Exception("Unexpected"));

            var result = await _controller.Register(model) as JsonResult;

            Assert.IsNotNull(result);
            var data = result.Value;
            var status = data.GetType().GetProperty("status")?.GetValue(data)?.ToString();
            var msg = data.GetType().GetProperty("msg")?.GetValue(data)?.ToString();

            Assert.AreEqual("error", status);
            Assert.AreEqual("Unknown error, please try again.", msg);
        }
        private void ValidateModel(object model)
        {
            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults, true);

            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    _controller.ModelState.AddModelError(memberName, validationResult.ErrorMessage);
                }
            }
        }
    }
}
