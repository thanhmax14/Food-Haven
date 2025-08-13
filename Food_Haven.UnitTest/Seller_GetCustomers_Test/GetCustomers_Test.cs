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
using Models;
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

namespace Food_Haven.UnitTest.Seller_GetCustomers_Test
{
    public class GetCustomers_Test
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

        private static IList<dynamic> ExtractJsonResultList(IActionResult result)
        {
            if (result is JsonResult json)
            {
                // Handle both List<object> or List<dynamic>
                if (json.Value is IList<dynamic> dynList)
                    return dynList;
                if (json.Value is IList<object> objList)
                    return objList.Cast<dynamic>().ToList();
            }
            return null;
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
        public async Task GetCustomers_NoUser_ReturnsError()
        {
            _userManagerMock
                .Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync((AppUser)null);

            var result = await _controller.GetCustomers();

            var dict = ExtractJsonResultDict(result);
            Assert.IsNotNull(dict);
            Assert.IsTrue(dict.ContainsKey("error"));
            Assert.AreEqual("You are not logged in!", dict["error"].ToString());
        }

        [Test]
        public async Task GetCustomers_StoreNotFound_ReturnsError()
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

            var result = await _controller.GetCustomers();

            var dict = ExtractJsonResultDict(result);
            Assert.IsNotNull(dict);
            Assert.IsTrue(dict.ContainsKey("error"));
            Assert.AreEqual("Store not found!", dict["error"].ToString());
        }

        [Test]
        public async Task GetCustomers_NoProducts_ReturnsEmptyList()
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

            var result = await _controller.GetCustomers();

            var list = ExtractJsonResultList(result);
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);
        }

     

    }

}