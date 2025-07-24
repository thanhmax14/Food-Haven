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
using Newtonsoft.Json.Linq;
using Repository.BalanceChange;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.User_StoreFollowers_Test
{
    public class StoreFollowers_Test
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

            private JObject ParseJsonResult(JsonResult result)
            {
                return JObject.FromObject(result.Value!);
            }

            [Test]
            public async Task TC01_StoreFollowers_UserNotLoggedIn_RedirectsToLogin()
            {
                _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                                .ReturnsAsync((AppUser)null);

                var model = new StoreFollowerViewModel
                {
                    StoreID = Guid.NewGuid()
                };

                var result = await _controller.StoreFollowers(model);

                Assert.IsInstanceOf<RedirectToActionResult>(result);
                var redirect = result as RedirectToActionResult;
                Assert.AreEqual("Login", redirect.ActionName);
                Assert.AreEqual("Home", redirect.ControllerName);
            }

            [Test]
            public async Task TC02_StoreFollowers_AlreadyFollowing_Unfollow_Success()
            {
                var user = new AppUser { Id = "user123" };
                var storeId = Guid.NewGuid();
                var existingFollow = new StoreFollower { StoreID = storeId, UserID = user.Id };

                _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                                .ReturnsAsync(user);
                _storeFollowersServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreFollower, bool>>>()))
                                          .ReturnsAsync(existingFollow);

                var model = new StoreFollowerViewModel { StoreID = storeId };
                var mockClients = new Mock<IHubClients>();
                var mockUserClient = new Mock<IClientProxy>();
                var mockHubContext = new Mock<IHubContext<FollowHub>>();
                _followHubContextMock.Setup(x => x.Clients).Returns(mockClients.Object);
                mockClients.Setup(x => x.User(It.IsAny<string>())).Returns(mockUserClient.Object);
                mockUserClient.Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                    .Returns(Task.CompletedTask);
                var serviceProviderMock = new Mock<IServiceProvider>();
                serviceProviderMock.Setup(x => x.GetService(typeof(IHubContext<FollowHub>)))
                    .Returns(mockHubContext.Object);
                var context = new DefaultHttpContext
                {
                    RequestServices = serviceProviderMock.Object
                };
                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = context
                };
                var result = await _controller.StoreFollowers(model) as JsonResult;

                var json = ParseJsonResult(result!);

                Assert.IsTrue(json["success"]!.Value<bool>());
                Assert.IsFalse(json["isFollowing"]!.Value<bool>());
                Assert.AreEqual("Unfollowed the store", json["message"]!.Value<string>());
            }

            [Test]
            public async Task TC03_StoreFollowers_NotFollowing_Follow_Success()
            {
                var user = new AppUser { Id = "user123" };
                var storeId = Guid.NewGuid();

                _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                                .ReturnsAsync(user);
                _storeFollowersServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreFollower, bool>>>()))
                                          .ReturnsAsync((StoreFollower)null);
                _storeFollowersServiceMock.Setup(x => x.AddAsync(It.IsAny<StoreFollower>()))
                                          .Returns(Task.CompletedTask);
                _storeFollowersServiceMock.Setup(x => x.SaveChangesAsync())
                                          .ReturnsAsync(1);
                var mockClients = new Mock<IHubClients>();
                var mockUserClient = new Mock<IClientProxy>();
                var mockHubContext = new Mock<IHubContext<FollowHub>>();
                _followHubContextMock.Setup(x => x.Clients).Returns(mockClients.Object);
                mockClients.Setup(x => x.User(It.IsAny<string>())).Returns(mockUserClient.Object);
                mockUserClient.Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                    .Returns(Task.CompletedTask);
                var serviceProviderMock = new Mock<IServiceProvider>();
                serviceProviderMock.Setup(x => x.GetService(typeof(IHubContext<FollowHub>)))
                    .Returns(mockHubContext.Object);
                var context = new DefaultHttpContext
                {
                    RequestServices = serviceProviderMock.Object
                };
                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = context
                };
                var model = new StoreFollowerViewModel { StoreID = storeId };

                var result = await _controller.StoreFollowers(model) as JsonResult;

                var json = ParseJsonResult(result!);
                Assert.IsTrue(json["success"]!.Value<bool>());
                Assert.IsTrue(json["isFollowing"]!.Value<bool>());
                Assert.AreEqual("Followed the store", json["message"]!.Value<string>());
            }


        }
    }

}