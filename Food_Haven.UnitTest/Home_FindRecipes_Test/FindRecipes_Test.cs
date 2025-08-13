using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
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
using Food_Haven.Web.Hubs;
using Food_Haven.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Net.payOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Home_FindRecipes_Test
{
    public class FindRecipes_Test
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<SignInManager<AppUser>> _signInManagerMock;
        private HomeController _controller;
        private Mock<ICategoryService> _categoryServiceMock;
        private Mock<IExpertRecipeServices> _expertRecipeServicesMock;
        private Mock<IRecipeViewHistoryServices> _recipeViewHistoryServicesMock;

        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(
                new Mock<IUserStore<AppUser>>().Object,
                null, null, null, null, null, null, null, null
            );

            _signInManagerMock = new Mock<SignInManager<AppUser>>(
                _userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
                null, null, null, null
            );

            var payOS = new PayOS("client-id", "api-key", "https://callback.url");
            var orderDetailMock = new Mock<IOrderDetailService>();
            var recipeServiceMock = new Mock<IRecipeService>();
            _categoryServiceMock = new Mock<ICategoryService>();
            var storeDetailServiceMock = new Mock<IStoreDetailService>();
            var emailSenderMock = new Mock<IEmailSender>();
            var cartMock = new Mock<ICartService>();
            var wishlistMock = new Mock<IWishlistServices>();
            var productServiceMock = new Mock<IProductService>();
            var productImgServiceMock = new Mock<IProductImageService>();
            var productVariantMock = new Mock<IProductVariantService>();
            var reviewServiceMock = new Mock<IReviewService>();
            var balanceChangeMock = new Mock<IBalanceChangeService>();
            var ordersServiceMock = new Mock<IOrdersServices>();
            var voucherMock = new Mock<IVoucherServices>();
            var storeReportMock = new Mock<IStoreReportServices>();
            var storeFollowersMock = new Mock<IStoreFollowersService>();
            var recipeSearchMock = new RecipeSearchService("");
            var hubContextMock = new Mock<IHubContext<ChatHub>>();

            // ✅ Khởi tạo các mock bị thiếu
            _expertRecipeServicesMock = new Mock<IExpertRecipeServices>();
            _recipeViewHistoryServicesMock = new Mock<IRecipeViewHistoryServices>();

            _controller = new HomeController(
                _signInManagerMock.Object,
                orderDetailMock.Object,
                recipeServiceMock.Object,
                _userManagerMock.Object,
                _categoryServiceMock.Object,
                storeDetailServiceMock.Object,
                emailSenderMock.Object,
                cartMock.Object,
                wishlistMock.Object,
                productServiceMock.Object,
                productImgServiceMock.Object,
                productVariantMock.Object,
                reviewServiceMock.Object,
                balanceChangeMock.Object,
                ordersServiceMock.Object,
                payOS,
                voucherMock.Object,
                storeReportMock.Object,
                storeFollowersMock.Object,
                recipeSearchMock,
                _expertRecipeServicesMock.Object,        // đã khởi tạo
                _recipeViewHistoryServicesMock.Object,   // đã khởi tạo
                hubContextMock.Object
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
        public async Task FindRecipes_ShouldReturnBadRequest_WhenIngredientsNull()
        {
            // Act
            var result = await _controller.FindRecipes(null);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = result as BadRequestObjectResult;
            Assert.AreEqual("You must enter at least 1 recipe", badRequest.Value);
        }

        [Test]
        public async Task FindRecipes_ShouldReturnBadRequest_WhenIngredientsEmpty()
        {
            // Act
            var result = await _controller.FindRecipes(new List<string>());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = result as BadRequestObjectResult;
            Assert.AreEqual("You must enter at least 1 recipe", badRequest.Value);
        }

        [Test]
        public async Task FindRecipes_ShouldReturnEmptyResults_WhenNoMatch()
        {
            // Arrange
            var recipes = new List<ExpertRecipe>
            {
                new ExpertRecipe { NER = JsonSerializer.Serialize(new List<string>{ "salt", "pepper" }) },
                new ExpertRecipe { NER = JsonSerializer.Serialize(new List<string>{ "egg", "milk" }) }
            };
            _expertRecipeServicesMock.Setup(s => s.ListAsync()).ReturnsAsync(recipes);

            var ingredients = new List<string> { "flour" };

            // Act
            var result = await _controller.FindRecipes(ingredients) as PartialViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("_RecipeResults", result.ViewName);
            var model = result.Model as List<ExpertRecipe>;
            Assert.NotNull(model);
            Assert.AreEqual(0, model.Count);
        }

        [Test]
        public async Task FindRecipes_ShouldReturnMatchingResults_LimitedByLimit()
        {
            // Arrange
            var recipes = new List<ExpertRecipe>
            {
                new ExpertRecipe { NER = JsonSerializer.Serialize(new List<string>{ "flour", "sugar" }) },
                new ExpertRecipe { NER = JsonSerializer.Serialize(new List<string>{ "flour", "sugar", "milk" }) },
                new ExpertRecipe { NER = JsonSerializer.Serialize(new List<string>{ "flour", "sugar", "butter" }) }
            };
            _expertRecipeServicesMock.Setup(s => s.ListAsync()).ReturnsAsync(recipes);

            var ingredients = new List<string> { "flour", "sugar" };

            // Act
            var result = await _controller.FindRecipes(ingredients, limit: 2) as PartialViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("_RecipeResults", result.ViewName);
            var model = result.Model as List<ExpertRecipe>;
            Assert.NotNull(model);
            Assert.AreEqual(2, model.Count);
        }
    }
}

