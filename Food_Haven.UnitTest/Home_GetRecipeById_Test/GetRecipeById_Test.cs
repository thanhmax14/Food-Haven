using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.ExpertRecipes;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.RecipeServices;
using BusinessLogic.Services.RecipeViewHistorys;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using BusinessLogic.Services.StoreFollowers;
using BusinessLogic.Services.StoreReports;
using BusinessLogic.Services.VoucherServices;
using BusinessLogic.Services.Wishlists;
using Food_Haven.Web.Controllers;
using Food_Haven.Web.Hubs;
using Food_Haven.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models;
using Moq;
using Net.payOS;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Home_GetRecipeById_Test
{
    public class GetRecipeById_Test
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<SignInManager<AppUser>> _signInManagerMock;
        private Mock<ICategoryService> _categoryServiceMock;
        private Mock<IEmailSender> _emailSenderMock;
        private Mock<IOrderDetailService> _orderDetailServiceMock;
        private Mock<IRecipeService> _recipeServiceMock;
        private Mock<IStoreDetailService> _storeDetailServiceMock;
        private Mock<ICartService> _cartServiceMock;
        private Mock<IWishlistServices> _wishlistServiceMock;
        private Mock<IProductService> _productServiceMock;
        private Mock<IProductImageService> _productImageServiceMock;
        private Mock<IProductVariantService> _productVariantServiceMock;
        private Mock<IReviewService> _reviewServiceMock;
        private Mock<IBalanceChangeService> _balanceChangeServiceMock;
        private Mock<IOrdersServices> _ordersServiceMock;
        private Mock<IVoucherServices> _voucherServiceMock;
        private Mock<IStoreReportServices> _storeReportServiceMock;
        private Mock<IStoreFollowersService> _storeFollowersServiceMock;
        private Mock<IExpertRecipeServices> _expertRecipeServicesMock;
        private Mock<IRecipeViewHistoryServices> _recipeViewHistoryServicesMock;
        private Mock<IHubContext<ChatHub>> _hubContextMock;

        private HomeController _controller;

        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(
                new Mock<IUserStore<AppUser>>().Object,
                null, null, null, null, null, null, null, null);

            _signInManagerMock = new Mock<SignInManager<AppUser>>(
                _userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
                null, null, null, null);

            _categoryServiceMock = new Mock<ICategoryService>();
            _emailSenderMock = new Mock<IEmailSender>();
            _orderDetailServiceMock = new Mock<IOrderDetailService>();
            _recipeServiceMock = new Mock<IRecipeService>();
            _storeDetailServiceMock = new Mock<IStoreDetailService>();
            _cartServiceMock = new Mock<ICartService>();
            _wishlistServiceMock = new Mock<IWishlistServices>();
            _productServiceMock = new Mock<IProductService>();
            _productImageServiceMock = new Mock<IProductImageService>();
            _productVariantServiceMock = new Mock<IProductVariantService>();
            _reviewServiceMock = new Mock<IReviewService>();
            _balanceChangeServiceMock = new Mock<IBalanceChangeService>();
            _ordersServiceMock = new Mock<IOrdersServices>();
            _voucherServiceMock = new Mock<IVoucherServices>();
            _storeReportServiceMock = new Mock<IStoreReportServices>();
            _storeFollowersServiceMock = new Mock<IStoreFollowersService>();
            _expertRecipeServicesMock = new Mock<IExpertRecipeServices>();
            _recipeViewHistoryServicesMock = new Mock<IRecipeViewHistoryServices>();
            _hubContextMock = new Mock<IHubContext<ChatHub>>();

            var payOS = new PayOS("client-id", "api-key", "https://callback.url");
            var recipeSearchService = new RecipeSearchService("");

            _controller = new HomeController(
                _signInManagerMock.Object,
                _orderDetailServiceMock.Object,
                _recipeServiceMock.Object,
                _userManagerMock.Object,
                _categoryServiceMock.Object,
                _storeDetailServiceMock.Object,
                _emailSenderMock.Object,
                _cartServiceMock.Object,
                _wishlistServiceMock.Object,
                _productServiceMock.Object,
                _productImageServiceMock.Object,
                _productVariantServiceMock.Object,
                _reviewServiceMock.Object,
                _balanceChangeServiceMock.Object,
                _ordersServiceMock.Object,
                payOS,
                _voucherServiceMock.Object,
                _storeReportServiceMock.Object,
                _storeFollowersServiceMock.Object,
                recipeSearchService,
                _expertRecipeServicesMock.Object,
                _recipeViewHistoryServicesMock.Object,
                _hubContextMock.Object
            );

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        [Test]
        public async Task GetRecipeById_ValidId_ReturnsJson()
        {
            // Arrange
            var id = Guid.NewGuid();
            var recipe = new ExpertRecipe
            {
                ID = id,
                Title = "Pizza",
                Ingredients = "Dough, Cheese, Tomato",
                Directions = "Bake it"
            };

            _expertRecipeServicesMock
                .Setup(x => x.GetAsyncById(id))
                .ReturnsAsync(recipe);

            // Act
            var result = await _controller.GetRecipeById(id);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            // Extract anonymous object properties using reflection
            var value = jsonResult.Value;
            Assert.IsNotNull(value);

            var type = value.GetType();

            var idProp = type.GetProperty("ID")?.GetValue(value);
            var titleProp = type.GetProperty("Title")?.GetValue(value);
            var ingredientsProp = type.GetProperty("Ingredients")?.GetValue(value);
            var directionsProp = type.GetProperty("Directions")?.GetValue(value);

            Assert.AreEqual(recipe.ID, idProp);
            Assert.AreEqual(recipe.Title, titleProp);
            Assert.AreEqual(recipe.Ingredients, ingredientsProp);
            Assert.AreEqual(recipe.Directions, directionsProp);
        }


        [Test]
        public async Task GetRecipeById_NotFound_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _expertRecipeServicesMock
                .Setup(x => x.GetAsyncById(id))
                .ReturnsAsync((ExpertRecipe)null);

            // Act
            var result = await _controller.GetRecipeById(id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task GetRecipeById_ThrowsException_Returns500()
        {
            // Arrange
            var id = Guid.NewGuid();

            _expertRecipeServicesMock
                .Setup(x => x.GetAsyncById(id))
                .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.GetRecipeById(id);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while retrieving the recipe.", objectResult.Value);
        }
    }
}
