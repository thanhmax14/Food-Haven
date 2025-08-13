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
using Moq;
using NUnit.Framework;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Seller_ProductStatistics_Test
{
    public class ProductStatistics_Test
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

        [Test]
        public async Task ProductStatistics_NoUser_ReturnsError()
        {
            _userManagerMock
                .Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync((AppUser)null);

            var result = await _controller.ProductStatistics();

            var dict = ExtractJsonResultDict(result);
            Assert.IsNotNull(dict);
            Assert.IsTrue(dict.ContainsKey("error"));
            Assert.AreEqual("You are not logged in!", dict["error"].ToString());
        }

        [Test]
        public async Task ProductStatistics_StoreNotFound_ReturnsError()
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

            var result = await _controller.ProductStatistics();

            var dict = ExtractJsonResultDict(result);
            Assert.IsNotNull(dict);
            Assert.IsTrue(dict.ContainsKey("error"));
            Assert.AreEqual("Store not found!", dict["error"].ToString());
        }

        [Test]
        public async Task ProductStatistics_NoProducts_ReturnsEmptySeries()
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

            var result = await _controller.ProductStatistics();

            var dict = ExtractJsonResultDict(result);
            Assert.IsNotNull(dict);
            Assert.IsTrue(dict.ContainsKey("success"));
            Assert.IsTrue((bool)dict["success"]);
            var data = dict["data"];
            Assert.IsNotNull(data);
            Assert.AreEqual(0, (int)data.GetType().GetProperty("total").GetValue(data));
        }

        [Test]
        public async Task ProductStatistics_ValidProductsAndOrders_ReturnsCorrectStatistics()
        {
            // Arrange user and store
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

            // Products
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

            // ProductTypes
            var variant1 = new ProductTypes { ID = Guid.NewGuid(), ProductID = product1.ID, Name = "Red", SellPrice = 10, Stock = 100 };
            var variant2 = new ProductTypes { ID = Guid.NewGuid(), ProductID = product2.ID, Name = "Big", SellPrice = 20, Stock = 50 };
            _productVariantServiceMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<ProductTypes, bool>>>(),
                    It.IsAny<Func<IQueryable<ProductTypes>, IOrderedQueryable<ProductTypes>>>(),
                    It.IsAny<Func<IQueryable<ProductTypes>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductTypes, object>>>()))
                .ReturnsAsync(new List<ProductTypes> { variant1, variant2 });

            // OrderDetails
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

            // Orders
            var order1 = new Order
            {
                ID = od1.OrderID,
                CreatedDate = today,
                Status = "CONFIRMED"
            };
            var order2 = new Order
            {
                ID = od2.OrderID,
                CreatedDate = today,
                Status = "CONFIRMED"
            };
            _ordersServiceMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(),
                    It.IsAny<Func<IQueryable<Order>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Order, object>>>()))
                .ReturnsAsync(new List<Order> { order1, order2 });

            // Act
            var result = await _controller.ProductStatistics("today");

            // Assert
            var dict = ExtractJsonResultDict(result);
            Assert.IsNotNull(dict);
            Assert.IsTrue(dict.ContainsKey("success"));
            Assert.IsTrue((bool)dict["success"]);
            var data = dict["data"];
            Assert.IsNotNull(data);

            var total = (int)data.GetType().GetProperty("total").GetValue(data);
            var labels = (string[])data.GetType().GetProperty("labels").GetValue(data);
            var series = (int[])data.GetType().GetProperty("series").GetValue(data);

            Assert.AreEqual(3, total); // <-- Fix: Should be 3, not 2
            Assert.AreEqual(2, labels.Length);
            Assert.Contains("Cookie (Red)", labels);
            Assert.Contains("Cake (Big)", labels);
            Assert.AreEqual(3, series[0] + series[1]);
        }
    }
}