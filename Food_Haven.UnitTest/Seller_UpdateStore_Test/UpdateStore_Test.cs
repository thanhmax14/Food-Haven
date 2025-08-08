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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
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

namespace Food_Haven.UnitTest.Seller_UpdateStore_Test
{
    public class UpdateStore_Test
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
        public async Task UpdateStore_SuccessfulUpdate_ReturnsRedirect()
        {
            // Arrange
            var user = new AppUser { Id = "seller1" };
            var storeId = Guid.NewGuid();
            var store = new StoreDetails { ID = storeId, ImageUrl = "/uploads/file1.png" };
            var model = new StoreViewModel
            {
                Name = "Pearl Restaurant",
                ShortDescriptions = "Cozy restaurant with tasty food and warm, friendly staff.",
                LongDescriptions = "A beautifully decorated restaurant offering delicious meals and exceptional service",
                Address = "Can Tho",
                Phone = "0987654321"
            };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailService2Mock.Setup(x => x.GetStoreByIdAsync(storeId)).ReturnsAsync(store);
            _storeDetailService2Mock.Setup(x => x.UpdateStoreAsync(storeId, model.Name, model.LongDescriptions, model.ShortDescriptions, model.Address, model.Phone, store.ImageUrl)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateStore(storeId, model, null);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = result as RedirectToActionResult;
            Assert.AreEqual("ViewStore", redirect.ActionName);
        }

        [Test]
        public async Task UpdateStore_StoreNotFound_ReturnsViewWithError()
        {
            // Arrange
            var user = new AppUser { Id = "seller1" };
            var storeId = Guid.NewGuid();
            var model = new StoreViewModel { Name = "Pearl Restaurant" };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailService2Mock.Setup(x => x.GetStoreByIdAsync(storeId)).ReturnsAsync((StoreDetails)null);

            // Act
            var result = await _controller.UpdateStore(storeId, model, null);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsTrue(_controller.ModelState.ContainsKey(""));
            Assert.AreEqual("No store found.", _controller.ModelState[""].Errors[0].ErrorMessage);
        }

        [Test]
        public async Task UpdateStore_InvalidImageExtension_ReturnsViewWithError()
        {
            // Arrange
            var user = new AppUser { Id = "seller1" };
            var storeId = Guid.NewGuid();
            var store = new StoreDetails { ID = storeId, ImageUrl = "/uploads/file1.png" };
            var model = new StoreViewModel { Name = "Pearl Restaurant" };
            var imgFileMock = new Mock<IFormFile>();
            imgFileMock.Setup(f => f.Length).Returns(1);
            imgFileMock.Setup(f => f.FileName).Returns("file1.svg");

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailService2Mock.Setup(x => x.GetStoreByIdAsync(storeId)).ReturnsAsync(store);

            // Act
            var result = await _controller.UpdateStore(storeId, model, imgFileMock.Object);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsTrue(_controller.ModelState.ContainsKey("Img"));
            Assert.AreEqual("Only image files (.png, .jpeg, .jpg) are supported.", _controller.ModelState["Img"].Errors[0].ErrorMessage);
        }

        [Test]
        public async Task UpdateStore_UpdateFails_ReturnsViewWithError()
        {
            // Arrange
            var user = new AppUser { Id = "seller1" };
            var storeId = Guid.NewGuid();
            var store = new StoreDetails { ID = storeId, ImageUrl = "/uploads/file1.png" };
            var model = new StoreViewModel { Name = "Pearl Restaurant" };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailService2Mock.Setup(x => x.GetStoreByIdAsync(storeId)).ReturnsAsync(store);
            _storeDetailService2Mock.Setup(x => x.UpdateStoreAsync(storeId, model.Name, model.LongDescriptions, model.ShortDescriptions, model.Address, model.Phone, store.ImageUrl)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateStore(storeId, model, null);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsTrue(_controller.ModelState.ContainsKey(""));
            Assert.AreEqual("Store update failed.", _controller.ModelState[""].Errors[0].ErrorMessage);
        }

        [Test]
        public async Task UpdateStore_ExceptionThrown_ReturnsException()
        {
            // Arrange
            var user = new AppUser { Id = "seller1" };
            var storeId = Guid.NewGuid();
            var store = new StoreDetails { ID = storeId, ImageUrl = "/uploads/file1.png" };
            var model = new StoreViewModel { Name = "Pearl Restaurant" };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailService2Mock.Setup(x => x.GetStoreByIdAsync(storeId)).ReturnsAsync(store);
            _storeDetailService2Mock.Setup(x => x.UpdateStoreAsync(storeId, model.Name, model.LongDescriptions, model.ShortDescriptions, model.Address, model.Phone, store.ImageUrl)).ThrowsAsync(new Exception("An unknown error occurred"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _controller.UpdateStore(storeId, model, null));
        }
    }
}
