using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.ComplaintImages;
using BusinessLogic.Services.Complaints;
using BusinessLogic.Services.FavoriteFavoriteRecipes;
using BusinessLogic.Services.IngredientTagServices;
using BusinessLogic.Services.Message;
using BusinessLogic.Services.MessageImages;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.RecipeIngredientTagIngredientTagServices;
using BusinessLogic.Services.RecipeReviewReviews;
using BusinessLogic.Services.RecipeServices;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using BusinessLogic.Services.StoreFollowers;
using BusinessLogic.Services.TypeOfDishServices;
using BusinessLogic.Services.VoucherServices;
using Food_Haven.Web.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.DBContext;
using Moq;
using Net.payOS;
using Net.payOS.Types;
using NUnit.Framework;
using Repository.BalanceChange;
using Repository.ViewModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.User_AddBalance_Test
{
    [TestFixture]
    public class AddBalance_Test
    {
        // FakePayOS để không gọi API thật
        public class FakePayOS : PayOS
        {
            public FakePayOS() : base("client-id", "api-key", "https://callback.url") { }

            public new Task<CreatePaymentResult> createPaymentLink(PaymentData paymentData)
            {
                // Return a CreatePaymentResult with realistic, non-null values for all required fields
                return Task.FromResult(new CreatePaymentResult(
            paymentLinkId: "mocked-link-id",
            status: "mock-status",
            amount: paymentData.amount,
            currency: "VND",
            expiredAt: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            checkoutUrl: "https://pay.payos.vn/web/mock-checkout",
            qrCode: "mock-qr-code",
            orderCode: 1234567890,
            bin: "mock-bin",
            accountNumber: "mock-account",
            description: "mock-description"
        ));
            }
        }

        private UsersController _controller;
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<HttpClient> _httpClientMock;
        private Mock<IBalanceChangeService> _balanceChangeServiceMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private PayOS _payOS;
        private ManageTransaction _manageTransaction;
        private FoodHavenDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FoodHavenDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new FoodHavenDbContext(options);

            _userManagerMock = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            _httpClientMock = new Mock<HttpClient>();
            _balanceChangeServiceMock = new Mock<IBalanceChangeService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _payOS = new PayOS("client-id", "api-key", "https://callback.url");
            _manageTransaction = new ManageTransaction(_dbContext);

            // Mock HttpContext + ClaimsPrincipal
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user123")
            }, "mock"));
            var context = new DefaultHttpContext { User = claims };
            _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(context);

            _controller = new UsersController(
                _userManagerMock.Object,
                _httpClientMock.Object,
                _balanceChangeServiceMock.Object,
                _httpContextAccessorMock.Object,
                Mock.Of<IProductService>(),
                Mock.Of<ICartService>(),
                Mock.Of<IProductVariantService>(),
                Mock.Of<IProductImageService>(),
                Mock.Of<IOrdersServices>(),
                Mock.Of<IOrderDetailService>(),
                _payOS,
                _manageTransaction,
                Mock.Of<IReviewService>(),
                Mock.Of<IRecipeService>(),
                Mock.Of<ICategoryService>(),
                Mock.Of<IIngredientTagService>(),
                Mock.Of<ITypeOfDishService>(),
                Mock.Of<IComplaintImageServices>(),
                Mock.Of<IComplaintServices>(),
                Mock.Of<IRecipeIngredientTagIngredientTagSerivce>(),
                Mock.Of<IMessageImageService>(),
                Mock.Of<IMessageService>(),
                Mock.Of<IHubContext<ChatHub>>(),
                Mock.Of<IRecipeReviewService>(),
                Mock.Of<IFavoriteRecipeService>(),
                Mock.Of<IStoreFollowersService>(),
                Mock.Of<IStoreDetailService>(),
                Mock.Of<IHubContext<FollowHub>>(),
                Mock.Of<IVoucherServices>()
            );
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
            _manageTransaction?.Dispose();
            _dbContext?.Dispose();
        }

        [Test]
        public async Task AddBalance_Should_Return_Success_When_ValidUser_And_ValidAmount()
        {
            var user = new AppUser { Id = "user123", UserName = "testuser" };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
            _balanceChangeServiceMock.Setup(b => b.GetBalance(user.Id)).ReturnsAsync(500000);
            _balanceChangeServiceMock.Setup(b => b.FindAsync(It.IsAny<Expression<Func<BalanceChange, bool>>>())).ReturnsAsync((BalanceChange)null);

            // Tạo HttpContext giả có Request
            var context = new DefaultHttpContext();
            context.Request.Scheme = "https";
            context.Request.Host = new HostString("localhost");
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "user123") }, "mock"));
            _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(context);

            // Thay thế PayOS bằng FakePayOS
            var fakePayOS = new FakePayOS();
            var payosField = typeof(UsersController)
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(f => f.Name == "_payos" && f.FieldType == typeof(PayOS));
            Assert.IsNotNull(payosField, "Không tìm thấy field _payos trong UsersController");
            payosField.SetValue(_controller, fakePayOS);

            var result = await _controller.AddBalance(100000) as JsonResult;

            Assert.IsNotNull(result, "JsonResult bị null");
            var erroMess = result?.Value as ErroMess;
            Assert.IsNotNull(erroMess, "ErroMess bị null");
            Assert.IsTrue(erroMess.success);
            StringAssert.Contains("https://pay.payos.vn/web/", erroMess.msg?.ToString());
        }

        [Test]
        public async Task AddBalance_Should_Return_Error_When_User_Not_Logged_In()
        {
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AppUser)null);

            var result = await _controller.AddBalance(100000);
            var json = result as JsonResult;
            Assert.IsNotNull(json);

            var props = json.Value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(json.Value));
            Assert.IsTrue((bool)props["notAuth"]);
            Assert.AreEqual("You must be logged in to perform this action!", props["message"]);
        }

        [Test]
        public async Task AddBalance_Should_Return_Error_When_Amount_Less_Than_Minimum()
        {
            var user = new AppUser { Id = "user123" };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var result = await _controller.AddBalance(90000) as JsonResult;

            Assert.IsNotNull(result);
            var erroMess = result.Value as ErroMess;
            Assert.AreEqual("Minimum deposit is 100,000 VND", erroMess.msg);
        }

        [Test]
        public async Task AddBalance_Should_Return_Error_When_User_Not_Exist_In_System()
        {
            var user = new AppUser { Id = "user123" };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync((AppUser)null);

            var result = await _controller.AddBalance(100000) as JsonResult;

            Assert.IsNotNull(result);
            var erroMess = result.Value as ErroMess;
            Assert.AreEqual("User does not exist in the system", erroMess.msg);
        }
    }
}
