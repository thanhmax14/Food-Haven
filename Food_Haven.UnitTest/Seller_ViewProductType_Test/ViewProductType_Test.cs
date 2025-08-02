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
using Models;
using Moq;
using NUnit.Framework;
using Repository.BalanceChange;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Food_Haven.UnitTest.Seller_ViewProductType_Test
{
    [TestFixture]
    public class ViewProductType_Test
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

        private void SetUser(string userId = "test-user-id")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        // TC01: Normal - Returns list of product types
        [Test]
        public async Task ViewProductType_ReturnsList_WhenProductTypeExists()
        {
            SetUser(); // <-- Add this line
            var productId = Guid.Parse("b06d9378-8bb3-4dea-9a74-146f42f7bdf6");
            var productTypes = new List<ProductVariantViewModel>
            {
                new ProductVariantViewModel { ID = Guid.NewGuid(), ProductID = productId, StoreID = Guid.NewGuid() }
            };
            _productVariantServiceMock.Setup(s => s.GetProductTypeByProductIdAsync(productId)).ReturnsAsync(productTypes);
            _productServiceMock.Setup(s => s.IsStoreActiveByProductIdAsync(productId)).ReturnsAsync(true);
            _storeDetailServiceMock.Setup(s => s.GetStoreByUserId(It.IsAny<string>())).Returns(new StoreDetails { Status = "Active" });

            var result = await _controller.ViewProductType(productId);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOf<List<ProductVariantViewModel>>(viewResult.Model);
            var model = (List<ProductVariantViewModel>)viewResult.Model;
            Assert.IsNotEmpty(model);
        }

        // TC02: Abnormal - Returns empty list when no product type
        [Test]
        public async Task ViewProductType_ReturnsEmptyList_WhenNoProductType()
        {
            SetUser(); // <-- Add this line
            var productId = Guid.Parse("fd2f0811-a365-47bb-b232-68c10248f1ff");
            _productVariantServiceMock.Setup(s => s.GetProductTypeByProductIdAsync(productId)).ReturnsAsync(new List<ProductVariantViewModel>());
            _productServiceMock.Setup(s => s.IsStoreActiveByProductIdAsync(productId)).ReturnsAsync(false);
            _storeDetailServiceMock.Setup(s => s.GetStoreByUserId(It.IsAny<string>())).Returns((StoreDetails)null);

            var result = await _controller.ViewProductType(productId);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOf<List<ProductVariantViewModel>>(viewResult.Model);
            var model = (List<ProductVariantViewModel>)viewResult.Model;
            Assert.IsEmpty(model);
        }

        // TC03: Exception - Service throws, controller should propagate or handle
        [Test]
        public void ViewProductType_ThrowsException_WhenServiceFails()
        {
            SetUser(); // <-- Add this line
            var productId = Guid.NewGuid();
            _productVariantServiceMock.Setup(s => s.GetProductTypeByProductIdAsync(productId)).ThrowsAsync(new Exception("Unknown error"));

            Assert.ThrowsAsync<Exception>(async () =>
            {
                await _controller.ViewProductType(productId);
            });
        }
    }
}
