using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.Categorys;
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
using Models;
using Models.DBContext;
using Moq;
using Net.payOS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.User_GetUserList_Test
{
    public class GetUserList_Test
    {
        public class Buy_Test
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
            [TearDown]

            [Test]
            public async Task TC01_GetUserList_ShouldReturnListOfUsersWhoTexted()
            {
                // Arrange
                var currentUser = new AppUser
                {
                    Id = "user1",
                    UserName = "user1",
                    ImageUrl = "avatar.jpg",
                    LastAccess = DateTime.Now
                };

                var adminId = "af32f202-1cb6-4191-8293-00da0aae4d2d";

                var messages = new List<Message>
    {
        new Message { FromUserId = "user1", ToUserId = "user2", IsRead = false },
        new Message { FromUserId = "user2", ToUserId = "user1", IsRead = false },
        new Message { FromUserId = "user1", ToUserId = adminId, IsRead = true }
    };

                var userList = new List<AppUser>
    {
        new AppUser
        {
            Id = "user2",
            UserName = "user2",
            ImageUrl = "img2.jpg",
            LastAccess = DateTime.Now.AddMinutes(-5)
        },
        new AppUser
        {
            Id = adminId,
            UserName = "admin",
            ImageUrl = "admin.jpg",
            LastAccess = DateTime.Now.AddMinutes(-1)
        }
    }.AsQueryable();

                _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                    .ReturnsAsync(currentUser);

                _messageServiceMock.Setup(m => m.GetAll()).Returns(messages.AsQueryable());

                _userManagerMock.Setup(m => m.Users).Returns(userList);

                // Act
                var result = await _controller.GetUserList() as JsonResult;

                // Assert
                Assert.IsNotNull(result);
                var data = result.Value as IEnumerable<dynamic>;
                Assert.IsNotNull(data);
                Assert.That(data.Count(), Is.EqualTo(2));
            }
            [Test]
            public async Task TC02_GetUserList_ShouldReturn500_WhenExceptionOccurs()
            {
                // Arrange
                _userManagerMock
                    .Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                    .ThrowsAsync(new Exception("Database failure"));

                // Act
                var result = await _controller.GetUserList();

                // Assert
                var objectResult = result as ObjectResult;
                Assert.IsNotNull(objectResult);
                Assert.AreEqual(500, objectResult.StatusCode);

                // Convert anonymous object to JObject to safely access dynamic properties
                var json = JsonConvert.SerializeObject(objectResult.Value);
                var response = JsonConvert.DeserializeObject<JObject>(json);

                Assert.That((string)response["message"], Does.Contain("An unknown error occurred"));
                Assert.That((string)response["error"], Is.EqualTo("Database failure"));
            }


        }
    }
}
