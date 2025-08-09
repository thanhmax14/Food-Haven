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
using System.Threading.Tasks;
using IFormFile = Microsoft.AspNetCore.Http.IFormFile;

namespace Food_Haven.UnitTest.Seller_UpdateProduct_Test
{
    [TestFixture]
    public class UpdateProduct_Test
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
        public async Task UpdateProduct_Normal_Success_ReturnsViewWithSuccessMessage()
        {
            // Arrange
            var model = new ProductUpdateViewModel
            {
                ProductID = Guid.NewGuid(),
                Name = "Pho",
                ShortDescription = "Very delicious!",
                LongDescription = "This pho is rich, flavorful, perfectly spiced, and absolutely delicious to enjoy.",
                ManufactureDate = DateTime.Parse("2025-03-20"),
                ExistingImages = new List<string> { "file1.png" },
                GalleryImages = new List<IFormFile>(), // Use empty or mock IFormFile objects here
                StoreID = Guid.NewGuid()
            };

            //_productServiceMock.Setup(s => s.UpdateProductAsync(model, It.IsAny<string>())).Returns(Task.CompletedTask);
            _productServiceMock.Setup(s => s.UpdateProductAsync(It.IsAny<ProductUpdateViewModel>(), It.IsAny<string>()))
                .ReturnsAsync((true, null));
            _productServiceMock.Setup(s => s.GetImageUrlsByProductIdAsync(model.ProductID)).ReturnsAsync(model.ExistingImages);
            _productServiceMock.Setup(s => s.GetActiveCategoriesAsync()).ReturnsAsync(new List<Categories>());
            _webHostEnvironmentMock.Setup(e => e.WebRootPath).Returns("wwwroot");

            // Act
            var result = await _controller.UpdateProduct(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewData["UpdateSuccess"] as bool?);
            Assert.AreEqual("Pho", ((ProductUpdateViewModel)result.Model).Name);
        }

        [Test]
        public async Task UpdateProduct_GalleryImageBoundary_ReturnsViewWithBoundaryMessage()
        {
            // Arrange
            var model = new ProductUpdateViewModel
            {
                ProductID = Guid.NewGuid(),
                GalleryImages = new List<IFormFile>(), // Use empty or mock IFormFile objects here
                StoreID = Guid.NewGuid()
            };

            _productServiceMock.Setup(s => s.GetImageUrlsByProductIdAsync(model.ProductID)).ReturnsAsync(new List<string>());
            _productServiceMock.Setup(s => s.GetActiveCategoriesAsync()).ReturnsAsync(new List<Categories>());

            _controller.ModelState.AddModelError("GalleryImages", "You can only select up to 4 gallery images.");

            // Act
            var result = await _controller.UpdateProduct(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var updateSuccess = result.ViewData["UpdateSuccess"] as bool?;
            Assert.IsTrue(updateSuccess == null || updateSuccess == false, "UpdateSuccess should be false or null when ModelState is invalid.");
            Assert.IsTrue(_controller.ModelState.ContainsKey("GalleryImages"));
        }

        [Test]
        public async Task UpdateProduct_ExceptionThrown_ReturnsViewWithErrorMessage()
        {
            // Arrange
            var model = new ProductUpdateViewModel
            {
                ProductID = Guid.NewGuid(),
                StoreID = Guid.NewGuid()
            };

            _productServiceMock.Setup(s => s.UpdateProductAsync(model, It.IsAny<string>())).ThrowsAsync(new Exception("An unknown error occurred..."));
            _productServiceMock.Setup(s => s.GetImageUrlsByProductIdAsync(model.ProductID)).ReturnsAsync(new List<string>());
            _productServiceMock.Setup(s => s.GetActiveCategoriesAsync()).ReturnsAsync(new List<Categories>());
            _webHostEnvironmentMock.Setup(e => e.WebRootPath).Returns("wwwroot");

            // Act
            var result = await _controller.UpdateProduct(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.ViewData["UpdateSuccess"] as bool?);
            Assert.IsTrue(_controller.ModelState[string.Empty].Errors[0].ErrorMessage.Contains("An unknown error occurred"));
        }
    }
}
