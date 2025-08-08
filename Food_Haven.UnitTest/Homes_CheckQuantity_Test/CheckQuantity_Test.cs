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
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Homes_CheckQuantity_Test
{
    public class CheckQuantity_Test
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
        // Nếu muốn mock luôn RecipeSearchService, bạn cần tạo interface cho nó
        // private Mock<IRecipeSearchService> _recipeSearchServiceMock;

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
            // Nếu RecipeSearchService cần được mock, bạn nên refactor thành interface IRecipeSearchService và mock nó
            var recipeSearchService = new RecipeSearchService(""); // Hoặc dùng Mock<IRecipeSearchService>()

            var payOS = new PayOS("client-id", "api-key", "https://callback.url");
            var hubContextMock = new Mock<IHubContext<ChatHub>>(); // Add this line

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
                 _expertRecipeServicesMock.Object, // <-- Add this argument
 _recipeViewHistoryServicesMock.Object,
 hubContextMock.Object// <-- Add this argument
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
        public async Task CheckQuantity_NormalUpdate_ReturnsSuccess()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            var productTypeId = Guid.Parse("C932ABBB-60B1-40EC-9EFF-290C9739E34C");
            var productVarian = new ProductTypes { ID = productTypeId, ProductID = Guid.NewGuid(), Stock = 10 };
            var product = new Product { ID = productVarian.ProductID };
            var cartItem = new Cart { ProductTypesID = productTypeId, UserID = user.Id, Quantity = 5 };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            _productVariantServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductTypes, bool>>>())).ReturnsAsync(productVarian);
            _productServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>())).ReturnsAsync(product);
            _cartServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Cart, bool>>>())).ReturnsAsync(cartItem);

            var obj = new CartViewModels { ProductTypeID = productTypeId, quantity = 7 };

            // Act
            var result = await _controller.CheckQuantity(obj) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var data = result.Value;
            var json = System.Text.Json.JsonSerializer.Serialize(data);
using var doc = System.Text.Json.JsonDocument.Parse(json);
var root = doc.RootElement;
Assert.IsTrue(root.GetProperty("success").GetBoolean());
Assert.AreEqual("Cập nhật số lượng thành công.", root.GetProperty("message").GetString());
Assert.AreEqual(7, root.GetProperty("quantity").GetInt32());
Assert.AreEqual(productTypeId, root.GetProperty("productId").GetGuid());
        }

        [Test]
        public async Task CheckQuantity_ExceedsStock_ReturnsBadRequest()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            var productTypeId = Guid.Parse("C932ABBB-60B1-40EC-9EFF-290C9739E34C");
            var productVarian = new ProductTypes { ID = productTypeId, ProductID = Guid.NewGuid(), Stock = 5 };
            var product = new Product { ID = productVarian.ProductID };
            var cartItem = new Cart { ProductTypesID = productTypeId, UserID = user.Id, Quantity = 5 };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            _productVariantServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductTypes, bool>>>())).ReturnsAsync(productVarian);
            _productServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>())).ReturnsAsync(product);
            _cartServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Cart, bool>>>())).ReturnsAsync(cartItem);

            // Mock IHubContext<CartHub> and its Clients property to avoid NullReferenceException
            var hubContextMock = new Mock<IHubContext<CartHub>>();
            var clientsMock = new Mock<IHubClients>();
            var allClientMock = new Mock<IClientProxy>();
            clientsMock.Setup(c => c.All).Returns(allClientMock.Object);
            hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IHubContext<CartHub>)))
                .Returns(hubContextMock.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = serviceProviderMock.Object;
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            var obj = new CartViewModels { ProductTypeID = productTypeId, quantity = 7 };

            // Act
            var result = await _controller.CheckQuantity(obj) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var data = result.Value;
            var json = System.Text.Json.JsonSerializer.Serialize(data);
    using var doc = System.Text.Json.JsonDocument.Parse(json);
    var root = doc.RootElement;
    Assert.IsFalse(root.GetProperty("success").GetBoolean());
    Assert.That(root.GetProperty("message").GetString(), Is.EqualTo("Số lượng vượt quá tồn kho. Chỉ còn 5 sản phẩm."));
    Assert.IsTrue(root.GetProperty("isMaxStock").GetBoolean());
}

        [Test]
        public async Task CheckQuantity_ProductNotFound_ReturnsNotFound()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            var productTypeId = Guid.Parse("C932ABBB-60B1-40EC-9EFF-290C9739E34C");
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            _productVariantServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductTypes, bool>>>())).ReturnsAsync((ProductTypes?)null);
            var obj = new CartViewModels { ProductTypeID = productTypeId, quantity = 5 };

            // Act
            var result = await _controller.CheckQuantity(obj) as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var data = result.Value;
            var json = System.Text.Json.JsonSerializer.Serialize(data);
    using var doc = System.Text.Json.JsonDocument.Parse(json);
    var root = doc.RootElement;
    Assert.AreEqual("Loại sản phẩm không tồn tại.", root.GetProperty("message").GetString());
        }

        [Test]
        public async Task CheckQuantity_Exception_ReturnsException()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            var productTypeId = Guid.Parse("C932ABBB-60B1-40EC-9EFF-290C9739E34C");
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ThrowsAsync(new Exception("An unknown error occurred"));

            var obj = new CartViewModels { ProductTypeID = productTypeId, quantity = 5 };

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _controller.CheckQuantity(obj));
        }
    }
}
