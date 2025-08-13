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
using Food_Haven.Web.Controllers;
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
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Repository.BalanceChange;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.User_SubmitPayment_Test
{
    // Fake PayOS để dùng trong test
    public class FakePayOS : PayOS
    {
        public FakePayOS() : base("client-id", "api-key", "https://callback.url") { }

        public new Task<CreatePaymentResult> createPaymentLink(PaymentData paymentData)
        {
            return Task.FromResult(new CreatePaymentResult(
                bin: "mock-bin",
                accountNumber: "mock-account",
                amount: paymentData.amount,
                description: "mock-description",
                orderCode: 1234567890,
                currency: "VND",
                paymentLinkId: "mocked-link-id",
                status: "mock-status",
                expiredAt: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                checkoutUrl: "https://pay.payos.vn/web/mock-checkout",
                qrCode: "mock-qr-code"
            ));
        }
    }

    [TestFixture]
    public class SubmitPayment_FullTest
    {
        private UsersController _controller;
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<ICartService> _cartMock;
        private Mock<IProductVariantService> _productVariantMock;
        private Mock<IProductService> _productMock;
        private Mock<IProductImageService> _imgMock;
        private Mock<IBalanceChangeService> _balanceMock;
        private Mock<IVoucherServices> _voucherMock;
        private Mock<ICategoryService> _categoryServiceMock;
        private ManageTransaction _manageTransaction;
        private FakePayOS _payos;
        private AppUser _user;
        private Guid _productId;
        private FoodHavenDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            // Khởi tạo dữ liệu cơ bản
            _user = new AppUser
            {
                Id = "user1",
                UserName = "tester",
                IsProfileUpdated = true
            };
            _productId = Guid.NewGuid();

            // DbContext InMemory
            var options = new DbContextOptionsBuilder<FoodHavenDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new FoodHavenDbContext(options);

            // Mock ManageTransaction
            var manageTransactionMock = new Mock<ManageTransaction>(_dbContext);
            manageTransactionMock
                .Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(async func =>
                {
                    await func();
                    return true;
                });
            _manageTransaction = manageTransactionMock.Object;

            // Mock services
            _userManagerMock = new Mock<UserManager<AppUser>>(
                Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null
            );
            _cartMock = new Mock<ICartService>();
            _productVariantMock = new Mock<IProductVariantService>();
            _productMock = new Mock<IProductService>();
            _imgMock = new Mock<IProductImageService>();
            _balanceMock = new Mock<IBalanceChangeService>();
            _voucherMock = new Mock<IVoucherServices>();
            _categoryServiceMock = new Mock<ICategoryService>();

            // Fake PayOS
            _payos = new FakePayOS();

            // Tạo HttpContext và session
            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, _user.Id)
        }))
            };
            httpContext.Session = new TestSession();

            // Khởi tạo controller
            _controller = new UsersController(
                _userManagerMock.Object,
                new HttpClient(),
                _balanceMock.Object,
                Mock.Of<IHttpContextAccessor>(a => a.HttpContext == httpContext),
                _productMock.Object,
                _cartMock.Object,
                _productVariantMock.Object,
                _imgMock.Object,
                Mock.Of<IOrdersServices>(),
                Mock.Of<IOrderDetailService>(),
                _payos,
                _manageTransaction,
                Mock.Of<IReviewService>(),
                Mock.Of<IRecipeService>(),
                _categoryServiceMock.Object,
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
                _voucherMock.Object
            );

            // Gán trực tiếp HttpContext
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }


        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
            _dbContext?.Dispose();
        }

        private void SetBillingTourInfo(int quantity, decimal price, string voucher = null)
        {
            if (_productId == Guid.Empty)
                _productId = Guid.NewGuid();

            var checkout = new CheckOutView
            {
                itemCheck = new List<ListItems>
                {
                    new ListItems
                    {
                        productID = _productId,
                        ItemQuantity = quantity,
                        ItemPrice = price
                    }
                },
                voucher = voucher
            };

            if (_controller.HttpContext?.Session == null)
                _controller.HttpContext.Session = new TestSession();

            _controller.HttpContext.Session.Set(
                "BillingTourInfo",
                JsonSerializer.SerializeToUtf8Bytes(checkout)
            );
        }

        private JsonResult CallAndGetJson(OrderInputModel model, string paymentOption)
        {
            var task = _controller.SubmitPayment(model, paymentOption);
            task.Wait();
            return task.Result as JsonResult;
        }

        // ==== TEST CASES ====

        [Test]
        public void ShouldReturnError_WhenUserNull()
        {
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AppUser)null);
            var result = CallAndGetJson(new OrderInputModel(), "OnlineGateway");
            Assert.AreEqual("You are not logged in!", ((ErroMess)result.Value).msg);
        }

        [Test]
        public void ShouldReturnError_WhenPaymentOptionNull()
        {
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(_user);
            var result = CallAndGetJson(new OrderInputModel(), null);
            Assert.AreEqual("Please select a payment method!", ((ErroMess)result.Value).msg);
        }

        [Test]
        public void ShouldReturnError_WhenModelStateInvalid()
        {
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(_user);
            _controller.ModelState.AddModelError("Address", "Required");
            var result = CallAndGetJson(new OrderInputModel(), "Wallet");
            Assert.AreEqual("Required", ((ErroMess)result.Value).msg);
        }

        [Test]
        public void ShouldReturnError_WhenNoProductsInSession_OnlineGateway()
        {
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(_user);
            var checkout = new CheckOutView { itemCheck = new List<ListItems>() };
            _controller.HttpContext.Session.Set("BillingTourInfo", JsonSerializer.SerializeToUtf8Bytes(checkout));
            var result = CallAndGetJson(new OrderInputModel(), "OnlineGateway");
            Assert.AreEqual("Please select the products you want to buy!", ((ErroMess)result.Value).msg);
        }

        [Test]
        public void ShouldReturnError_WhenCartNotFound_Wallet()
        {
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(_user);
            SetBillingTourInfo(1, 100);
            _cartMock.Setup(c => c.FindAsync(It.IsAny<Expression<Func<Cart, bool>>>())).ReturnsAsync((Cart)null);
            var result = CallAndGetJson(new OrderInputModel(), "Wallet");
            Assert.AreEqual("A product is not in your cart!", ((ErroMess)result.Value).msg);
        }

        [Test]
        public void ShouldReturnError_WhenStockExceeded_Wallet()
        {
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(_user);
            SetBillingTourInfo(5, 100);
            _cartMock.Setup(c => c.FindAsync(It.IsAny<Expression<Func<Cart, bool>>>())).ReturnsAsync(new Cart { ProductTypesID = _productId, Quantity = 5 });
            _productVariantMock.Setup(v => v.FindAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>())).ReturnsAsync(new ProductTypes { ID = _productId, Stock = 2, SellPrice = 100 });
            var result = CallAndGetJson(new OrderInputModel(), "Wallet");
            Assert.AreEqual("Not enough stock for a product.", ((ErroMess)result.Value).msg);
        }

        [Test]
        public void ShouldReturnError_WhenInsufficientBalance_Wallet()
        {
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(_user);
            SetBillingTourInfo(1, 100);
            _cartMock.Setup(c => c.FindAsync(It.IsAny<Expression<Func<Cart, bool>>>())).ReturnsAsync(new Cart());
            _productVariantMock.Setup(v => v.FindAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>())).ReturnsAsync(new ProductTypes { ID = _productId, Stock = 10, SellPrice = 100 });
            _balanceMock.Setup(b => b.CheckMoney(_user.Id, It.IsAny<decimal>())).ReturnsAsync(false);
            var result = CallAndGetJson(new OrderInputModel(), "Wallet");
            Assert.AreEqual("Insufficient balance to make a purchase!", ((ErroMess)result.Value).msg);
        }

        [Test]
        public void ShouldReturnError_WhenInvalidPaymentOption()
        {
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(_user);
            var result = CallAndGetJson(new OrderInputModel(), "BankTransfer");
            Assert.AreEqual("Invalid payment method!", ((ErroMess)result.Value).msg);
        }

        
    }

    public class TestSession : ISession
    {
        private Dictionary<string, byte[]> _sessionStorage = new Dictionary<string, byte[]>();
        public IEnumerable<string> Keys => _sessionStorage.Keys;
        public string Id => Guid.NewGuid().ToString();
        public bool IsAvailable => true;
        public void Clear() => _sessionStorage.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _sessionStorage.Remove(key);
        public void Set(string key, byte[] value) => _sessionStorage[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);
    }
}
