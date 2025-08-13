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
using Models;
using Moq;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Seller_GetStatisticsByDate_Test
{
    public class GetStatisticsByDate_Test
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

        private static IDictionary<string, object> ExtractResultObject(IActionResult result)
        {
            object value = null;

            if (result is JsonResult json)
                value = json.Value;
            else if (result is BadRequestObjectResult badRequest)
                value = badRequest.Value;

            if (value == null)
                return null;

            // If it's already a dictionary, just return it
            if (value is IDictionary<string, object> dict)
                return dict;

            // Convert anonymous object → dictionary using reflection
            var props = value.GetType().GetProperties();
            var dictionary = new Dictionary<string, object>();
            foreach (var prop in props)
            {
                dictionary[prop.Name] = prop.GetValue(value);
            }
            return dictionary;
        }

        [Test]
        public async Task GetStatisticsByDate_NoUser_ReturnsError()
        {
            _userManagerMock
                .Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync((AppUser)null);

            var result = await _controller.GetStatisticsByDate();

            var dict = ExtractResultObject(result);
            Assert.IsNotNull(dict);

            Assert.IsTrue(dict.ContainsKey("error"), "Expected error key in result");
            Assert.AreEqual("You are not logged in!", dict["error"].ToString());
        }

        [Test]
        public async Task GetStatisticsByDate_WithValidData_ReturnsCorrectStatistics()
        {
            // Arrange
            var userId = "user-1";
            var storeId = Guid.NewGuid();

            var user = new AppUser { Id = userId };
            _userManagerMock
                .Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var store = new StoreDetails { ID = storeId, UserID = userId };
            _storeDetailServiceMock
                .Setup(s => s.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync(store);
            _storeDetailService2Mock
                .Setup(s => s.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync(store);

            var product1 = new Product { ID = Guid.NewGuid(), StoreID = storeId };
            var product2 = new Product { ID = Guid.NewGuid(), StoreID = storeId };
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

            var variant1 = new ProductTypes { ID = Guid.NewGuid(), ProductID = product1.ID };
            var variant2 = new ProductTypes { ID = Guid.NewGuid(), ProductID = product2.ID };
            _productVariantServiceMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<ProductTypes, bool>>>(),
                    It.IsAny<Func<IQueryable<ProductTypes>, IOrderedQueryable<ProductTypes>>>(),
                    It.IsAny<Func<IQueryable<ProductTypes>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductTypes, object>>>()))
                .ReturnsAsync(new List<ProductTypes> { variant1, variant2 });

            var order1 = new Order
            {
                ID = Guid.NewGuid(),
                CreatedDate = DateTime.Today.AddDays(-1),
                Status = "CONFIRMED",
                TotalPrice = 200m
            };
            var order2 = new Order
            {
                ID = Guid.NewGuid(),
                CreatedDate = DateTime.Today.AddDays(-2),
                Status = "Cancelled by User",
                TotalPrice = 100m
            };
            _ordersServiceMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(),
                    It.IsAny<Func<IQueryable<Order>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Order, object>>>()))
                .ReturnsAsync(new List<Order> { order1, order2 });

            var od1 = new OrderDetail
            {
                OrderID = order1.ID,
                ProductTypesID = variant1.ID,
                ProductPrice = 50m,
                Quantity = 2,
                CommissionPercent = 10
            };
            var od2 = new OrderDetail
            {
                OrderID = order2.ID,
                ProductTypesID = variant2.ID,
                ProductPrice = 100m,
                Quantity = 1,
                CommissionPercent = 10
            };

            // Make OrderDetailService respect predicate so only relevant details are returned for CONFIRMED order
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
                    return all.Where(predicate).ToList();
                });

            // Act
            var result = await _controller.GetStatisticsByDate();

            // Assert
            var dict = ExtractResultObject(result);
            Assert.IsNotNull(dict, "Expected the result to be a dictionary-like object");

            if (dict.ContainsKey("error"))
                Assert.Fail("Unexpected error in result: " + dict["error"]);

            Assert.AreEqual(2, Convert.ToInt32(dict["totalOrders"]));
            // Commission = 50*2*0.1 = 10, Earnings = 200 - 10 = 190
            Assert.AreEqual(190m, Convert.ToDecimal(dict["totalEarnings"]));
        }

        [Test]
        public async Task GetStatisticsByDate_FromGreaterThanTo_ReturnsBadRequest()
        {
            var userId = "user-1";
            var storeId = Guid.NewGuid();

            var user = new AppUser { Id = userId };
            _userManagerMock
                .Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var store = new StoreDetails { ID = storeId, UserID = userId };
            _storeDetailServiceMock
                .Setup(s => s.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync(store);
            _storeDetailService2Mock
                .Setup(s => s.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync(store);

            // Setup required products, variants, orders, and order details to avoid short-circuit
            var product = new Product { ID = Guid.NewGuid(), StoreID = storeId };
            _productServiceMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<Func<IQueryable<Product>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Product, object>>>()))
                .ReturnsAsync(new List<Product> { product });
            _productService2Mock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<Func<IQueryable<Product>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Product, object>>>()))
                .ReturnsAsync(new List<Product> { product });

            var variant = new ProductTypes { ID = Guid.NewGuid(), ProductID = product.ID };
            _productVariantServiceMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<ProductTypes, bool>>>(),
                    It.IsAny<Func<IQueryable<ProductTypes>, IOrderedQueryable<ProductTypes>>>(),
                    It.IsAny<Func<IQueryable<ProductTypes>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductTypes, object>>>()))
                .ReturnsAsync(new List<ProductTypes> { variant });

            var order = new Order
            {
                ID = Guid.NewGuid(),
                CreatedDate = DateTime.Today,
                Status = "CONFIRMED",
                TotalPrice = 100m
            };
            _ordersServiceMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(),
                    It.IsAny<Func<IQueryable<Order>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Order, object>>>()))
                .ReturnsAsync(new List<Order> { order });

            var od = new OrderDetail
            {
                OrderID = order.ID,
                ProductTypesID = variant.ID,
                ProductPrice = 100m,
                Quantity = 1,
                CommissionPercent = 10
            };
            _orderDetailServiceMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<OrderDetail, bool>>>(),
                    It.IsAny<Func<IQueryable<OrderDetail>, IOrderedQueryable<OrderDetail>>>(),
                    It.IsAny<Func<IQueryable<OrderDetail>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<OrderDetail, object>>>()))
                .ReturnsAsync(new List<OrderDetail> { od });

            // Act
            var result = await _controller.GetStatisticsByDate(DateTime.Today, DateTime.Today.AddDays(-1));

            var dict = ExtractResultObject(result);
            Assert.IsNotNull(dict);
            Assert.IsTrue(dict.ContainsKey("error"), "Expected error key in result");
            Assert.AreEqual("Start date cannot be greater than end date.", dict["error"].ToString());
        }
    }
}
