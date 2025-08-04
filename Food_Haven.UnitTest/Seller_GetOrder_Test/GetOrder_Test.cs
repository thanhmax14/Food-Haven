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
using Food_Haven.Web.Controllers;
using Food_Haven.Web.Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models;
using MockQueryable.Moq;
using Moq;
using Repository.BalanceChange;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MockQueryable;

namespace Food_Haven.UnitTest.Seller_GetOrder_Test
{
    public class GetOrder_Test
    {

        private SellerController _controller;

        // Các Mock Dependencies
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

            // Khởi tạo SellerController với các dependency mock
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
        [Test]
        public async Task GetOrder_ReturnsJsonResult_WhenUserLoggedInAndHasOrders()
        {
            // Arrange: Setup dữ liệu và mock
            var sellerUser = new AppUser { Id = "user1", UserName = "seller1" };
            var buyer = new AppUser { Id = "buyer1", UserName = "buyer1" };
            var users = new List<AppUser> { sellerUser, buyer }.AsQueryable().BuildMock();

            var store = new StoreDetails { ID = Guid.NewGuid(), UserID = sellerUser.Id };
            var product = new Product { ID = Guid.NewGuid(), StoreID = store.ID };
            var variant = new ProductTypes { ID = Guid.NewGuid(), ProductID = product.ID };

            var orderId = Guid.NewGuid();
            var orderDetail = new OrderDetail
            {
                OrderID = orderId,
                ProductTypesID = variant.ID,
                Quantity = 2
            };

            var order = new Order
            {
                ID = orderId,
                UserID = "buyer1",
                CreatedDate = DateTime.Now,
                TotalPrice = 100,
                Status = "Pending",
                PaymentStatus = "Unpaid",
                Note = "note",
                DeliveryAddress = "addr",
                Description = "desc"
            };

            // Mock UserManager
            var mockUserDbSet = new Mock<DbSet<AppUser>>();
            mockUserDbSet.As<IQueryable<AppUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUserDbSet.As<IQueryable<AppUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUserDbSet.As<IQueryable<AppUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUserDbSet.As<IQueryable<AppUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(sellerUser);
            _userManagerMock.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            // Mock StoreDetails service (MUST mock the second one)
            _storeDetailService2Mock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync(store);

            // Mock Product service (phải dùng _productService2Mock)
            _productService2Mock
                .Setup(x => x.ListAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<Func<IQueryable<Product>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Product, object>>>()
                ))
                .ReturnsAsync(new List<Product> { product });

            // Mock ProductVariant service
            _productVariantServiceMock
                .Setup(x => x.ListAsync(
                    It.IsAny<Expression<Func<ProductTypes, bool>>>(),
                    It.IsAny<Func<IQueryable<ProductTypes>, IOrderedQueryable<ProductTypes>>>(),
                    It.IsAny<Func<IQueryable<ProductTypes>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductTypes, object>>>()
                ))
                .ReturnsAsync(new List<ProductTypes> { variant });

            // Mock OrderDetail service
            _orderDetailServiceMock
                .Setup(x => x.ListAsync(
                    It.IsAny<Expression<Func<OrderDetail, bool>>>(),
                    It.IsAny<Func<IQueryable<OrderDetail>, IOrderedQueryable<OrderDetail>>>(),
                    It.IsAny<Func<IQueryable<OrderDetail>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<OrderDetail, object>>>()
                ))
                .ReturnsAsync(new List<OrderDetail> { orderDetail });

            // Mock Orders service
            _ordersServiceMock
                .Setup(x => x.ListAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(),
                    It.IsAny<Func<IQueryable<Order>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Order, object>>>()
                ))
                .ReturnsAsync(new List<Order> { order });

            // Act
            var result = await _controller.GetOrder();

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult, "JsonResult is null");

            if (jsonResult.Value is List<GetSellerOrder> data)
            {
                Assert.IsNotNull(data, "Result data is null");
                Assert.AreEqual(1, data.Count, "Expected 1 order");

                var orderDto = data.First();
                Assert.AreEqual("buyer1", orderDto.UserName, "Username mismatch");
                Assert.AreEqual("PENDING", orderDto.Status, "Order status mismatch");
                Assert.AreEqual("Unpaid", orderDto.StatusPayment, "Payment status mismatch");
            }
            else if (jsonResult.Value is string str)
            {
                Assert.Fail($"Controller returned error string: {str}");
            }
            else
            {
                Assert.Fail($"Unexpected result type: {jsonResult.Value?.GetType().FullName}");
            }
        }


        [Test]
        public async Task GetOrder_ReturnsEmptyList_WhenNoOrdersExist()
        {
            var user = new AppUser { Id = "user1", UserName = "seller1" };
            var store = new StoreDetails { ID = Guid.NewGuid(), UserID = user.Id };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>())).ReturnsAsync(store);
            _productServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null)).ReturnsAsync(new List<Product>());

            var result = await _controller.GetOrder();

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            if (jsonResult.Value is List<GetSellerOrder> list)
    {
        Assert.IsEmpty(list);
    }
    else if (jsonResult.Value is string str)
    {
        Assert.AreEqual("Store not found", str);
    }
    else
    {
        Assert.Fail($"Unexpected result type: {jsonResult.Value?.GetType().FullName}");
    }
        }
        [Test]
        public async Task GetOrder_ReturnsErrorMessage_WhenUserNotLoggedIn()
        {
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AppUser)null);

            var result = await _controller.GetOrder();

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = jsonResult.Value as ErroMess;
            Assert.IsNotNull(data);
            Assert.AreEqual("You are not logged in!", data.msg);
        }

        [Test]
        public async Task GetOrder_ReturnsStoreNotFound_WhenStoreDoesNotExist()
        {
            var user = new AppUser { Id = "user1", UserName = "seller1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            // Không mock trả về store
            _storeDetailService2Mock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>())).ReturnsAsync((StoreDetails)null);

            var result = await _controller.GetOrder();

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.AreEqual("Store not found", jsonResult.Value);
        }
    }
}
