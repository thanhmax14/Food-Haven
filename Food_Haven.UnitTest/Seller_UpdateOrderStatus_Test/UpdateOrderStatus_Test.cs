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
using Microsoft.AspNetCore.SignalR;
using Models;
using Moq;
using NUnit.Framework;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Seller_UpdateOrderStatus_Test
{
    [TestFixture]
    public class UpdateOrderStatus_Test
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

        private void MockListOrderDetails(List<OrderDetail> details)
        {
            _orderDetailServiceMock.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<OrderDetail, bool>>>(),
                It.IsAny<Func<IQueryable<OrderDetail>, IOrderedQueryable<OrderDetail>>>(),
                It.IsAny<Func<IQueryable<OrderDetail>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<OrderDetail, object>>>()
            )).ReturnsAsync(details);
        }

        private void MockFindOrder(Order order)
        {
            _ordersServiceMock.Setup(x => x.FindAsync(
                It.IsAny<Expression<Func<Order, bool>>>()
            )).ReturnsAsync(order);
        }

        private void MockHubSend()
        {
            var clientProxyMock = new Mock<IClientProxy>();

            clientProxyMock
                .Setup(c => c.SendCoreAsync(
                    It.IsAny<string>(),
                    It.IsAny<object[]>(),
                    It.IsAny<CancellationToken>()
                ))
                .Returns(Task.CompletedTask);

            var hubClientsMock = new Mock<IHubClients>();
            hubClientsMock.Setup(c => c.All).Returns(clientProxyMock.Object);

            _hubContextMock.Setup(c => c.Clients).Returns(hubClientsMock.Object);
        }

        [Test]
        public async Task UpdateOrderStatus_PreparingInKitchen_Success()
        {
            var orderId = Guid.NewGuid();
            var order = new Order { ID = orderId, Status = "Pending" };
            var details = new List<OrderDetail> { new OrderDetail { OrderID = orderId } };

            MockFindOrder(order);
            MockListOrderDetails(details);
            _ordersServiceMock.Setup(x => x.UpdateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            _orderDetailServiceMock.Setup(x => x.UpdateAsync(It.IsAny<OrderDetail>())).Returns(Task.CompletedTask);
            _ordersServiceMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _orderDetailServiceMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockHubSend();

            var result = await _controller.UpdateOrderStatus(orderId, "PREPARING IN KITCHEN") as JsonResult;

            Assert.NotNull(result);
            Assert.IsTrue((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value));
        }

        [Test]
        public async Task UpdateOrderStatus_Delivering_Success()
        {
            var orderId = Guid.NewGuid();
            var order = new Order { ID = orderId, Status = "Pending" };
            var details = new List<OrderDetail> { new OrderDetail { OrderID = orderId } };

            MockFindOrder(order);
            MockListOrderDetails(details);
            _ordersServiceMock.Setup(x => x.UpdateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            _ordersServiceMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockHubSend();

            var result = await _controller.UpdateOrderStatus(orderId, "DELIVERING") as JsonResult;
            Assert.IsTrue((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value));
        }

        [Test]
        public async Task UpdateOrderStatus_Confirmed_Success()
        {
            var orderId = Guid.NewGuid();
            var order = new Order { ID = orderId, Status = "Pending" };
            var details = new List<OrderDetail> { new OrderDetail { OrderID = orderId } };

            // Mock user & store
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new AppUser { Id = "fake-user-id" });

            _storeDetailServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync(new StoreDetails { ID = Guid.NewGuid(), UserID = "fake-user-id" });

            // Mock order + details
            MockFindOrder(order);
            MockListOrderDetails(details);

            // Mock update
            _ordersServiceMock.Setup(x => x.UpdateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            _orderDetailServiceMock.Setup(x => x.UpdateAsync(It.IsAny<OrderDetail>())).Returns(Task.CompletedTask);
            _ordersServiceMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _orderDetailServiceMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Mock hub
            MockHubSend();

            var result = await _controller.UpdateOrderStatus(orderId, "CONFIRMED") as JsonResult;
            Assert.IsTrue((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value));
        }



        [Test]
        public async Task UpdateOrderStatus_CancelledByUser_Fail()
        {
            var orderId = Guid.NewGuid();
            MockFindOrder(new Order { ID = orderId, Status = "Pending" });

            var result = await _controller.UpdateOrderStatus(orderId, "CANCELLED BY USER") as JsonResult;
            Assert.IsFalse((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value));
        }

        [Test]
        public async Task UpdateOrderStatus_CancelledByShop_Fail()
        {
            var orderId = Guid.NewGuid();
            MockFindOrder(new Order { ID = orderId, Status = "Pending" });

            var result = await _controller.UpdateOrderStatus(orderId, "CANCELLED BY SHOP") as JsonResult;
            Assert.IsFalse((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value));
        }

        [Test]
        public async Task UpdateOrderStatus_OrderNotFound_Fail()
        {
            MockFindOrder(null);

            var result = await _controller.UpdateOrderStatus(Guid.NewGuid(), "CONFIRMED") as JsonResult;
            Assert.IsFalse((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value));
        }

        [Test]
        public async Task UpdateOrderStatus_NoOrderDetails_Fail()
        {
            var orderId = Guid.NewGuid();
            MockFindOrder(new Order { ID = orderId, Status = "Pending" });
            MockListOrderDetails(new List<OrderDetail>());

            var result = await _controller.UpdateOrderStatus(orderId, "CONFIRMED") as JsonResult;
            Assert.IsFalse((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value));
        }

        [Test]
        public async Task UpdateOrderStatus_ExceptionThrown_Fail()
        {
            _ordersServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Order, bool>>>())).ThrowsAsync(new Exception("DB error"));

            var result = await _controller.UpdateOrderStatus(Guid.NewGuid(), "CONFIRMED") as JsonResult;
            Assert.IsFalse((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value));
        }
    }
}
