using AutoMapper;
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
using BusinessLogic.Services.StoreReports;
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
using NUnit.Framework;
using Repository.BalanceChange;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest
{
    [TestFixture]
    public class ViewMyFeedBack_Test
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
            // Test Case 1: Successful retrieval of user feedback
            [Test]
            public async Task ViewMyFeedBack_ValidUserWithFeedback_ReturnsJsonWithFeedback()
            {
                // Arrange
                var userId = "8e91c798-bc78-46a9-89a4-5d0aaea77f5f";
                var user = new AppUser { Id = userId, UserName = "testuser" };

                _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

                var productId = Guid.NewGuid();
                var reviews = new List<Review>
    {
        new Review
        {
            ID = Guid.NewGuid(),
            UserID = userId,
            ProductID = productId,
            Comment = "Great product!",
            CommentDate = DateTime.Now,
            Reply = "Thank you!",
            ReplyDate = DateTime.Now
        }
    };

                _reviewServiceMock.Setup(x => x.ListAsync()).ReturnsAsync(reviews);
                _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

                // ✅ Sửa thành mock đúng kiểu trả về Product
                _productServiceMock.Setup(x => x.GetAsyncById(productId))
                    .ReturnsAsync(new Product { Name = "TestProduct" });

                // Act
                var result = await _controller.ViewMyFeedBack() as JsonResult;

                // Assert
                Assert.IsNotNull(result);
                var userFeedbacks = result.Value as List<ReivewViewModel>; // Kiểm tra lại tên class thật của bạn
                Assert.IsNotNull(userFeedbacks);
                Assert.AreEqual(1, userFeedbacks.Count);
                Assert.AreEqual("Great product!", userFeedbacks[0].Comment);
                Assert.AreEqual("testuser", userFeedbacks[0].Username);
                Assert.AreEqual("TestProduct", userFeedbacks[0].ProductName);
            }


            [Test]
            public async Task ViewMyFeedBack_ValidUserNoFeedbacks_ReturnsEmptyJson()
            {
                // Arrange
                var user = new AppUser { Id = Guid.NewGuid().ToString(), UserName = "testuser" };
                _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                _reviewServiceMock.Setup(x => x.ListAsync()).ReturnsAsync(new List<Review>());
                _userManagerMock.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(user);
                _productServiceMock.Setup(x => x.GetAsyncById(It.IsAny<Guid>())).ReturnsAsync(new Product { Name = "TestRecipe" });

                // Act
                var result = await _controller.ViewMyFeedBack() as JsonResult;

                // Assert
                Assert.IsNotNull(result);
                var userFeedbacks = result.Value as List<ReivewViewModel>;
                Assert.IsNotNull(userFeedbacks);
                Assert.IsEmpty(userFeedbacks);
            }

            [Test]
            public async Task ViewMyFeedBack_ExceptionThrown_ReturnsStatus500()
            {
                // Arrange
                var user = new AppUser { Id = Guid.NewGuid().ToString(), UserName = "testuser" };
                _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                _reviewServiceMock.Setup(x => x.ListAsync()).ThrowsAsync(new Exception("Test Exception"));

                // Act
                var result = await _controller.ViewMyFeedBack() as ObjectResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(500, result.StatusCode);
                Assert.AreEqual("Lỗi khi lấy danh sách phản hồi.", result.Value);
            }


        }
    }

}