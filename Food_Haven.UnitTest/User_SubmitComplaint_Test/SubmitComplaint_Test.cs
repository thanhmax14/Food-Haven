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
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Food_Haven.UnitTest.User_SubmitComplaint_Test
{
    public class SubmitComplaint_Test
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
            var manageTransactionMock = new Mock<ManageTransaction>(dbContext); // truyền instance
            manageTransactionMock
                .Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(async (func) =>
                {
                    await func();
                    return true;
                });



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
                _followHubContextMock.Object, _voucherServiceMock.Object
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
        // Helper to get property value from anonymous object
        private object GetPropertyValue(object obj, string propertyName)
        {
            var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            return prop?.GetValue(obj);
        }

        [Test]
        public async Task SubmitComplaint_ValidInput_ReturnsSuccess()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            var orderDetailId = Guid.NewGuid();
            var orderDetail = new OrderDetail { ID = orderDetailId, OrderID = Guid.NewGuid(), TotalPrice = 100 };
            var order = new Order { ID = orderDetail.OrderID, VoucherID = null, TotalPrice = 100 };
            var images = new List<IFormFile>
        {
            CreateMockFormFile("file1.jpg", 1024),
            CreateMockFormFile("file2.png", 1024)
        };
            var model = new ComplaintViewModel
            {
                OrderDetailID = orderDetailId,
                Description = "The product arrived damaged.",
                Images = images
            };
            _orderDetailServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>())).ReturnsAsync(orderDetail);
            _ordersServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(order);

            // Act
            var result = await _controller.SubmitComplaint(model) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue((bool)GetPropertyValue(result.Value, "success"));
            Assert.AreEqual("Complaint submitted successfully.", GetPropertyValue(result.Value, "message"));
        }

        [Test]
        public async Task SubmitComplaint_TooManyImages_ReturnsError()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            var orderDetailId = Guid.NewGuid();
            var orderDetail = new OrderDetail { ID = orderDetailId, OrderID = Guid.NewGuid(), TotalPrice = 100 };
            var order = new Order { ID = orderDetail.OrderID, VoucherID = null, TotalPrice = 100 };
            var images = new List<IFormFile>
        {
            CreateMockFormFile("file1.jpg", 1024),
            CreateMockFormFile("file2.png", 1024),
            CreateMockFormFile("file3.gif", 1024),
            CreateMockFormFile("file4.bmp", 1024)
        };
            var model = new ComplaintViewModel
            {
                OrderDetailID = orderDetailId,
                Description = "The product arrived damaged.",
                Images = images
            };
            _orderDetailServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>())).ReturnsAsync(orderDetail);
            _ordersServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(order);

            // Act
            var result = await _controller.SubmitComplaint(model) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse((bool)GetPropertyValue(result.Value, "success"));
            Assert.AreEqual("You can select up to 3 images only.", GetPropertyValue(result.Value, "message"));
        }

        [Test]
        public async Task SubmitComplaint_InvalidImageFormat_ReturnsError()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            var orderDetailId = Guid.NewGuid();
            var orderDetail = new OrderDetail { ID = orderDetailId, OrderID = Guid.NewGuid(), TotalPrice = 100 };
            var order = new Order { ID = orderDetail.OrderID, VoucherID = null, TotalPrice = 100 };
            var images = new List<IFormFile>
        {
            CreateMockFormFile("malicious.exe", 1024),
            CreateMockFormFile("image.bmp", 1024)
        };
            var model = new ComplaintViewModel
            {
                OrderDetailID = orderDetailId,
                Description = "The product arrived damaged.",
                Images = images
            };
            _orderDetailServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>())).ReturnsAsync(orderDetail);
            _ordersServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(order);

            // Act
            var result = await _controller.SubmitComplaint(model) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse((bool)GetPropertyValue(result.Value, "success"));
            Assert.AreEqual("Image malicious.exe has an invalid format.", GetPropertyValue(result.Value, "message"));
        }

        [Test]
        public async Task SubmitComplaint_ImageSizeExceeded_ReturnsError()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            var orderDetailId = Guid.NewGuid();
            var orderDetail = new OrderDetail { ID = orderDetailId, OrderID = Guid.NewGuid(), TotalPrice = 100 };
            var order = new Order { ID = orderDetail.OrderID, VoucherID = null, TotalPrice = 100 };
            var images = new List<IFormFile>
        {
            CreateMockFormFile("file1.jpg", 6 * 1024 * 1024) // 6MB
        };
            var model = new ComplaintViewModel
            {
                OrderDetailID = orderDetailId,
                Description = "The product arrived damaged.",
                Images = images
            };
            _orderDetailServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>())).ReturnsAsync(orderDetail);
            _ordersServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(order);

            // Act
            var result = await _controller.SubmitComplaint(model) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse((bool)GetPropertyValue(result.Value, "success"));
            Assert.AreEqual("Image file1.jpg exceeds the 5MB size limit.", GetPropertyValue(result.Value, "message"));
        }

        [Test]
        public async Task SubmitComplaint_OrderDetailNotFound_ReturnsError()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            var orderDetailId = Guid.NewGuid();
            var images = new List<IFormFile>
        {
            CreateMockFormFile("file1.jpg", 1024)
        };
            var model = new ComplaintViewModel
            {
                OrderDetailID = orderDetailId,
                Description = "The product arrived damaged.",
                Images = images
            };
            _orderDetailServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>())).ReturnsAsync((OrderDetail)null);

            // Act
            var result = await _controller.SubmitComplaint(model) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse((bool)GetPropertyValue(result.Value, "success"));
            Assert.AreEqual("There are no products in this order.", GetPropertyValue(result.Value, "message"));
        }

        [Test]
        public async Task SubmitComplaint_ExceptionThrown_ReturnsError()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            var orderDetailId = Guid.NewGuid();
            var orderDetail = new OrderDetail { ID = orderDetailId, OrderID = Guid.NewGuid(), TotalPrice = 100 };
            var order = new Order { ID = orderDetail.OrderID, VoucherID = null, TotalPrice = 100 };
            var images = new List<IFormFile>
        {
            CreateMockFormFile("file1.jpg", 1024)
        };
            var model = new ComplaintViewModel
            {
                OrderDetailID = orderDetailId,
                Description = "The product arrived damaged.",
                Images = images
            };
            _orderDetailServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>())).ReturnsAsync(orderDetail);
            _ordersServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(order);
            _complaintServicesMock.Setup(x => x.AddAsync(It.IsAny<Complaint>())).ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _controller.SubmitComplaint(model) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse((bool)GetPropertyValue(result.Value, "success"));
            Assert.AreEqual("An error occurred while submitting your complaint. Please try again later.", GetPropertyValue(result.Value, "message"));
        }

        // Helper to create a mock IFormFile
        private IFormFile CreateMockFormFile(string fileName, long length)
        {
            var ms = new MemoryStream(new byte[length]);
            return new FormFile(ms, 0, length, "file", fileName);
        }
    }
}
