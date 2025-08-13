using AutoMapper;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Categorys;
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
using Food_Haven.Web.Controllers;
using Food_Haven.Web.Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Models;
using Models.DBContext;
using Moq;
using NUnit.Framework;
using Repository.BalanceChange;
using Repository.StoreDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using static Food_Haven.UnitTest.Admin_GetComplaint_Test.GetComplaint_Test;

namespace Food_Haven.UnitTest.Admin_GetTopStores_Test
{
    public class GetTopStores_Test
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<ITypeOfDishService> _typeOfDishServiceMock;
        private Mock<IIngredientTagService> _ingredientTagServiceMock;
        private Mock<IStoreDetailService> _storeServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private Mock<StoreDetailsRepository> _storeRepositoryMock;
        private Mock<IBalanceChangeService> _balanceMock;
        private Mock<ICategoryService> _categoryServiceMock;
        private ManageTransaction _manageTransaction;
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
        private List<AppUser> _allAppUsers;
        private Mock<IQueryable<AppUser>> _usersQueryableMock;

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
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
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
            _manageTransaction = manageTransactionMock.Object;

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
            var hubContextMock = new Mock<IHubContext<ChatHub>>();

            _expertRecipeServicesMock = new Mock<IExpertRecipeServices>();

            _controller = new AdminController(
                _userManagerMock.Object,
                _typeOfDishServiceMock.Object,
                _ingredientTagServiceMock.Object,
                _storeServiceMock.Object,
                _mapperMock.Object,
                _webHostEnvironmentMock.Object,
                _balanceMock.Object,
                _categoryServiceMock.Object,
                _manageTransaction,
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

            // Setup Users queryable for UserManager.Users (for LINQ ToListAsync support)
            _allAppUsers = new List<AppUser>();
            var usersQueryable = _allAppUsers.AsQueryable();
            var mockUsers = new Mock<IQueryable<AppUser>>();
            mockUsers.As<IAsyncEnumerable<AppUser>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<AppUser>(usersQueryable.GetEnumerator()));
            mockUsers.As<IQueryable<AppUser>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<AppUser>(usersQueryable.Provider));
            mockUsers.As<IQueryable<AppUser>>().Setup(m => m.Expression).Returns(usersQueryable.Expression);
            mockUsers.As<IQueryable<AppUser>>().Setup(m => m.ElementType).Returns(usersQueryable.ElementType);
            mockUsers.As<IQueryable<AppUser>>().Setup(m => m.GetEnumerator()).Returns(usersQueryable.GetEnumerator());
            _usersQueryableMock = mockUsers;
            _userManagerMock.Setup(m => m.Users).Returns(_usersQueryableMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        private static IDictionary<string, object> ExtractJsonResultDict(IActionResult result)
        {
            if (result is JsonResult json && json.Value is IDictionary<string, object> dict)
                return dict;
            if (result is JsonResult json2)
            {
                var value = json2.Value;
                var props = value.GetType().GetProperties();
                var dictionary = new Dictionary<string, object>();
                foreach (var prop in props)
                {
                    dictionary[prop.Name] = prop.GetValue(value);
                }
                return dictionary;
            }
            return null;
        }

        private static IList<dynamic> ExtractJsonResultListFromData(IActionResult result)
        {
            var dict = ExtractJsonResultDict(result);
            if (dict != null && dict.ContainsKey("data"))
            {
                var data = dict["data"];
                if (data is IEnumerable<object> objEnum)
                    return objEnum.Cast<dynamic>().ToList();
                if (data is System.Collections.IEnumerable enumerable)
                    return enumerable.Cast<dynamic>().ToList();
            }
            return null;
        }

        private object GetProp(object obj, string prop)
        {
            return obj?.GetType().GetProperty(prop)?.GetValue(obj);
        }

        [Test]
        public async Task GetTopStores_ReturnsCorrectStores()
        {
            // Use a fixed "now" so dates are stable for the test
            var fixedNow = new DateTime(2025, 8, 13, 12, 0, 0, DateTimeKind.Utc);

            // Seed users
            var user1 = new AppUser { Id = "user1", UserName = "seller1", FirstName = "John", LastName = "Doe" };
            var user2 = new AppUser { Id = "user2", UserName = "seller2", FirstName = "Jane", LastName = "Smith" };
            _allAppUsers.Clear();
            _allAppUsers.AddRange(new[] { user1, user2 });

            // Seed stores -- ensure CreatedDate is within last7days for store2, outside for store1
            var store1 = new StoreDetails { ID = Guid.NewGuid(), UserID = "user1", Name = "Store Alpha", CreatedDate = fixedNow.AddDays(-10), IsActive = true };
            var store2 = new StoreDetails { ID = Guid.NewGuid(), UserID = "user2", Name = "Store Beta", CreatedDate = fixedNow.AddDays(-2), IsActive = false };
            _storeServiceMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<StoreDetails, bool>>>(),
                    It.IsAny<Func<IQueryable<StoreDetails>, IOrderedQueryable<StoreDetails>>>(),
                    It.IsAny<Func<IQueryable<StoreDetails>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StoreDetails, object>>>()))
                .ReturnsAsync(new List<StoreDetails> { store1, store2 });
            _storeServiceMock
                .Setup(s => s.ListAsync())
                .ReturnsAsync(new List<StoreDetails> { store1, store2 });

            // Seed products
            var prod1 = new Product { ID = Guid.NewGuid(), StoreID = store1.ID, Name = "Pasta" };
            var prod2 = new Product { ID = Guid.NewGuid(), StoreID = store2.ID, Name = "Cake" };
            _productMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<Func<IQueryable<Product>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Product, object>>>()))
                .ReturnsAsync(new List<Product> { prod1, prod2 });
            _productMock
                .Setup(s => s.ListAsync())
                .ReturnsAsync(new List<Product> { prod1, prod2 });

            // Seed variants
            var variant1 = new ProductTypes { ID = Guid.NewGuid(), ProductID = prod1.ID, Name = "Large" };
            var variant2 = new ProductTypes { ID = Guid.NewGuid(), ProductID = prod2.ID, Name = "Small" };
            _variantServiceMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<ProductTypes, bool>>>(),
                    It.IsAny<Func<IQueryable<ProductTypes>, IOrderedQueryable<ProductTypes>>>(),
                    It.IsAny<Func<IQueryable<ProductTypes>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductTypes, object>>>()))
                .ReturnsAsync(new List<ProductTypes> { variant1, variant2 });
            _variantServiceMock
                .Setup(s => s.ListAsync())
                .ReturnsAsync(new List<ProductTypes> { variant1, variant2 });

            // Seed order details (all within period)
            var od1 = new OrderDetail
            {
                OrderID = Guid.NewGuid(),
                ProductTypesID = variant1.ID,
                ProductPrice = 12m,
                Quantity = 2,
                CreatedDate = fixedNow.AddDays(-2), // within last 7 days
                Status = "CONFIRMED"
            };
            var od2 = new OrderDetail
            {
                OrderID = Guid.NewGuid(),
                ProductTypesID = variant2.ID,
                ProductPrice = 8m,
                Quantity = 1,
                CreatedDate = fixedNow.AddDays(-1), // within last 7 days
                Status = "CONFIRMED"
            };
            _orderDetailMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<OrderDetail, bool>>>(),
                    It.IsAny<Func<IQueryable<OrderDetail>, IOrderedQueryable<OrderDetail>>>(),
                    It.IsAny<Func<IQueryable<OrderDetail>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<OrderDetail, object>>>()))
                .ReturnsAsync(new List<OrderDetail> { od1, od2 });
            _orderDetailMock
                .Setup(s => s.ListAsync())
                .ReturnsAsync(new List<OrderDetail> { od1, od2 });

            // Seed orders
            var order1 = new Order
            {
                ID = od1.OrderID,
                CreatedDate = fixedNow.AddDays(-2),
                Status = "CONFIRMED",
                UserID = "customer-x",
                OrderTracking = "OT1",
                TotalPrice = 100m
            };
            var order2 = new Order
            {
                ID = od2.OrderID,
                CreatedDate = fixedNow.AddDays(-1),
                Status = "CONFIRMED",
                UserID = "customer-y",
                OrderTracking = "OT2",
                TotalPrice = 50m
            };
            _orderMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(),
                    It.IsAny<Func<IQueryable<Order>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Order, object>>>()))
                .ReturnsAsync(new List<Order> { order1, order2 });
            _orderMock
                .Setup(s => s.ListAsync())
                .ReturnsAsync(new List<Order> { order1, order2 });

            // Act
            var result = await _controller.GetTopStores("last7days", "");

            // Assert
            var dict = ExtractJsonResultDict(result);
            Assert.IsNotNull(dict);
            Assert.IsTrue((bool)dict["success"]);
            var list = ExtractJsonResultListFromData(result);
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);

            var storeA = list.FirstOrDefault(x => GetProp(x, "name")?.ToString() == "Store Alpha");
            var storeB = list.FirstOrDefault(x => GetProp(x, "name")?.ToString() == "Store Beta");

            Assert.IsNotNull(storeA);
            Assert.AreEqual("John Doe", GetProp(GetProp(storeA, "customer"), "name"));
            Assert.AreEqual(1, GetProp(storeA, "quantity")); // Only 1 product
            Assert.AreEqual(100m, GetProp(storeA, "amount"));
            Assert.AreEqual("Active", GetProp(storeA, "status"));

            Assert.IsNotNull(storeB);
            Assert.AreEqual("Jane Smith", GetProp(GetProp(storeB, "customer"), "name"));
            Assert.AreEqual(1, GetProp(storeB, "quantity")); // Only 1 product
            Assert.AreEqual(50m, GetProp(storeB, "amount"));
            Assert.AreEqual("Banned", GetProp(storeB, "status"));
        }

       
    }
}