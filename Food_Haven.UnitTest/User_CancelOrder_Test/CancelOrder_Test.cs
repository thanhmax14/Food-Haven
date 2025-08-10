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
using Microsoft.EntityFrameworkCore;
using Models.DBContext;
using Moq;
using Net.payOS;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Food_Haven.UnitTest.User_CancelOrder_Test
{
    public class CancelOrder_Test
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<IBalanceChangeService> _balanceMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IProductService> _productServiceMock;
        private Mock<ICartService> _cartServiceMock;
        private Mock<IProductVariantService> _productVariantServiceMock;
        private Mock<IProductImageService> _productImageServiceMock;
        private Mock<IOrdersServices> _ordersServiceMock;
        private Mock<IOrderDetailService> _orderDetailServiceMock;
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
        private Mock<IHubContext<FollowHub>> _followHubContextMock;
        private Mock<IRecipeReviewService> _recipeReviewServiceMock;
        private Mock<IFavoriteRecipeService> _favoriteRecipeServiceMock;
        private Mock<IStoreFollowersService> _storeFollowersServiceMock;
        private Mock<IStoreDetailService> _storeDetailServiceMock;
        private HttpClient _httpClient; // Không cần mock, có thể dùng real object
        private PayOS _payos;
        private ManageTransaction _manageTransaction;
        private Mock<IVoucherServices> _voucherServiceMock;

        // Controller instance
        private UsersController _controller;
        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(
                new Mock<IUserStore<AppUser>>().Object,
                null, null, null, null, null, null, null, null);

            _balanceMock = new Mock<IBalanceChangeService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _productServiceMock = new Mock<IProductService>();
            _cartServiceMock = new Mock<ICartService>();
            _productVariantServiceMock = new Mock<IProductVariantService>();
            _productImageServiceMock = new Mock<IProductImageService>();
            _ordersServiceMock = new Mock<IOrdersServices>();
            _orderDetailServiceMock = new Mock<IOrderDetailService>();
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
            _followHubContextMock = new Mock<IHubContext<FollowHub>>();
            _recipeReviewServiceMock = new Mock<IRecipeReviewService>();
            _favoriteRecipeServiceMock = new Mock<IFavoriteRecipeService>();
            _storeFollowersServiceMock = new Mock<IStoreFollowersService>();
            _storeDetailServiceMock = new Mock<IStoreDetailService>();
            _voucherServiceMock = new Mock<IVoucherServices>();

            _httpClient = new HttpClient();
            _payos = new PayOS("client-id", "api-key", "https://callback.url");

            var options = new DbContextOptionsBuilder<FoodHavenDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var dbContext = new FoodHavenDbContext(options);

            var manageTransactionMock = new Mock<ManageTransaction>(dbContext);
            manageTransactionMock
                .Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(async (func) =>
                {
                    await func();
                    return true;
                });

            // Gán mock này cho _manageTransaction
            _manageTransaction = manageTransactionMock.Object;

            _controller = new UsersController(
                _userManagerMock.Object,
                _httpClient,
                _balanceMock.Object,
                _httpContextAccessorMock.Object,
                _productServiceMock.Object,
                _cartServiceMock.Object,
                _productVariantServiceMock.Object,
                _productImageServiceMock.Object,
                _ordersServiceMock.Object,
                _orderDetailServiceMock.Object,
                _payos,
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
                _followHubContextMock.Object,
                _voucherServiceMock.Object
            );

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient?.Dispose();
            _controller?.Dispose();
        }

        private object GetPropertyValue(object obj, string propertyName)
        {
            var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            return prop?.GetValue(obj);
        }

        [Test]
        public async Task CancelOrder_OrderNotFound_ReturnsError()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _ordersServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync((Order)null);

            // Act
            var result = await _controller.CancelOrder("D14DF64E-67A7-49C3-808A-0F5868D9747A") as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse((bool)GetPropertyValue(result.Value, "success"));
            Assert.AreEqual("Order not found.", GetPropertyValue(result.Value, "message"));
        }

        [Test]
        public async Task CancelOrder_OrderNotPending_ReturnsError()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            var order = new Order { ID = Guid.NewGuid(), Status = "Confirmed", UserID = user.Id, OrderTracking = "E2B85056-031A-4FC3-9F2E-125C9633B170" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _ordersServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(order);

            // Act
            var result = await _controller.CancelOrder(order.OrderTracking) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse((bool)GetPropertyValue(result.Value, "success"));
            Assert.AreEqual("Only pending orders can be cancelled.", GetPropertyValue(result.Value, "message"));
        }

        [Test]
        public async Task CancelOrder_NoProductsInOrder_ReturnsError()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            var order = new Order { ID = Guid.NewGuid(), Status = "Pending", UserID = user.Id, OrderTracking = "D6017BE2-C3A0-4F0C-B0CD-13E44FE41F2F" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _ordersServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(order);
            _orderDetailServiceMock
                .Setup(x => x.ListAsync(
                    It.IsAny<Expression<Func<OrderDetail, bool>>>(),
                    It.IsAny<Func<IQueryable<OrderDetail>, IOrderedQueryable<OrderDetail>>>(),
                    It.IsAny<Func<IQueryable<OrderDetail>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<OrderDetail, object>>>()
                ))
                .ReturnsAsync(new List<OrderDetail>());
            // Act
            var result = await _controller.CancelOrder(order.OrderTracking) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse((bool)GetPropertyValue(result.Value, "success"));
            Assert.AreEqual("There are no products in this order.", GetPropertyValue(result.Value, "message"));
        }

        [Test]
        public async Task CancelOrder_Success_ReturnsSuccess()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                ID = orderId,
                Status = "Pending",
                UserID = user.Id,
                OrderTracking = "TRACKING123",
                TotalPrice = 100000,
                VoucherID = null // Không test voucher
            };

            var orderDetails = new List<OrderDetail>
    {
        new OrderDetail
        {
            ID = Guid.NewGuid(),
            OrderID = orderId,
            ProductTypesID = Guid.NewGuid(),
            Quantity = 1
        }
    };

            var product = new ProductTypes
            {
                ID = orderDetails[0].ProductTypesID,
                Stock = 10,
                IsActive = true
            };

            _userManagerMock
                .Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            // FindAsync - truyền đủ tham số (tránh optional argument)
            _ordersServiceMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync((Expression<Func<Order, bool>> predicate) =>
                {
                    var compiled = predicate.Compile();
                    return compiled(order) ? order : null;
                });

            // ListAsync - truyền đủ 3 tham số
            _orderDetailServiceMock
                .Setup(x => x.ListAsync(
                    It.IsAny<Expression<Func<OrderDetail, bool>>>(),
                    null,
                    null))
                .ReturnsAsync(orderDetails);

            _orderDetailServiceMock
                .Setup(x => x.UpdateAsync(It.IsAny<OrderDetail>()))
                .Returns(Task.CompletedTask);

            _productVariantServiceMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>()))
                .ReturnsAsync(product);

            _productVariantServiceMock
                .Setup(x => x.UpdateAsync(It.IsAny<ProductTypes>()))
                .Returns(Task.CompletedTask);

            _balanceMock
                .Setup(x => x.GetBalance(user.Id))
                .ReturnsAsync(500000);

            _balanceMock
                .Setup(x => x.AddAsync(It.IsAny<BalanceChange>()))
                .Returns(Task.CompletedTask);

            // Mock SaveChangesAsync để không lỗi
            _orderDetailServiceMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _productVariantServiceMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _ordersServiceMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _balanceMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Mock SignalR Hub
            var clientProxyMock = new Mock<IClientProxy>();
            clientProxyMock
                .Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _chatHubContextMock
                .Setup(x => x.Clients.All)
                .Returns(clientProxyMock.Object);

            // Act
            var result = await _controller.CancelOrder(order.OrderTracking) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue((bool)GetPropertyValue(result.Value, "success"));
            Assert.AreEqual(
                "The order has been cancelled and refunded successfully.",
                GetPropertyValue(result.Value, "message")
            );
        }



        [Test]
        public async Task CancelOrder_ExceptionThrown_ReturnsError()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            var order = new Order { ID = Guid.NewGuid(), Status = "Pending", UserID = user.Id, OrderTracking = "EXCEPTION-CASE" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _ordersServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(order);
            _orderDetailServiceMock
                .Setup(x => x.ListAsync(
                    It.IsAny<Expression<Func<OrderDetail, bool>>>(),
                    It.IsAny<Func<IQueryable<OrderDetail>, IOrderedQueryable<OrderDetail>>>(),
                    It.IsAny<Func<IQueryable<OrderDetail>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<OrderDetail, object>>>()
                ))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _controller.CancelOrder(order.OrderTracking) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse((bool)GetPropertyValue(result.Value, "success"));
            Assert.AreEqual("An error occurred while processing your request. Please try again or contact admin.", GetPropertyValue(result.Value, "message"));
        }
    }
}
