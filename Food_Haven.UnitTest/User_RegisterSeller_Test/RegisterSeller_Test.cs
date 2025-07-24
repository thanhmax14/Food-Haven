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
using Food_Haven.Web.Controllers;
using Food_Haven.Web.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models;
using Moq;
using Net.payOS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Repository.BalanceChange;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.User_RegisterSeller_Test
{
    [TestFixture]
    public class RegisterSeller_Test
    {
        private UsersController _controller;

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
        private PayOS _payOS;
        private ManageTransaction _manageTransaction;
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
            _payOS = new PayOS("client-id", "api-key", "https://callback.url");
            _manageTransaction = null;
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

            _controller = new UsersController(
                _userManagerMock.Object,
                _httpClientMock.Object,
                _balanceChangeServiceMock.Object,
                _httpContextAccessorMock.Object,
                _productServiceMock.Object,
                _cartServiceMock.Object,
                _productVariantServiceMock.Object,
                _productImageServiceMock.Object,
                _ordersServiceMock.Object,
                _orderDetailServiceMock.Object,
                _payOS,
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
                _followHubContextMock.Object
            );
        }
        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }
        [Test]
        public async Task RegisterSeller_ValidUserNotSeller_ReturnsSuccess()
        {
            var user = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testuser",
                Address = "123 Test St",
                PhoneNumber = "1234567890",
                Birthday = DateTime.Now.AddYears(-20),
                FirstName = "John",
                LastName = "Doe",
                RequestSeller = "0"
            };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            var model = new IndexUserViewModels();
            var result = await _controller.RegisterSeller(model) as JsonResult;

            Assert.IsNotNull(result);
            var json = JsonConvert.SerializeObject(result.Value);
            dynamic response = JsonConvert.DeserializeObject<dynamic>(json);
            Assert.IsTrue((bool)response.success);
            Assert.AreEqual("Register Seller successfully", (string)response.message);
        }

        [Test]
        public async Task RegisterSeller_ValidUserAlreadySeller_ReturnsError()
        {
            // Arrange
            var user = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testuser",
                Address = "123 Test St",
                PhoneNumber = "1234567890",
                Birthday = DateTime.Now.AddYears(-20),
                FirstName = "John",
                LastName = "Doe",
                RequestSeller = "1" // đã là người bán
            };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var model = new IndexUserViewModels();

            // Act
            var result = await _controller.RegisterSeller(model) as JsonResult;

            // Assert
            Assert.IsNotNull(result);

            // Dùng JObject để truy xuất các trường trả về
            var json = JsonConvert.SerializeObject(result.Value);
            var jObject = JObject.Parse(json);

            Assert.IsFalse((bool)jObject["success"]);
            Assert.AreEqual("You are already a seller.", (string)jObject["message"]);
        }




        [Test]
        public async Task RegisterSeller_UserNotAuthenticated_ReturnsRedirectToLogin()
        {
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AppUser)null);
            var model = new IndexUserViewModels();

            var result = await _controller.RegisterSeller(model) as JsonResult;

            Assert.IsNotNull(result);
            var json = JsonConvert.SerializeObject(result.Value);
            dynamic response = JsonConvert.DeserializeObject<dynamic>(json);
            Assert.IsFalse((bool)response.success);
        }

        [Test]
        public async Task RegisterSeller_UserWithIncompleteData_ReturnsError()
        {
            var user = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testuser",
                Address = null,
                PhoneNumber = "1234567890",
                Birthday = DateTime.Now.AddYears(-20),
                FirstName = "John",
                LastName = "Doe",
                RequestSeller = "0"
            };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            var model = new IndexUserViewModels();

            var result = await _controller.RegisterSeller(model) as JsonResult;

            Assert.IsNotNull(result);
            var json = JsonConvert.SerializeObject(result.Value);
            dynamic response = JsonConvert.DeserializeObject<dynamic>(json);
            Assert.IsFalse((bool)response.success);
            Assert.AreEqual("Please complete all required information before registering as a seller.", (string)response.message);
        }
    }
}
