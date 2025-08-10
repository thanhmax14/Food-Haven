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
using Moq;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Food_Haven.Web.Controllers;
using Models;
using System.Linq.Expressions;

namespace Food_Haven.UnitTest.Seller_GetValidMonths_Test
{
    [TestFixture]
    public class GetValidMonths_Test
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

        [Test]
        public async Task GetValidMonths_ReturnsMonths_WhenDataExists()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var store = new StoreDetails { ID = Guid.NewGuid(), UserID = user.Id };
            _storeDetailService2Mock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>())).ReturnsAsync(store);

            var products = new List<Product> { new Product { ID = Guid.NewGuid(), StoreID = store.ID } };
            _productService2Mock.Setup(x => x.ListAsync(
    It.IsAny<Expression<Func<Product, bool>>>(),
    null,
    null)).ReturnsAsync(products);

            var variants = new List<ProductTypes> { new ProductTypes { ID = Guid.NewGuid(), ProductID = products[0].ID } };
            _productVariantServiceMock.Setup(x => x.ListAsync(
    It.IsAny<Expression<Func<ProductTypes, bool>>>(),
    null,
    null)).ReturnsAsync(variants);

            var orderDetails = new List<OrderDetail> { new OrderDetail { OrderID = Guid.NewGuid(), ProductTypesID = variants[0].ID } };
            _orderDetailServiceMock.Setup(x => x.ListAsync(
    It.IsAny<Expression<Func<OrderDetail, bool>>>(),
    null,
    null)).ReturnsAsync(orderDetails);

            var orders = new List<Order> { new Order { ID = orderDetails[0].OrderID, Status = "CONFIRMED", CreatedDate = new DateTime(2024, 7, 1) } };
            _ordersServiceMock.Setup(x => x.ListAsync(
    It.IsAny<Expression<Func<Order, bool>>>(),
    null,
    null)).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetValidMonths() as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            var months = result.Value as List<string>;
            Assert.IsNotNull(months);
            Assert.Contains("2024-07", months);
        }

        [Test]
        public async Task GetValidMonths_ReturnsFallbackMonths_WhenExceptionThrown()
        {
            // Arrange
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _controller.GetValidMonths() as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            Assert.IsTrue(result.Value.ToString().Contains("Error retrieving month configuration"));
        }
    }
}
