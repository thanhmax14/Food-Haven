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
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;
using Models;
using Moq;
using Net.payOS;
using NUnit.Framework;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.User_MyViewRecipe_Test
{
    [TestFixture]
    public class MyViewRecipe_Test
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

        [Test]
        public async Task MyViewRecipe_ValidUserWithRecipes_ReturnsViewWithModel()
        {
            // TC1: User hợp lệ, có recipe
            var userId = "8e91c798-bc78-46a9-89a4-5d0aaea77f5f";
            var user = new AppUser { Id = userId };
            var typeOfDishId = Guid.NewGuid();      
            var recipes = new List<Recipe>
            {
                new Recipe { ID = Guid.NewGuid(), UserID = userId, TypeOfDishID = typeOfDishId }
            };
            var ingredientTags = new List<IngredientTag>();
            var typeOfDishes = new List<TypeOfDish> { new TypeOfDish { ID = typeOfDishId, Name = "Main" } };
            var recipeIngredientTags = new List<RecipeIngredientTag>();
            var categories = new List<Categories>();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _recipeServiceMock.Setup(s => s.ListAsync(null, null, null)).ReturnsAsync(recipes);
            _ingredientTagServiceMock.Setup(s => s.ListAsync(null, null, null)).ReturnsAsync(ingredientTags);
            _typeOfDishServiceMock.Setup(s => s.ListAsync(null, null, null)).ReturnsAsync(typeOfDishes);
            _recipeIngredientTagServiceMock
                .Setup(s => s.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RecipeIngredientTag, bool>>>(), null, It.IsAny<Func<IQueryable<RecipeIngredientTag>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<RecipeIngredientTag, object>>>()))
                .ReturnsAsync(recipeIngredientTags);
            _categoryServiceMock.Setup(s => s.ListAsync(null, null, null)).ReturnsAsync(categories);

            var result = await _controller.MyViewRecipe(1);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
        }

        [Test]
        public async Task MyViewRecipe_ValidUserWithNoRecipes_ReturnsViewWithEmptyList()
        {
            // TC2: User hợp lệ, không có recipe
            var userId = "00f56274-2b84-4965-bf88-bc522e2e17ed";
            var user = new AppUser { Id = userId };
            var recipes = new List<Recipe>();
            var ingredientTags = new List<IngredientTag>();
            var typeOfDishes = new List<TypeOfDish>();
            var categories = new List<Categories>();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _recipeServiceMock.Setup(s => s.ListAsync(null, null, null)).ReturnsAsync(recipes);
            _ingredientTagServiceMock.Setup(s => s.ListAsync(null, null, null)).ReturnsAsync(ingredientTags);
            _typeOfDishServiceMock.Setup(s => s.ListAsync(null, null, null)).ReturnsAsync(typeOfDishes);
            _categoryServiceMock.Setup(s => s.ListAsync(null, null, null)).ReturnsAsync(categories);

            var result = await _controller.MyViewRecipe(1);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            // Có thể kiểm tra thêm: danh sách recipes trong model là rỗng
        }



        [Test]
        public async Task MyViewRecipe_ServiceThrowsException_RedirectsToError()
        {
            var userId = "any-user";
            var user = new AppUser { Id = userId };

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _recipeServiceMock.Setup(s => s.ListAsync()).ThrowsAsync(new Exception("DB error"));

            // Setup TempData to avoid NullReferenceException
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var result = await _controller.MyViewRecipe(1);

            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Error", redirectResult.ActionName);
            Assert.AreEqual("Home", redirectResult.ControllerName);
        }


    }
}
