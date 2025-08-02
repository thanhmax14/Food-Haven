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
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Models;
using Moq;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Seller_ToggleStatus_Test
{
    [TestFixture]
    public class ToggleStatus_Test
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
        public async Task ToggleStatus_ReturnsOk_WhenProductStatusUpdatedSuccessfully()
        {
            // Arrange
            var productId = Guid.Parse("ed1c69fa-c1f5-45f1-a9d0-79c9f2120269");
            _productServiceMock.Setup(s => s.ToggleProductStatus(productId)).ReturnsAsync(true);

            // Act
            var result = await _controller.ToggleStatus(productId, true);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var value = okResult.Value;
            var successProp = value.GetType().GetProperty("success");
            var messageProp = value.GetType().GetProperty("message");

            Assert.IsNotNull(successProp);
            Assert.IsNotNull(messageProp);

            Assert.IsTrue((bool)successProp.GetValue(value));
            Assert.AreEqual("Product status updated successfully!", (string)messageProp.GetValue(value));
        }

        [Test]
        public async Task ToggleStatus_ReturnsNotFound_WhenProductNotFound()
        {
            // Arrange
            var productId = Guid.Parse("ed1c69fa-c1f5-45f1-a9d0-79c9f2120abc");
            _productServiceMock.Setup(s => s.ToggleProductStatus(productId)).ReturnsAsync(false);

            // Act
            var result = await _controller.ToggleStatus(productId, false);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);

            var value = notFoundResult.Value;
            var messageProp = value.GetType().GetProperty("message");

            Assert.IsNotNull(messageProp);
            Assert.AreEqual("Product not found", (string)messageProp.GetValue(value));
        }

        [Test]
        public void ToggleStatus_ThrowsException_WhenServiceThrows()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _productServiceMock.Setup(s => s.ToggleProductStatus(productId)).ThrowsAsync(new Exception("Unexpected error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () =>
            {
                await _controller.ToggleStatus(productId, true);
            });
        }

        [Test]
        public async Task ToggleStatus_BoundaryCondition_ProductIdEmpty_ReturnsNotFound()
        {
            // Arrange
            var productId = Guid.Empty;
            _productServiceMock.Setup(s => s.ToggleProductStatus(productId)).ReturnsAsync(false);

            // Act
            var result = await _controller.ToggleStatus(productId, true);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);

            var value = notFoundResult.Value;
            var messageProp = value.GetType().GetProperty("message");

            Assert.IsNotNull(messageProp);
            Assert.AreEqual("Product not found", (string)messageProp.GetValue(value));
        }
    }
}
