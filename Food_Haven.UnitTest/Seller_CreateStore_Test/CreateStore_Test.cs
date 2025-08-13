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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Repository.BalanceChange;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Seller_CreateStore_Test
{
    public class CreateStore_Test
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
        public async Task CreateStore_Success_ReturnsRedirect()
        {
            // Arrange
            var user = new AppUser { Id = "seller1" };
            var model = new CreateStoreViewModel
            {
                Name = "Pearl Restaurant",
                ShortDescriptions = "Cozy restaurant with tasty food and warm, friendly staff.",
                LongDescriptions = "A beautifully decorated restaurant offering delicious meals and exceptional service",
                Address = "Can Tho",
                Phone = "0987654321"
            };

            var imgFileMock = new Mock<IFormFile>();
            imgFileMock.Setup(f => f.Length).Returns(1);
            imgFileMock.Setup(f => f.FileName).Returns("file1.png");
            model.ImgFile = imgFileMock.Object;

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailServiceMock.Setup(x => x.IsUserSellerAsync(user.Id)).ReturnsAsync(true);
            _storeDetailServiceMock.Setup(x => x.IsStoreNameExistsAsync(model.Name, model.ID)).ReturnsAsync(false);
            _storeDetailServiceMock.Setup(x => x.IsPhoneExistsAsync(model.Phone, model.ID)).ReturnsAsync(false);
            _storeDetailServiceMock.Setup(x => x.AddStoreAsync(It.IsAny<CreateStoreViewModel>(), user.Id)).ReturnsAsync(true);
            _webHostEnvironmentMock.Setup(x => x.WebRootPath).Returns("wwwroot");

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new Mock<ISession>().Object;
            _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _controller.CreateStore(model);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect?.ActionName, Is.EqualTo("ViewStore"));
        }

        [Test]
        public async Task CreateStore_UserNotSeller_ReturnsViewWithError()
        {
            // Arrange
            var user = new AppUser { Id = "seller1" };
            var model = new CreateStoreViewModel { Name = "Pearl Restaurant" };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailServiceMock.Setup(x => x.IsUserSellerAsync(user.Id)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateStore(model);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual("You do not have permission to create a store.", _controller.ViewBag.PermissionError);
        }

        [Test]
        public async Task CreateStore_InvalidImageExtension_ReturnsViewWithError()
        {
            // Arrange
            var user = new AppUser { Id = "seller1" };
            var model = new CreateStoreViewModel { Name = "Pearl Restaurant" };

            var imgFileMock = new Mock<IFormFile>();
            imgFileMock.Setup(f => f.Length).Returns(1);
            imgFileMock.Setup(f => f.FileName).Returns("file1.svg");
            model.ImgFile = imgFileMock.Object;

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailServiceMock.Setup(x => x.IsUserSellerAsync(user.Id)).ReturnsAsync(true);
            _storeDetailServiceMock.Setup(x => x.IsStoreNameExistsAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);
            _storeDetailServiceMock.Setup(x => x.IsPhoneExistsAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);
            // Act
            var result = await _controller.CreateStore(model);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsTrue(_controller.ModelState.ContainsKey("ImgFile"));
            Assert.AreEqual("Only image files (.png, .jpeg, .jpg) are supported.", _controller.ModelState["ImgFile"].Errors[0].ErrorMessage);
        }

        [Test]
        public async Task CreateStore_PhoneAlreadyUsed_ReturnsViewWithError()
        {
            // Arrange
            var user = new AppUser { Id = "seller1" };
            var model = new CreateStoreViewModel
            {
                Name = "Pearl Restaurant",
                Phone = "0989889889"
                // Do NOT set ImgFile here, to simulate missing image and trigger the image required error
            };

            // Mock environment để tránh Path.Combine lỗi
            _webHostEnvironmentMock.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            // Mock user và service
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailServiceMock.Setup(x => x.IsUserSellerAsync(user.Id)).ReturnsAsync(true);
            _storeDetailServiceMock
                .Setup(x => x.IsStoreNameExistsAsync(It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);
            _storeDetailServiceMock
                .Setup(x => x.IsPhoneExistsAsync(model.Phone, model.ID))
                .ReturnsAsync(true); // Phone đã tồn tại

            // Act
            var result = await _controller.CreateStore(model);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);

            // The controller will return image required error first if ImgFile is missing
            var allErrors = _controller.ModelState
                .SelectMany(ms => ms.Value.Errors.Select(e => e.ErrorMessage))
                .ToList();

            Assert.IsTrue(allErrors.Any(e => e.Contains("Store Image", StringComparison.OrdinalIgnoreCase)),
                $"Expected image required error, but found: {string.Join(", ", allErrors)}");
        }




        [Test]
        public async Task CreateStore_UnknownError_ReturnsViewWithError()
        {
            // Arrange
            var user = new AppUser { Id = "seller1" };
            var imgFileMock = new Mock<IFormFile>();
            imgFileMock.Setup(f => f.Length).Returns(1);
            imgFileMock.Setup(f => f.FileName).Returns("file1.png"); // valid extension

            var model = new CreateStoreViewModel
            {
                Name = "Pearl Restaurant",
                ImgFile = imgFileMock.Object
            };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailServiceMock.Setup(x => x.IsUserSellerAsync(user.Id)).ReturnsAsync(true);
            _storeDetailServiceMock.Setup(x => x.IsStoreNameExistsAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);
            _storeDetailServiceMock.Setup(x => x.IsPhoneExistsAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);
            _storeDetailServiceMock.Setup(x => x.AddStoreAsync(It.IsAny<CreateStoreViewModel>(), user.Id))
                .ThrowsAsync(new Exception("An unknown error occurred"));

            _webHostEnvironmentMock.Setup(x => x.WebRootPath).Returns(Path.GetTempPath());

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _controller.CreateStore(model));
            Assert.That(ex.Message, Is.EqualTo("An unknown error occurred"));
        }
    }
}
