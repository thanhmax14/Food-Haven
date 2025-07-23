using System.Security.Claims;
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
using MockQueryable;
using Models;
using Moq;
using Net.payOS;
using Repository.BalanceChange;
using Repository.ViewModels;

namespace Food_Haven.UnitTest.Users_UpdateProfile_Test
{
    public class UpdateProfile_Test
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

        [Test]
        public async Task UpdateProfile_UserNotFound_ReturnsJsonError()
        {
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AppUser)null);

            var obj = new IndexUserViewModels { userView = new UsersViewModel() };
            var result = await _controller.UpdateProfile(obj);

            Assert.IsInstanceOf<JsonResult>(result);
            var json = (JsonResult)result;
            Assert.IsTrue(json.Value.ToString().Contains("User not found"));
        }

        [Test]
        public async Task UpdateProfile_InvalidPhoneNumber_ReturnsJsonError()
        {
            var user = new AppUser { Id = "1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var obj = new IndexUserViewModels { userView = new UsersViewModel { PhoneNumber = "123" } };
            var result = await _controller.UpdateProfile(obj);

            Assert.IsInstanceOf<JsonResult>(result);
            var json = (JsonResult)result;
            Assert.IsTrue(json.Value.ToString().Contains("Phone number must be exactly 10 digits"));
        }

        [Test]
        public async Task UpdateProfile_ExistingPhoneNumber_ReturnsJsonError()
        {
            var user = new AppUser { Id = "1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var obj = new IndexUserViewModels { userView = new UsersViewModel { PhoneNumber = "0123456789" } };

            var userStore = new List<AppUser> { new AppUser { Id = "2", PhoneNumber = "0123456789" } };
            var usersMock = userStore.AsQueryable().BuildMock(); // trả về IMock<IQueryable<AppUser>>

            _userManagerMock.Setup(x => x.Users).Returns(usersMock); // KHÔNG dùng .Object

            var result = await _controller.UpdateProfile(obj);

            Assert.IsInstanceOf<JsonResult>(result);
            var json = (JsonResult)result;
            Assert.IsTrue(json.Value.ToString().Contains("Phone number already exists"));
        }

        [Test]
        public async Task UpdateProfile_BirthdayGreaterThanToday_ReturnsJsonError()
        {
            var user = new AppUser { Id = "1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var obj = new IndexUserViewModels { userView = new UsersViewModel { Birthday = DateTime.Today.AddDays(1) } };
            var result = await _controller.UpdateProfile(obj);

            Assert.IsInstanceOf<JsonResult>(result);
            var json = (JsonResult)result;
            Assert.IsTrue(json.Value.ToString().Contains("Date of birth cannot be greater than the current date"));
        }

        [Test]
        public async Task UpdateProfile_Success_ReturnsJsonSuccess()
        {
            var user = new AppUser { Id = "1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var obj = new IndexUserViewModels
            {
                userView = new UsersViewModel
                {
                    PhoneNumber = "0123456789",
                    Birthday = DateTime.Today,
                    Email = "test@example.com",
                    Address = "Test Address",
                    FirstName = "Test",
                    LastName = "User"
                }
            };

            var userStore = new List<AppUser>().AsQueryable();
            var usersMock = userStore.BuildMock(); // Sử dụng BuildMock() để mock async

            _userManagerMock.Setup(x => x.Users).Returns(usersMock);

            var identityResult = IdentityResult.Success;
            _userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(identityResult);

            var result = await _controller.UpdateProfile(obj);

            Assert.IsInstanceOf<JsonResult>(result);
            var json = (JsonResult)result;
            Assert.IsTrue(json.Value.ToString().Contains("Profile updated successfully"));
        }

    }
}
