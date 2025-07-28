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
using Models;
using Moq;
using Net.payOS;
using Repository.BalanceChange;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Users_CreateRecipe_Test
{
    public class CreateRecipe_Test
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

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "user123") }))
                }
            };
        }
        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }
        // Helper: Minimal valid JPEG header (not a real image, but enough for ImageSharp to recognize)
        // This is a real, minimal 1x1 pixel JPEG image.
        // This is a real, minimal 1x1 pixel JPEG image.
        // This is a real, minimal 1x1 pixel JPEG image.
        private static readonly byte[] ValidJpegBytes = new byte[]
        {
            0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x01, 0x00, 0x00, 0x01,
            0x00, 0x01, 0x00, 0x00, 0xFF, 0xDB, 0x00, 0x43, 0x00, 0x03, 0x02, 0x02, 0x03, 0x02, 0x02, 0x03,
            0x03, 0x03, 0x03, 0x04, 0x03, 0x03, 0x04, 0x05, 0x08, 0x05, 0x05, 0x04, 0x04, 0x05, 0x0A, 0x07,
            0x07, 0x06, 0x08, 0x0C, 0x0A, 0x0C, 0x0C, 0x0B, 0x0A, 0x0B, 0x0B, 0x0D, 0x0E, 0x12, 0x10, 0x0D,
            0x0E, 0x11, 0x0E, 0x0B, 0x0B, 0x10, 0x16, 0x10, 0x11, 0x13, 0x14, 0x15, 0x15, 0x15, 0x0C, 0x0F,
            0x17, 0x18, 0x16, 0x14, 0x18, 0x12, 0x14, 0x15, 0x14, 0xFF, 0xC0, 0x00, 0x11, 0x08, 0x00, 0x01,
            0x00, 0x01, 0x03, 0x01, 0x11, 0x00, 0x02, 0x11, 0x01, 0x03, 0x11, 0x01, 0xFF, 0xC4, 0x00, 0x14,
            0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0xFF, 0xC4, 0x00, 0x14, 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xDA, 0x00, 0x0C, 0x03, 0x01, 0x00,
            0x02, 0x11, 0x03, 0x11, 0x00, 0x3F, 0x00, 0xD2, 0xCF, 0x20, 0xFF, 0xD9
        };

        [Test]
        public async Task CreateRecipe_ValidInput_ReturnsRedirectToMyViewRecipe()
        {
            // Arrange
            var user = new AppUser { Id = "user123" };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _categoryServiceMock.Setup(m => m.ListAsync()).ReturnsAsync(new List<Categories> { new Categories { ID = Guid.NewGuid() } });
            _typeOfDishServiceMock.Setup(m => m.ListAsync()).ReturnsAsync(new List<TypeOfDish> { new TypeOfDish { ID = Guid.NewGuid() } });
            _recipeServiceMock.Setup(m => m.AddAsync(It.IsAny<Recipe>())).Returns(Task.CompletedTask);
            _recipeServiceMock.Setup(m => m.SaveChangesAsync()).Returns(Task.FromResult(1));
            _recipeIngredientTagServiceMock.Setup(m => m.AddAsync(It.IsAny<RecipeIngredientTag>())).Returns(Task.CompletedTask);

            var recipeViewModel = new RecipeViewModels
            {
                Title = "Spaghetti Carbonara",
                ShortDescriptions = "A classic Italian pasta...",
                PreparationTime = "15",
                CookTime = "20",
                DifficultyLevel = "Medium",
                Servings = "4 people",
                CateID = Guid.NewGuid(),
                TypeOfDishID = Guid.NewGuid(),
                CookingStep = "1. Boil pasta.\n2. Fry pancetta.\n3. Mix eggs and cheese.\n4. Combine all.",
                Ingredient = "Chicken, Lemon, Salt, Pepper, Olive Oil",
                SelectedIngredientTags = new List<Guid> { Guid.NewGuid() },
                IsActive = true
            };

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(ValidJpegBytes.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(ValidJpegBytes));

            // Act
            var result = await _controller.CreateRecipe(recipeViewModel, fileMock.Object);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("MyViewRecipe", redirect.ActionName);
            Assert.AreEqual("Users", redirect.ControllerName);
        }
        [Test]
        public async Task CreateRecipe_UserNotLoggedIn_ReturnsRedirectToLogin()
        {
            // Arrange
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AppUser)null);

            var recipeViewModel = new RecipeViewModels();
            var fileMock = new Mock<IFormFile>();

            // Act
            var result = await _controller.CreateRecipe(recipeViewModel, fileMock.Object);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Login", redirect.ActionName);
            Assert.AreEqual("Home", redirect.ControllerName);
        }


    }
}
