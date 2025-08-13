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
using Models;
using Moq;
using Moq.Protected;
using Net.payOS;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.User_Withdraw_Test
{
    [TestFixture]
    public class Withdraw_Test
    {
        private UsersController _controller;

        // Mocks and real instances for constructor dependencies
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<HttpClient> _httpClientMock;
        private Mock<IBalanceChangeService> _balanceChangeServiceMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IProductService> _productServiceMock;
        private Mock<ICartService> _cartServiceMock;
        private Mock<IProductVariantService> _productVariantServiceMock;
        private Mock<IProductImageService> _productImageServiceMock;
        private Mock<IOrdersServices> _ordersServiceMock;
        private Mock<IOrderDetailService> _orderDetailServiceMock;
        private PayOS _payOS; // Real instance
        private ManageTransaction _manageTransaction; // Real instance
        private Mock<IReviewService> _reviewServiceMock;
        private Mock<IRecipeService> _recipeServiceMock;
        private Mock<ICategoryService> _categoryServiceMock;
        private Mock<IIngredientTagService> _ingredientTagServiceMock;
        private Mock<ITypeOfDishService> _typeOfDishServiceMock;
        private Mock<IComplaintImageServices> _complaintImageServicesMock;
        private Mock<IComplaintServices> _complaintServicesMock;
        private Mock<IRecipeIngredientTagIngredientTagSerivce> _recipeIngredientTagServiceMock;
        private Mock<IMessageImageService> _messageImageServiceMock;
        private Mock<IMessageService> _messageServiceMock;
        private Mock<IHubContext<ChatHub>> _chatHubContextMock;
        private Mock<IRecipeReviewService> _recipeReviewServiceMock;
        private Mock<IFavoriteRecipeService> _favoriteRecipeServiceMock;
        private Mock<IStoreFollowersService> _storeFollowersServiceMock;
        private Mock<IStoreDetailService> _storeDetailServiceMock;
        private Mock<IHubContext<FollowHub>> _followHubContextMock;
        private Mock<IVoucherServices> _voucherServiceMock;

        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            _httpClientMock = new Mock<HttpClient>();
            _balanceChangeServiceMock = new Mock<IBalanceChangeService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _productServiceMock = new Mock<IProductService>();
            _cartServiceMock = new Mock<ICartService>();
            _productVariantServiceMock = new Mock<IProductVariantService>();
            _productImageServiceMock = new Mock<IProductImageService>();
            _ordersServiceMock = new Mock<IOrdersServices>();
            _orderDetailServiceMock = new Mock<IOrderDetailService>();
            _payOS = new PayOS("client-id", "api-key", "https://callback.url"); // Real instance
            _manageTransaction = null; // Real instance (adjust constructor if needed)
            _reviewServiceMock = new Mock<IReviewService>();
            _recipeServiceMock = new Mock<IRecipeService>();
            _categoryServiceMock = new Mock<ICategoryService>();
            _ingredientTagServiceMock = new Mock<IIngredientTagService>();
            _typeOfDishServiceMock = new Mock<ITypeOfDishService>();
            _complaintImageServicesMock = new Mock<IComplaintImageServices>();
            _complaintServicesMock = new Mock<IComplaintServices>();
            _recipeIngredientTagServiceMock = new Mock<IRecipeIngredientTagIngredientTagSerivce>();
            _messageImageServiceMock = new Mock<IMessageImageService>();
            _messageServiceMock = new Mock<IMessageService>();
            _chatHubContextMock = new Mock<IHubContext<ChatHub>>();
            _recipeReviewServiceMock = new Mock<IRecipeReviewService>();
            _favoriteRecipeServiceMock = new Mock<IFavoriteRecipeService>();
            _storeFollowersServiceMock = new Mock<IStoreFollowersService>();
            _storeDetailServiceMock = new Mock<IStoreDetailService>();
            _voucherServiceMock = new Mock<IVoucherServices>();
            _followHubContextMock = new Mock<IHubContext<FollowHub>>();

            var context = new Mock<HttpContext>();
            _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(context.Object);

            _controller = new UsersController(
                _userManagerMock.Object,
                _httpClientMock.Object,
                _balanceChangeServiceMock.Object, // Removed null
                _httpContextAccessorMock.Object,
                _productServiceMock.Object,
                _cartServiceMock.Object,
                _productVariantServiceMock.Object,
                _productImageServiceMock.Object,
                _ordersServiceMock.Object,
                _orderDetailServiceMock.Object,
                _payOS,
                _manageTransaction, // Use real ManageTransaction instance
                _reviewServiceMock.Object,
                _recipeServiceMock.Object,
                _categoryServiceMock.Object,
                _ingredientTagServiceMock.Object,
                _typeOfDishServiceMock.Object,
                _complaintImageServicesMock.Object,
                _complaintServicesMock.Object,
                _recipeIngredientTagServiceMock.Object,
                _messageImageServiceMock.Object,
                _messageServiceMock.Object,
                _chatHubContextMock.Object,
                _recipeReviewServiceMock.Object,
                _favoriteRecipeServiceMock.Object,
                _storeFollowersServiceMock.Object,
                _storeDetailServiceMock.Object,
                _followHubContextMock.Object,
                _voucherServiceMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        private JObject GetJsonObject(JsonResult result)
        {
            // Chuyển Value -> JObject an toàn
            var json = System.Text.Json.JsonSerializer.Serialize(result.Value);
            return JObject.Parse(json);
        }
        [Test]
        public async Task Withdraw_Success_ReturnsSuccess()
        {
            var user = new AppUser { Id = "user1" };

            _userManagerMock
                .Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _balanceChangeServiceMock
                .Setup(x => x.CheckMoney(user.Id, 60000m))
                .ReturnsAsync(true);

            _balanceChangeServiceMock
                .Setup(x => x.GetBalance(user.Id))
                .ReturnsAsync(100000m);

            _balanceChangeServiceMock
                .Setup(x => x.AddAsync(It.IsAny<BalanceChange>()))
                .Returns(Task.CompletedTask);

            _balanceChangeServiceMock
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Mock hubContext
            var mockClientProxy = new Mock<IClientProxy>();
            mockClientProxy
                .Setup(x => x.SendCoreAsync(
                    It.IsAny<string>(),
                    It.IsAny<object[]>(),
                    default))
                .Returns(Task.CompletedTask);

            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);

            var mockHubContext = new Mock<IHubContext<ChatHub>>();
            mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);

            // Gán mocks vào controller
            typeof(UsersController)
                .GetField("_balance", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(_controller, _balanceChangeServiceMock.Object);

            typeof(UsersController)
                .GetField("_hubContext", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(_controller, mockHubContext.Object);

            // Mock API VietQR
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"data\":[{\"code\":\"VCB\",\"shortName\":\"VietinBank\"}]}")
                });

            var httpClient = new HttpClient(handlerMock.Object);
            typeof(UsersController)
                .GetField("client", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(_controller, httpClient);

            // Act
            var result = await _controller.Withdraw(60000, "VCB", "0123456789", "Tran Gia Huy") as JsonResult;
            var obj = GetJsonObject(result);

            // Assert
            Assert.IsTrue(obj["success"].Value<bool>());
            Assert.AreEqual("Success", obj["msg"].Value<string>());
        }




        [Test]
        public async Task Withdraw_AmountTooLow_ReturnsError()
        {
            var user = new AppUser { Id = "user1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var result = await _controller.Withdraw(40000, "VCB", "0123456789", "Tran Gia Huy") as JsonResult;
            var obj = GetJsonObject(result);

            Assert.IsFalse(obj["success"].Value<bool>());
            Assert.AreEqual("Minimum withdrawal is 10,000 ₫", obj["msg"].Value<string>());
        }

        [Test]
        public async Task Withdraw_AmountIsDecimal_ReturnsError()
        {
            var user = new AppUser { Id = "user1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            // Mock CheckMoney để không fail ở đoạn balance
            _balanceChangeServiceMock.Setup(x => x.CheckMoney(user.Id, It.IsAny<decimal>())).ReturnsAsync(true);

            var result = await _controller.Withdraw(500005, "VCB", "0123456789", "Tran Gia Huy") as JsonResult;
            var obj = GetJsonObject(result);

            Assert.IsFalse(obj["success"].Value<bool>());

            // Mong đợi message thực tế từ controller
            Assert.AreEqual("An error occurred, please try again or contact admin!", obj["msg"].Value<string>());
        }



        [Test]
        public async Task Withdraw_InsufficientBalance_ReturnsError()
        {
            var user = new AppUser { Id = "user1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);
            _balanceChangeServiceMock.Setup(x => x.CheckMoney(user.Id, 60000)).ReturnsAsync(false);

            var result = await _controller.Withdraw(60000, "VCB", "0123456789", "Tran Gia Huy") as JsonResult;
            var obj = GetJsonObject(result);

            Assert.IsFalse(obj["success"].Value<bool>());
            Assert.AreEqual("Insufficient balance!", obj["msg"].Value<string>());
        }

        [Test]
        public async Task Withdraw_BankApiFails_ReturnsError()
        {
            var user = new AppUser { Id = "user1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var httpClient = new HttpClient(httpMessageHandler.Object);
            typeof(UsersController)
                .GetField("client", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_controller, httpClient);

            var result = await _controller.Withdraw(60000, "VCB", "0123456789", "Tran Gia Huy") as JsonResult;
            var obj = GetJsonObject(result);

            Assert.IsFalse(obj["success"].Value<bool>());
            Assert.AreEqual("Unable to verify bank. Please try again later!", obj["msg"].Value<string>());
        }
    }
}
