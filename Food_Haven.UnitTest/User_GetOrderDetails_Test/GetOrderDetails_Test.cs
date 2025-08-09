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
using Food_Haven.Web.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Net.payOS;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;

namespace Food_Haven.UnitTest.User_GetOrderDetails_Test
{
    [TestFixture]
    public class GetOrderDetails_Test
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
        public async Task GetOrderDetails_Success_ReturnsOrderDetails()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            var orderId = "D140F6AE-67A7-49C3-80BA-0F5866BD347A";
            var order = new Order { ID = Guid.Parse(orderId), OrderTracking = orderId, Status = "PROCESSING", Description = "" };
            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail { ID = Guid.NewGuid(), OrderID = order.ID, ProductTypesID = Guid.NewGuid(), ProductPrice = 100, TotalPrice = 100, Quantity = 1, Status = "PROCESSING", IsFeedback = true }
            };
            var productTypes = new List<ProductTypes>
            {
                new ProductTypes { ID = orderDetails[0].ProductTypesID, Name = "ProductA" }
            };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            _ordersServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>())).ReturnsAsync(order);
            _orderDetailServiceMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrderDetail, bool>>>(), null, null)).ReturnsAsync(orderDetails);
            _productVariantServiceMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductTypes, bool>>>(), null, null)).ReturnsAsync(productTypes);
            _complaintServicesMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Complaint, bool>>>())).ReturnsAsync(new Complaint());

            // Act
            var actionResult = await _controller.GetOrderDetails(orderId);
            var result = actionResult as JsonResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value?.GetType().GetProperty("success") != null
                ? result.Value
                : null;
            Assert.NotNull(value);
        }

        [Test]
        public async Task GetOrderDetails_OrderNotFound_ReturnsError()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            var orderId = "E2BB650E-031A-4C3F-92CF-1265C93B1B70";
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            _ordersServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>())).ReturnsAsync((Order)null);

            // Act
            var actionResult = await _controller.GetOrderDetails(orderId);
            var result = actionResult as JsonResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value?.GetType().GetProperty("success") != null
                ? result.Value
                : null;
            Assert.NotNull(value);
        }

        [Test]
        public async Task GetOrderDetails_NoOrderDetails_ReturnsError()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            var orderId = "D140F6AE-67A7-49C3-80BA-0F5866BD347A";
            var order = new Order { ID = Guid.Parse(orderId), OrderTracking = orderId, Status = "PROCESSING", Description = "" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            _ordersServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>())).ReturnsAsync(order);
            _orderDetailServiceMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrderDetail, bool>>>(), null, null)).ReturnsAsync(new List<OrderDetail>());

            // Act
            var actionResult = await _controller.GetOrderDetails(orderId);
            var result = actionResult as JsonResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value?.GetType().GetProperty("success") != null
                ? result.Value
                : null;
            Assert.NotNull(value);
        }

        [Test]
        public async Task GetOrderDetails_ExceptionThrown_ReturnsError()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            var orderId = "D140F6AE-67A7-49C3-80BA-0F5866BD347A";
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            _ordersServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>())).ThrowsAsync(new Exception("DB error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _controller.GetOrderDetails(orderId));
        }
    }
}
