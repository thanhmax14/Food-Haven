using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Food_Haven.Web.Controllers;
using Food_Haven.Web.Hubs;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.RecipeServices;
using BusinessLogic.Services.TypeOfDishServices;
using BusinessLogic.Services.FavoriteFavoriteRecipes;
using Models;
using Repository.ViewModels;
using AutoMapper;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.StoreDetail;
using BusinessLogic.Services.StoreFollowers;
using BusinessLogic.Services.StoreReports;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.ComplaintImages;
using BusinessLogic.Services.Complaints;
using BusinessLogic.Services.IngredientTagServices;
using BusinessLogic.Services.Message;
using BusinessLogic.Services.MessageImages;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.RecipeIngredientTagIngredientTagServices;
using BusinessLogic.Services.RecipeReviewReviews;
using Net.payOS;
using Repository.BalanceChange;

namespace Food_Haven.UnitTest
{
    [TestFixture]
    public class ViewMyFeedBack_Test
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
                _followHubContextMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
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
