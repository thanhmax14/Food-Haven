using AutoMapper;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.ComplaintImages;
using BusinessLogic.Services.Complaints;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using BusinessLogic.Services.VoucherServices;
using Food_Haven.Web.Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using NUnit.Framework;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using static Food_Haven.UnitTest.Admin_GetComplaint_Test.GetComplaint_Test;

namespace Food_Haven.UnitTest.Seller_RecentOrders_Test
{
    public class RecentOrders_Test
    {
        private SellerController _controller;
        private Mock<IReviewService> _reviewServiceMock;
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<IProductService> _productServiceMock;
        private Mock<IStoreDetailService> _storeDetailServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private Mock<IProductVariantService> _productVariantServiceMock;
        private Mock<IOrdersServices> _ordersServiceMock;
        private Mock<IBalanceChangeService> _balanceChangeServiceMock;
        private Mock<IOrderDetailService> _orderDetailServiceMock;
        private Mock<IStoreDetailService> _storeDetailService2Mock;
        private Mock<IProductService> _productService2Mock;
        private Mock<IVoucherServices> _voucherServiceMock;
        private Mock<IProductImageService> _productImageServiceMock;
        private Mock<IComplaintImageServices> _complaintImageServicesMock;
        private Mock<IComplaintServices> _complaintServiceMock;
        private Mock<ManageTransaction> _manageTransactionMock;
        private Mock<IHubContext<ChatHub>> _hubContextMock;

        private List<AppUser> _allAppUsers;
        private Mock<IQueryable<AppUser>> _usersQueryableMock;

        [SetUp]
        public void Setup()
        {
            _reviewServiceMock = new Mock<IReviewService>();
            _userManagerMock = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            _productServiceMock = new Mock<IProductService>();
            _storeDetailServiceMock = new Mock<IStoreDetailService>();
            _mapperMock = new Mock<IMapper>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _productVariantServiceMock = new Mock<IProductVariantService>();
            _ordersServiceMock = new Mock<IOrdersServices>();
            _balanceChangeServiceMock = new Mock<IBalanceChangeService>();
            _orderDetailServiceMock = new Mock<IOrderDetailService>();
            _storeDetailService2Mock = new Mock<IStoreDetailService>();
            _productService2Mock = new Mock<IProductService>();
            _voucherServiceMock = new Mock<IVoucherServices>();
            _productImageServiceMock = new Mock<IProductImageService>();
            _complaintImageServicesMock = new Mock<IComplaintImageServices>();
            _complaintServiceMock = new Mock<IComplaintServices>();
            _manageTransactionMock = new Mock<ManageTransaction>();
            _hubContextMock = new Mock<IHubContext<ChatHub>>();

            _controller = new SellerController(
                _reviewServiceMock.Object,
                _userManagerMock.Object,
                _productServiceMock.Object,
                _storeDetailServiceMock.Object,
                _mapperMock.Object,
                _webHostEnvironmentMock.Object,
                _productVariantServiceMock.Object,
                _ordersServiceMock.Object,
                _balanceChangeServiceMock.Object,
                _orderDetailServiceMock.Object,
                _storeDetailService2Mock.Object,
                _productService2Mock.Object,
                _voucherServiceMock.Object,
                _productImageServiceMock.Object,
                _complaintImageServicesMock.Object,
                _complaintServiceMock.Object,
                null,
                _hubContextMock.Object
            );

            // Setup Users queryable for UserManager.Users
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
                // handle if controller returns new { success=true, data = list } where list is List<anon>
                var dataType = data.GetType();
                if (dataType.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(dataType.GetGenericTypeDefinition()))
                {
                    var list = ((System.Collections.IEnumerable)data).Cast<dynamic>().ToList();
                    return list;
                }
            }
            return null;
        }

        [Test]
        public async Task RecentOrders_NoUser_ReturnsError()
        {
            _userManagerMock
                .Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync((AppUser)null);

            var result = await _controller.RecentOrders();

            var dict = ExtractJsonResultDict(result);
            Assert.IsNotNull(dict);
            Assert.IsTrue(dict.ContainsKey("error"));
            Assert.AreEqual("You are not logged in!", dict["error"].ToString());
        }

        [Test]
        public async Task RecentOrders_StoreNotFound_ReturnsError()
        {
            var user = new AppUser { Id = "user-1" };
            _userManagerMock
                .Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _storeDetailServiceMock
                .Setup(s => s.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync((StoreDetails)null);
            _storeDetailService2Mock
                .Setup(s => s.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync((StoreDetails)null);

            var result = await _controller.RecentOrders();

            var dict = ExtractJsonResultDict(result);
            Assert.IsNotNull(dict);
            Assert.IsTrue(dict.ContainsKey("error"));
            Assert.AreEqual("Store not found!", dict["error"].ToString());
        }

        [Test]
        public async Task RecentOrders_NoProducts_ReturnsEmptySuccess()
        {
            var user = new AppUser { Id = "user-1" };
            var store = new StoreDetails { ID = Guid.NewGuid(), UserID = user.Id };
            _userManagerMock
                .Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _storeDetailServiceMock
                .Setup(s => s.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync(store);
            _storeDetailService2Mock
                .Setup(s => s.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync(store);

            _productServiceMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<Func<IQueryable<Product>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Product, object>>>()))
                .ReturnsAsync(new List<Product>());
            _productService2Mock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<Func<IQueryable<Product>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Product, object>>>()))
                .ReturnsAsync(new List<Product>());

            var result = await _controller.RecentOrders();

            var dict = ExtractJsonResultDict(result);
            Assert.IsNotNull(dict);
            Assert.IsTrue(dict.ContainsKey("success"));
            Assert.IsTrue((bool)dict["success"]);
            var data = dict["data"] as IList<object>;
            Assert.IsNotNull(data);
            Assert.AreEqual(0, data.Count);
        }
        [Test]
        public async Task RecentOrders_Valid_ReturnsOrdersList()
        {
            // Arrange user and store
            var seller = new AppUser { Id = "seller-1" };
            var store = new StoreDetails { ID = Guid.NewGuid(), UserID = seller.Id };
            _userManagerMock
                .Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(seller);
            _storeDetailServiceMock
                .Setup(s => s.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync(store);
            _storeDetailService2Mock
                .Setup(s => s.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync(store);

            var product1 = new Product { ID = Guid.NewGuid(), StoreID = store.ID, Name = "Cookie" };
            var product2 = new Product { ID = Guid.NewGuid(), StoreID = store.ID, Name = "Cake" };
            _productServiceMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<Func<IQueryable<Product>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Product, object>>>()))
                .ReturnsAsync(new List<Product> { product1, product2 });
            _productService2Mock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<Func<IQueryable<Product>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Product, object>>>()))
                .ReturnsAsync(new List<Product> { product1, product2 });

            var variant1 = new ProductTypes { ID = Guid.NewGuid(), ProductID = product1.ID, Name = "Red", SellPrice = 10, Stock = 100 };
            var variant2 = new ProductTypes { ID = Guid.NewGuid(), ProductID = product2.ID, Name = "Big", SellPrice = 20, Stock = 50 };
            _productVariantServiceMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<ProductTypes, bool>>>(),
                    It.IsAny<Func<IQueryable<ProductTypes>, IOrderedQueryable<ProductTypes>>>(),
                    It.IsAny<Func<IQueryable<ProductTypes>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductTypes, object>>>()))
                .ReturnsAsync(new List<ProductTypes> { variant1, variant2 });

            var today = DateTime.Now.Date;
            var od1 = new OrderDetail
            {
                OrderID = Guid.NewGuid(),
                ProductTypesID = variant1.ID,
                ProductPrice = 10m,
                Quantity = 2,
                CreatedDate = today,
                Status = "CONFIRMED"
            };
            var od2 = new OrderDetail
            {
                OrderID = Guid.NewGuid(),
                ProductTypesID = variant2.ID,
                ProductPrice = 20m,
                Quantity = 1,
                CreatedDate = today,
                Status = "CONFIRMED"
            };
            _orderDetailServiceMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<OrderDetail, bool>>>(),
                    It.IsAny<Func<IQueryable<OrderDetail>, IOrderedQueryable<OrderDetail>>>(),
                    It.IsAny<Func<IQueryable<OrderDetail>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<OrderDetail, object>>>()))
                .ReturnsAsync((Expression<Func<OrderDetail, bool>> predicate,
                               Func<IQueryable<OrderDetail>, IOrderedQueryable<OrderDetail>> orderBy,
                               Func<IQueryable<OrderDetail>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<OrderDetail, object>> include) =>
                {
                    var all = new List<OrderDetail> { od1, od2 }.AsQueryable();
                    if (predicate != null)
                        return all.Where(predicate).ToList();
                    return all.ToList();
                });

            // Orders, each by a different customer, each with an OrderTracking
            var order1 = new Order
            {
                ID = od1.OrderID,
                CreatedDate = today,
                Status = "CONFIRMED",
                UserID = "customer-1",
                OrderTracking = "TRACK-1",
                TotalPrice = 100m
            };
            var order2 = new Order
            {
                ID = od2.OrderID,
                CreatedDate = today,
                Status = "CONFIRMED",
                UserID = "customer-2",
                OrderTracking = "TRACK-2",
                TotalPrice = 50m
            };
            _ordersServiceMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(),
                    It.IsAny<Func<IQueryable<Order>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Order, object>>>()))
                .ReturnsAsync(new List<Order> { order1, order2 });

            // Customers
            var customer1 = new AppUser { Id = "customer-1", UserName = "Alice", FirstName = "Alice", LastName = "A" };
            var customer2 = new AppUser { Id = "customer-2", UserName = "Bob", FirstName = "Bob", LastName = "B" };
            _allAppUsers.Clear();
            _allAppUsers.AddRange(new[] { customer1, customer2, seller });

            // Act
            var result = await _controller.RecentOrders("today");

            // Assert
            var dict = ExtractJsonResultDict(result);
            Assert.IsNotNull(dict);
            Assert.IsTrue((bool)dict["success"]);
            var list = ExtractJsonResultListFromData(result);
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);

            // Use reflection for property access
            var orderObj1 = list.FirstOrDefault(x => GetProp(x, "id")?.ToString() == "TRACK-1");
            var orderObj2 = list.FirstOrDefault(x => GetProp(x, "id")?.ToString() == "TRACK-2");

            Assert.IsNotNull(orderObj1);
            Assert.AreEqual("Alice A", GetProp(GetProp(orderObj1, "customer"), "name"));
            Assert.AreEqual(2, GetProp(orderObj1, "quantity"));
            Assert.AreEqual(100m, GetProp(orderObj1, "amount"));

            Assert.IsNotNull(orderObj2);
            Assert.AreEqual("Bob B", GetProp(GetProp(orderObj2, "customer"), "name"));
            Assert.AreEqual(1, GetProp(orderObj2, "quantity"));
            Assert.AreEqual(50m, GetProp(orderObj2, "amount"));
        }

        /// <summary>
        /// Helper for accessing property of anonymous object by name.
        /// </summary>
        private object GetProp(object obj, string prop)
        {
            return obj?.GetType().GetProperty(prop)?.GetValue(obj);
        }

    }
}
