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
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.DBContext;
using Moq;
using Net.payOS;
using Newtonsoft.Json.Linq;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Food_Haven.Web.Controllers.UsersController;

namespace Food_Haven.UnitTest.User_SendMessage_Test
{
    public class SendMessage_Test
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<IBalanceChangeService> _balanceMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IProductService> _productServiceMock;
        private Mock<ICartService> _cartServiceMock;
        private Mock<IProductVariantService> _productVariantServiceMock;
        private Mock<IProductImageService> _productImageServiceMock;
        private Mock<IOrdersServices> _ordersServiceMock;
        private Mock<IOrderDetailService> _orderDetailServiceMock;
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
        private Mock<IHubContext<FollowHub>> _followHubContextMock;
        private Mock<IRecipeReviewService> _recipeReviewServiceMock;
        private Mock<IFavoriteRecipeService> _favoriteRecipeServiceMock;
        private Mock<IStoreFollowersService> _storeFollowersServiceMock;
        private Mock<IStoreDetailService> _storeDetailServiceMock;
        private HttpClient _httpClient; // Không cần mock, có thể dùng real object
        private PayOS _payos;
        private ManageTransaction _manageTransaction;
        private Mock<IVoucherServices> _voucherServiceMock;

        // Controller instance
        private UsersController _controller;
        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(
                new Mock<IUserStore<AppUser>>().Object,
                null, null, null, null, null, null, null, null);

            _balanceMock = new Mock<IBalanceChangeService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _productServiceMock = new Mock<IProductService>();
            _cartServiceMock = new Mock<ICartService>();
            _productVariantServiceMock = new Mock<IProductVariantService>();
            _productImageServiceMock = new Mock<IProductImageService>();
            _ordersServiceMock = new Mock<IOrdersServices>();
            _orderDetailServiceMock = new Mock<IOrderDetailService>();
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
            _followHubContextMock = new Mock<IHubContext<FollowHub>>();
            _recipeReviewServiceMock = new Mock<IRecipeReviewService>();
            _favoriteRecipeServiceMock = new Mock<IFavoriteRecipeService>();
            _storeFollowersServiceMock = new Mock<IStoreFollowersService>();
            _storeDetailServiceMock = new Mock<IStoreDetailService>();
            _voucherServiceMock = new Mock<IVoucherServices>();
            _httpClient = new HttpClient();
            _payos = new PayOS("client-id", "api-key", "https://callback.url");
            var options = new DbContextOptionsBuilder<FoodHavenDbContext>()
     .UseInMemoryDatabase(databaseName: "TestDb")
     .Options;

            var dbContext = new FoodHavenDbContext(options);
            var manageTransactionMock = new Mock<ManageTransaction>(dbContext); // truyền instance
            manageTransactionMock
                .Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(async (func) =>
                {
                    await func();
                    return true;
                });



            _controller = new UsersController(
                _userManagerMock.Object,
                _httpClient,
                _balanceMock.Object,
                _httpContextAccessorMock.Object,
                _productServiceMock.Object,
                _cartServiceMock.Object,
                _productVariantServiceMock.Object,
                _productImageServiceMock.Object,
                _ordersServiceMock.Object,
                _orderDetailServiceMock.Object,
                _payos,
                _manageTransaction,
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
                _followHubContextMock.Object, _voucherServiceMock.Object
            );

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }
        [TearDown]
        public void TearDown()
        {
            _httpClient?.Dispose();
            _controller?.Dispose();
        }
        [Test]
        public async Task SendMessage_Should_SaveMessage_And_SendViaSignalR()
        {
            // Arrange
            var model = new ChatMessageModel
            {
                id = Guid.NewGuid().ToString(),
                from_id = "userA",
                to_id = "userB",
                msg = "Hello!",
                isReplied = Guid.Empty,
                has_images = new List<string> { "https://example.com/image1.jpg" }
            };

            var fromUser = new AppUser
            {
                Id = "userA",
                UserName = "Alice",
                ImageUrl = "https://example.com/avatar.jpg"
            };

            // Mock UserManager
            _userManagerMock.Setup(m => m.FindByIdAsync(model.from_id))
                            .ReturnsAsync(fromUser);

            // Mock MessageService
            _messageServiceMock.Setup(m => m.AddAsync(It.IsAny<Message>()))
                               .Returns(Task.CompletedTask);
            _messageServiceMock.Setup(m => m.SaveChangesAsync())
                               .ReturnsAsync(1);

            // Mock HubContext
            var hubClientsMock = new Mock<IHubClients>();
            var clientProxyMock = new Mock<IClientProxy>();

            hubClientsMock.Setup(c => c.User(It.IsAny<string>()))
                          .Returns(clientProxyMock.Object);

            _chatHubContextMock.Setup(c => c.Clients)
                               .Returns(hubClientsMock.Object);

            // Act
            var result = await _controller.SendMessage(model) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            var jsonData = result.Value;
            Assert.IsNotNull(jsonData);

            var jsonObj = JObject.FromObject(jsonData);
            Assert.AreEqual("userA", jsonObj["from_id"].ToString());
            Assert.AreEqual("userB", jsonObj["to_id"].ToString());
            Assert.AreEqual("Hello!", jsonObj["msg"].ToString());
            Assert.AreEqual("Alice", jsonObj["senderName"].ToString());
            Assert.AreEqual("https://example.com/avatar.jpg", jsonObj["senderAvatar"].ToString());

            // Verify lưu tin nhắn
            _messageServiceMock.Verify(m => m.AddAsync(It.Is<Message>(msg =>
                msg.FromUserId == model.from_id &&
                msg.ToUserId == model.to_id &&
                msg.MessageText == model.msg &&
                msg.IsRead == false &&
                msg.Images.Count == 1 &&
                msg.Images.First().ImageUrl == model.has_images.First()
            )), Times.Once);

            _messageServiceMock.Verify(m => m.SaveChangesAsync(), Times.Once);

            // Verify lấy thông tin người gửi
            _userManagerMock.Verify(m => m.FindByIdAsync(model.from_id), Times.Once);

            // Verify gửi qua SignalR
            clientProxyMock.Verify(cp => cp.SendCoreAsync("ReceiveMessage",
                It.Is<object[]>(o => o.Length == 1), default), Times.Once);

            clientProxyMock.Verify(cp => cp.SendCoreAsync("MessageSent",
                It.Is<object[]>(o => o.Length == 1), default), Times.Once);
        }

    }
}
