using AutoMapper;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.ComplaintImages;
using BusinessLogic.Services.Complaints;
using BusinessLogic.Services.ExpertRecipes;
using BusinessLogic.Services.IngredientTagServices;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.RecipeIngredientTagIngredientTagServices;
using BusinessLogic.Services.RecipeServices;
using BusinessLogic.Services.StoreDetail;
using BusinessLogic.Services.StoreReports;
using BusinessLogic.Services.TypeOfDishServices;
using BusinessLogic.Services.VoucherServices;
using Food_Haven.Web.Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.DBContext;
using Moq;
using Repository.BalanceChange;
using Repository.StoreDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Admin_GetDateConfig_Test
{
    public class GetDateConfig_Test
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<ITypeOfDishService> _typeOfDishServiceMock;
        private Mock<IIngredientTagService> _ingredientTagServiceMock;
        private Mock<IStoreDetailService> _storeServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private Mock<IBalanceChangeService> _balanceMock;
        private Mock<ICategoryService> _categoryServiceMock;
        private Mock<IComplaintServices> _complaintServiceMock;
        private Mock<IOrderDetailService> _orderDetailMock;
        private Mock<IOrdersServices> _orderMock;
        private Mock<IProductVariantService> _variantServiceMock;
        private Mock<IComplaintImageServices> _complaintImageMock;
        private Mock<IProductService> _productMock;
        private Mock<IVoucherServices> _voucherMock;
        private Mock<IRecipeService> _recipeServiceMock;
        private Mock<IStoreReportServices> _storeReportMock;
        private Mock<IProductImageService> _productImageServiceMock;
        private Mock<IRecipeIngredientTagIngredientTagSerivce> _recipeIngredientTagServiceMock;
        private Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private Mock<IExpertRecipeServices> _expertRecipeServicesMock;

        private AdminController _controller;

        [SetUp]
        public void Setup()
        {
            var store = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);
            _typeOfDishServiceMock = new Mock<ITypeOfDishService>();
            _ingredientTagServiceMock = new Mock<IIngredientTagService>();
            _storeServiceMock = new Mock<IStoreDetailService>();
            _mapperMock = new Mock<IMapper>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _balanceMock = new Mock<IBalanceChangeService>();
            _categoryServiceMock = new Mock<ICategoryService>();

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

            _complaintServiceMock = new Mock<IComplaintServices>();
            _orderDetailMock = new Mock<IOrderDetailService>();
            _orderMock = new Mock<IOrdersServices>();
            _variantServiceMock = new Mock<IProductVariantService>();
            _complaintImageMock = new Mock<IComplaintImageServices>();
            _productMock = new Mock<IProductService>();
            _voucherMock = new Mock<IVoucherServices>();
            _recipeServiceMock = new Mock<IRecipeService>();
            _storeReportMock = new Mock<IStoreReportServices>();
            _productImageServiceMock = new Mock<IProductImageService>();
            _recipeIngredientTagServiceMock = new Mock<IRecipeIngredientTagIngredientTagSerivce>();
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);
            _expertRecipeServicesMock = new Mock<IExpertRecipeServices>();
            var hubContextMock = new Mock<IHubContext<ChatHub>>();

            _controller = new AdminController(
                _userManagerMock.Object,
                _typeOfDishServiceMock.Object,
                _ingredientTagServiceMock.Object,
                _storeServiceMock.Object,
                _mapperMock.Object,
                _webHostEnvironmentMock.Object,
                _balanceMock.Object,
                _categoryServiceMock.Object,
                manageTransactionMock.Object,
                _complaintServiceMock.Object,
                _orderDetailMock.Object,
                _orderMock.Object,
                _variantServiceMock.Object,
                _complaintImageMock.Object,
                _storeServiceMock.Object,
                _productMock.Object,
                _voucherMock.Object,
                _recipeServiceMock.Object,
                _storeReportMock.Object,
                _storeReportMock.Object,
                _productImageServiceMock.Object,
                _recipeIngredientTagServiceMock.Object,
                _roleManagerMock.Object,
                _expertRecipeServicesMock.Object,
                hubContextMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        [Test]
        public async Task GetDateConfig_ReturnsListOfDaysOfOrders_WhenServerIsAvailable()
        {
            // Arrange
            var orderList = new List<Order>
            {
                new Order { Status = "CONFIRMED", CreatedDate = DateTime.Today.AddDays(-5) },
                new Order { Status = "DELIVERING", CreatedDate = DateTime.Today }
            };
            _orderMock.Setup(o => o.ListAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(),
                It.IsAny<Func<IQueryable<Order>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Order, object>>>()
            )).ReturnsAsync(orderList);

            var userList = new List<AppUser>
            {
                new AppUser { JoinedDate = DateTime.Today.AddDays(-10) }
            };
            // Fix: Mock Users to return an async enumerable
            var asyncUsers = new TestAsyncEnumerable<AppUser>(userList);
            _userManagerMock.Setup(u => u.Users).Returns(asyncUsers);

            // Act
            var actionResult = await _controller.GetDateConfig();

            object? rawValue = null;
            if (actionResult is JsonResult jsonResult)
                rawValue = jsonResult.Value;
            else if (actionResult is ObjectResult objectResult)
                rawValue = objectResult.Value;

            // Assert
            Assert.IsNotNull(rawValue, "Expected GetDateConfig to return data");
            var data = ToDict(rawValue);

            if (data.ContainsKey("error"))
                Assert.Fail($"Controller returned error: {data["error"]}, message: {data["message"]}");

            Assert.That(GetValueCaseInsensitive(data, "minDate"), Is.EqualTo(DateTime.Today.AddDays(-10).ToString("yyyy-MM-dd")));
            Assert.That(GetValueCaseInsensitive(data, "maxDate"), Is.EqualTo(DateTime.Today.ToString("yyyy-MM-dd")));
            Assert.That(Convert.ToInt32(GetValueCaseInsensitive(data, "defaultDays")), Is.EqualTo(11));
        }

        // Helper for async enumerable
        public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
            public IAsyncEnumerator<T> GetAsyncEnumerator(System.Threading.CancellationToken cancellationToken = default)
                => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;
            public TestAsyncEnumerator(IEnumerator<T> inner) { _inner = inner; }
            public T Current => _inner.Current;
            public ValueTask DisposeAsync() { _inner.Dispose(); return ValueTask.CompletedTask; }
            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
        }

        private static string GetValueCaseInsensitive(Dictionary<string, object> dict, string key)
        {
            var match = dict.FirstOrDefault(k =>
                string.Equals(k.Key, key, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(match.Key))
                throw new KeyNotFoundException($"Key '{key}' not found. Available keys: {string.Join(", ", dict.Keys)}");
            return match.Value?.ToString();
        }



        [Test]
        public async Task GetDateConfig_ReturnsErrorMessage_WhenExceptionOccurs()
        {
            // Arrange
            _orderMock.Setup(o => o.ListAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(),
                It.IsAny<Func<IQueryable<Order>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Order, object>>>()
            ))
            .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetDateConfig() as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            var value = ToDict(result.Value);
            Assert.AreEqual("Error retrieving date configuration", value["error"]?.ToString());
            Assert.IsTrue(value["message"]?.ToString().Contains("Database error"));
        }

        private static Mock<DbSet<T>> CreateDbSetMock<T>(IEnumerable<T> elements) where T : class
        {
            var queryable = elements.AsQueryable();
            var dbSetMock = new Mock<DbSet<T>>();
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            return dbSetMock;
        }

        private static Dictionary<string, object> ToDict(object value)
        {
            if (value is Dictionary<string, object> dict)
                return dict;

            var json = System.Text.Json.JsonSerializer.Serialize(value);
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        }
    }
}
