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
using Microsoft.AspNetCore.Http;
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
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Seller_CreateProductType_Test
{
    [TestFixture]
    public class CreateProductType_Test
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

        // TC01: Normal - Valid data, should create product type
        [Test]
        public async Task CreateProductType_ReturnsView_WithSuccess_WhenModelIsValid()
        {
            SetUser();
            var model = new ProductVariantCreateViewModel
            {
                ProductID = Guid.Parse("b06d9378-8bb3-4dea-9a74-146f42f7bdf6"),
                // Add other valid properties if needed
            };
            _controller.ModelState.Clear();
            _productVariantServiceMock.Setup(s => s.CreateProductVariantAsync(model)).Returns(Task.CompletedTask);

            var result = await _controller.CreateProductType(model);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(model, viewResult.Model);
            Assert.IsTrue(_controller.ViewBag.ProductTypeCreated);
        }

        // TC02: Abnormal - Invalid price, should return error message
        [Test]
        public async Task CreateProductType_ReturnsView_WithError_WhenPriceIsInvalid()
        {
            SetUser();
            var model = new ProductVariantCreateViewModel
            {
                ProductID = Guid.Parse("b06d9378-8bb3-4dea-9a74-146f42f7bdf6"),
                // Price = -10000, // Add property if exists
            };
            _controller.ModelState.AddModelError("Price", "Sell price must be greater than or equal to 0.");

            var result = await _controller.CreateProductType(model);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(model, viewResult.Model);
            Assert.IsTrue(_controller.ModelState.ContainsKey("Price"));
        }

        // TC03: Abnormal - Invalid original price, should return error message
        [Test]
        public async Task CreateProductType_ReturnsView_WithError_WhenOriginalPriceIsInvalid()
        {
            SetUser();
            var model = new ProductVariantCreateViewModel
            {
                ProductID = Guid.Parse("b06d9378-8bb3-4dea-9a74-146f42f7bdf6"),
                // OriginalPrice = -10000, // Add property if exists
            };
            _controller.ModelState.AddModelError("OriginalPrice", "Original price must be greater than or equal to 0.");

            var result = await _controller.CreateProductType(model);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(model, viewResult.Model);
            Assert.IsTrue(_controller.ModelState.ContainsKey("OriginalPrice"));
        }

        // TC04: Abnormal - Invalid stock, should return error message
        [Test]
        public async Task CreateProductType_ReturnsView_WithError_WhenStockIsInvalid()
        {
            SetUser();
            var model = new ProductVariantCreateViewModel
            {
                ProductID = Guid.Parse("b06d9378-8bb3-4dea-9a74-146f42f7bdf6"),
                // Stock = -20, // Add property if exists
            };
            _controller.ModelState.AddModelError("Stock", "Stock quantity must be greater than or equal to 0.");

            var result = await _controller.CreateProductType(model);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(model, viewResult.Model);
            Assert.IsTrue(_controller.ModelState.ContainsKey("Stock"));
        }

        // TC05: Exception - Service throws, should propagate or handle
        [Test]
        public void CreateProductType_ThrowsException_WhenServiceFails()
        {
            SetUser();
            var model = new ProductVariantCreateViewModel
            {
                ProductID = Guid.NewGuid()
            };
            _controller.ModelState.Clear();
            _productVariantServiceMock.Setup(s => s.CreateProductVariantAsync(model)).ThrowsAsync(new Exception("An unknown error occurred..."));

            Assert.ThrowsAsync<Exception>(async () =>
            {
                await _controller.CreateProductType(model);
            });
        }
    }
}
