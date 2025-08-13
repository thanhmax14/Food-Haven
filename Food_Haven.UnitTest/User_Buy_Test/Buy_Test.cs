using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
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
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.User_Buy_Test
{
    [TestFixture]
    public class Buy_Test
    {
        private UsersController _controller;
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<IProductVariantService> _productVariantMock;
        private Mock<IProductService> _productMock;
        private Mock<ICartService> _cartMock;
        private Mock<IProductImageService> _imgMock;
        private Mock<IVoucherServices> _voucherMock;

        private AppUser _user;

        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(
                new Mock<IUserStore<AppUser>>().Object,
                null, null, null, null, null, null, null, null);

            _productVariantMock = new Mock<IProductVariantService>();
            _productMock = new Mock<IProductService>();
            _cartMock = new Mock<ICartService>();
            _imgMock = new Mock<IProductImageService>();
            _voucherMock = new Mock<IVoucherServices>();

            // Fake user
            _user = new AppUser
            {
                Id = "user1",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "0123456789",
                Address = "123 Street",
                IsProfileUpdated = true
            };

            _controller = new UsersController(
                _userManagerMock.Object,
                new HttpClient(),
                Mock.Of<IBalanceChangeService>(),
                Mock.Of<IHttpContextAccessor>(),
                _productMock.Object,
                _cartMock.Object,
                _productVariantMock.Object,
                _imgMock.Object,
                Mock.Of<IOrdersServices>(),
                Mock.Of<IOrderDetailService>(),
                new Net.payOS.PayOS("id", "key", "callback"),
                null,
                Mock.Of<IReviewService>(),
                Mock.Of<IRecipeService>(),
                Mock.Of<ICategoryService>(),
                Mock.Of<IIngredientTagService>(),
                Mock.Of<ITypeOfDishService>(),
                Mock.Of<IComplaintImageServices>(),
                Mock.Of<IComplaintServices>(),
                Mock.Of<IRecipeIngredientTagIngredientTagSerivce>(),
                Mock.Of<IMessageImageService>(),
                Mock.Of<IMessageService>(),
                Mock.Of<IHubContext<ChatHub>>(),
                Mock.Of<IRecipeReviewService>(),
                Mock.Of<IFavoriteRecipeService>(),
                Mock.Of<IStoreFollowersService>(),
                Mock.Of<IStoreDetailService>(),
                Mock.Of<IHubContext<FollowHub>>(),
                _voucherMock.Object
            );

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Enable session
            var session = new TestSession();
            _controller.ControllerContext.HttpContext.Session = session;
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }
        [Test]
        public async Task Buy_ShouldReturnError_WhenUserNotLoggedIn()
        {
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AppUser)null);

            var result = await _controller.Buy(new List<Guid> { Guid.NewGuid() }, null) as JsonResult;
            var data = result.Value as ErroMess;

            Assert.AreEqual("You are not logged in!", data.msg);
        }

        [Test]
        public async Task Buy_ShouldReturnError_WhenNoProductSelected()
        {
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(_user);

            var result = await _controller.Buy(null, null) as JsonResult;
            var data = result.Value as ErroMess;

            Assert.AreEqual("Please select the products you want to buy!", data.msg);
        }

        [Test]
        public async Task Buy_ShouldReturnError_WhenProfileNotUpdated()
        {
            _user.IsProfileUpdated = false;
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(_user);

            var result = await _controller.Buy(new List<Guid> { Guid.NewGuid() }, null) as JsonResult;
            var data = result.Value as ErroMess;

            Assert.AreEqual("You must update your personal information before making a purchase!", data.msg);
        }

        [Test]
        public async Task Buy_ShouldReturnError_WhenProductNotInCart()
        {
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(_user);

            var productId = Guid.NewGuid();
            var variant = new ProductTypes { ID = productId, ProductID = Guid.NewGuid(), Name = "Test" };
            var product = new Product { ID = variant.ProductID, StoreID = Guid.NewGuid() };

            _productVariantMock.Setup(s => s.GetAsyncById(productId)).ReturnsAsync(variant);
            _productMock.Setup(s => s.FindAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(product);
            _cartMock.Setup(s => s.FindAsync(It.IsAny<Expression<Func<Cart, bool>>>())).ReturnsAsync((Cart)null);

            var result = await _controller.Buy(new List<Guid> { productId }, null) as JsonResult;
            var data = result.Value as ErroMess;

            Assert.AreEqual("The selected product does not exist in the cart!", data.msg);
        }

        [Test]
        public async Task Buy_ShouldReturnError_WhenStockNotEnough()
        {
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(_user);

            var productId = Guid.NewGuid();
            var variant = new ProductTypes { ID = productId, ProductID = Guid.NewGuid(), Name = "Test", Stock = 1, SellPrice = 10 };
            var product = new Product { ID = variant.ProductID, StoreID = Guid.NewGuid() };
            var cartItem = new Cart { ProductTypesID = productId, Quantity = 5, UserID = _user.Id };

            _productVariantMock.Setup(s => s.GetAsyncById(productId)).ReturnsAsync(variant);
            _productMock.Setup(s => s.FindAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(product);
            _cartMock.Setup(s => s.FindAsync(It.IsAny<Expression<Func<Cart, bool>>>())).ReturnsAsync(cartItem);
            _productVariantMock.Setup(s => s.FindAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>())).ReturnsAsync(variant);

            var result = await _controller.Buy(new List<Guid> { productId }, null) as JsonResult;
            var data = result.Value as ErroMess;

            Assert.AreEqual("The quantity you wish to buy exceeds the available stock!", data.msg);
        }

        [Test]
        public async Task Buy_ShouldReturnSuccess_WhenValidRequest()
        {
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(_user);

            var productId = Guid.NewGuid();
            var variant = new ProductTypes
            { ID = productId, ProductID = Guid.NewGuid(), Name = "Test", Stock = 10, SellPrice = 10 };
            var product = new Product { ID = variant.ProductID, StoreID = Guid.NewGuid() };
            var cartItem = new Cart { ProductTypesID = productId, Quantity = 1, UserID = _user.Id };
            var img = new ProductImage { ProductID = product.ID, IsMain = true, ImageUrl = "http://example.com/img.jpg" };

            _productVariantMock.Setup(s => s.GetAsyncById(productId)).ReturnsAsync(variant);
            _productMock.Setup(s => s.FindAsync(It.IsAny<Expression<Func<Product, bool>>>())).ReturnsAsync(product);
            _cartMock.Setup(s => s.FindAsync(It.IsAny<Expression<Func<Cart, bool>>>())).ReturnsAsync(cartItem);
            _productVariantMock.Setup(s => s.FindAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>())).ReturnsAsync(variant);
            _imgMock.Setup(s => s.FindAsync(It.IsAny<Expression<Func<ProductImage, bool>>>())).ReturnsAsync(img);

            var result = await _controller.Buy(new List<Guid> { productId }, null) as JsonResult;
            var json = JObject.FromObject(result.Value);
            Assert.IsTrue(json["success"].Value<bool>());
            Assert.AreEqual("/Users/CheckOut", json["redirectUrl"].Value<string>());

        }
    }

    // Fake ISession for unit tests
    public class TestSession : ISession
    {
        private Dictionary<string, byte[]> _sessionStorage = new Dictionary<string, byte[]>();
        public IEnumerable<string> Keys => _sessionStorage.Keys;
        public string Id => Guid.NewGuid().ToString();
        public bool IsAvailable => true;
        public void Clear() => _sessionStorage.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _sessionStorage.Remove(key);
        public void Set(string key, byte[] value) => _sessionStorage[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);
    }
}
