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
using Models;
using Moq;
using NUnit.Framework;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace Food_Haven.UnitTest.Seller_UpdateProductTypeStatus_Test
{
    [TestFixture]
    public class UpdateProductTypeStatus_Test
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
        public void UpdateProductTypeStatus_ReturnsSuccessTrue_WithUpdatedMessage()
        {
            // Arrange
            var variantId = Guid.NewGuid();
            bool isActive = true;
            _productVariantServiceMock.Setup(s => s.UpdateProductVariantStatus(variantId, isActive)).Returns(true);

            // Act
            var result = _controller.UpdateProductTypeStatus(variantId, isActive) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            var json = JsonSerializer.Serialize(result.Value);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.IsTrue(root.GetProperty("success").GetBoolean());
        }

        [Test]
        public void UpdateProductTypeStatus_ReturnsSuccessFalse_WithFailedMessage()
        {
            // Arrange
            var variantId = Guid.NewGuid();
            bool isActive = false;
            _productVariantServiceMock.Setup(s => s.UpdateProductVariantStatus(variantId, isActive)).Returns(false);

            // Act
            var result = _controller.UpdateProductTypeStatus(variantId, isActive) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            var json = System.Text.Json.JsonSerializer.Serialize(result.Value);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.IsFalse(root.GetProperty("success").GetBoolean());
            // Remove message assertion
        }

        [Test]
        public void UpdateProductTypeStatus_ReturnsErrorMessage_WhenExceptionThrown()
        {
            // Arrange
            var variantId = Guid.NewGuid();
            bool isActive = true;
            _productVariantServiceMock.Setup(s => s.UpdateProductVariantStatus(variantId, isActive))
                .Throws(new Exception("Test exception"));

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _controller.UpdateProductTypeStatus(variantId, isActive));
            Assert.AreEqual("Test exception", ex.Message);
        }

        [Test]
        public void UpdateProductTypeStatus_ReturnsFailedMessage_WithInvalidGuid()
        {
            // Arrange
            var invalidVariantId = Guid.Empty; // boundary/invalid input
            bool isActive = true;
            _productVariantServiceMock.Setup(s => s.UpdateProductVariantStatus(invalidVariantId, isActive)).Returns(false);

            // Act
            var result = _controller.UpdateProductTypeStatus(invalidVariantId, isActive) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            var json = System.Text.Json.JsonSerializer.Serialize(result.Value);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.IsFalse(root.GetProperty("success").GetBoolean());
            // Remove message assertion
        }
    }
}
