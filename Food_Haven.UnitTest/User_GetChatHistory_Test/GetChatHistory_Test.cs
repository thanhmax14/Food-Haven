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

namespace Food_Haven.UnitTest.User_GetChatHistory_Test
{
    public class GetChatHistory_Test
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
        public async Task GetChatHistory_Should_MarkUnreadMessagesAsRead_AndReturnChats()
        {
            // Arrange
            var userId = "userA";
            var otherId = "userB";

            var messages = new List<Message>
    {
        // Tin chưa đọc — phải có ToUserId == userId và IsRead = false
        new Message
        {
            ID = Guid.NewGuid(),
            FromUserId = otherId,
            ToUserId = userId,
            MessageText = "Hello to A",
            SentAt = DateTime.Now.AddMinutes(-5),
            IsRead = false,
            Images = new List<MessageImage>()
        },
        // Tin đã đọc
        new Message
        {
            ID = Guid.NewGuid(),
            FromUserId = userId,
            ToUserId = otherId,
            MessageText = "Reply to B",
            SentAt = DateTime.Now.AddMinutes(-2),
            IsRead = true,
            Images = new List<MessageImage>()
        }
    }.AsQueryable();

            // Mock GetAll()
            _messageServiceMock.Setup(m => m.GetAll())
                .Returns(messages);

            // Mock UpdateAsync & SaveChangesAsync
            _messageServiceMock.Setup(m => m.UpdateAsync(It.IsAny<Message>()))
                .Returns(Task.CompletedTask);
            _messageServiceMock.Setup(m => m.SaveChangesAsync())
                .ReturnsAsync(1);

            // Mock HubContext (MessagesRead event)
            var hubClientsMock = new Mock<IHubClients>();
            var clientProxyMock = new Mock<IClientProxy>();
            hubClientsMock.Setup(clients => clients.User(It.IsAny<string>()))
                          .Returns(clientProxyMock.Object);
            _chatHubContextMock.Setup(c => c.Clients).Returns(hubClientsMock.Object);

            // Act
            var result = await _controller.GetChatHistory(userId, otherId) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            var data = result.Value;
            Assert.IsNotNull(data);

            var json = JObject.FromObject(data);
            var chats = json["chats"] as JArray;
            Assert.AreEqual(2, chats.Count);

            // Tin chưa đọc ban đầu giờ phải thành đã đọc
            _messageServiceMock.Verify(m => m.UpdateAsync(It.Is<Message>(msg => msg.IsRead)), Times.AtLeastOnce);
            _messageServiceMock.Verify(m => m.SaveChangesAsync(), Times.Once);

            // Đảm bảo đã gửi thông báo "MessagesRead"
            clientProxyMock.Verify(cp => cp.SendCoreAsync("MessagesRead",
                It.IsAny<object[]>(), default), Times.Once);
        }


    }
}
