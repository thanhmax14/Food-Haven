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
using Repository.BalanceChange;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Seller_ViewProductList_Test
{
    public class ViewProductList_Test
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
        public async Task ViewProductList_ReturnsViewWithProducts_WhenConnectionSucceeds()
        {
            // Arrange
            var userId = "8e91c798-bc78-46a9-89a4-5d0aaea77f5f";
            var products = new List<Product>
    {
        new Product { ID = Guid.NewGuid(), StoreID = Guid.NewGuid(), Name = "Product 1" }
    };
            // Ánh xạ từ Product sang ProductIndexViewModel
            var productViewModels = products.Select(p => new ProductIndexViewModel
            {
                ProductId = p.ID,
                StoreId = p.StoreID,
                Name = p.Name
            }).ToList();
            var store = new StoreDetails { ID = Guid.NewGuid(), IsActive = true, Status = "Active" };

            // Thiết lập mock để trả về ProductIndexViewModel
            _productServiceMock.Setup(s => s.GetProductsByCurrentUser(userId)).Returns(productViewModels);
            _storeDetailServiceMock.Setup(s => s.GetStoreByUserId(userId)).Returns(store);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = _controller.ViewProductList(); // Không async vì phương thức không sử dụng await

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(productViewModels, viewResult.Model); // So sánh với ProductIndexViewModel
            Assert.AreEqual(products.First().StoreID, _controller.ViewBag.StoreId); // So sánh với StoreID từ Product
            Assert.AreEqual(true, _controller.ViewBag.StoreStatus);
            Assert.AreEqual("Active", _controller.ViewBag.StoreStatusText);
        }
        [Test]
        public async Task ViewProductList_ReturnsViewWithEmptyList_WhenConnectionFails()
        {
            // Arrange
            var userId = "d2953e19-3568-4a27-92cf-109000a8383c";
            var products = new List<Product>(); // Danh sách rỗng Product
                                                // Tạo danh sách rỗng ProductIndexViewModel
            var productViewModels = new List<ProductIndexViewModel>(); // Danh sách rỗng cho view model
            var store = new StoreDetails { ID = Guid.NewGuid(), IsActive = false, Status = "Pending" };

            // Thiết lập mock để trả về ProductIndexViewModel
            _productServiceMock.Setup(s => s.GetProductsByCurrentUser(userId)).Returns(productViewModels);
            _storeDetailServiceMock.Setup(s => s.GetStoreByUserId(userId)).Returns(store);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = _controller.ViewProductList(); // Không async vì phương thức không sử dụng await

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(productViewModels, viewResult.Model); // So sánh với ProductIndexViewModel
            Assert.AreEqual(Guid.Empty, _controller.ViewBag.StoreId); // Vì products rỗng
            Assert.AreEqual(false, _controller.ViewBag.StoreStatus);
            Assert.AreEqual("Pending", _controller.ViewBag.StoreStatusText);
            // Thêm kiểm tra lỗi nếu controller đặt ViewBag.Message
            // Assert.AreEqual("An unknown error occurred...", _controller.ViewBag.Message); // Nếu có
        }
        [Test]
        public async Task ViewProductList_ThrowsException_WhenServiceFails()
        {
            // Arrange
            var userId = "b2953e19-3568-4a27-92cf-109000a8383c";

            _productServiceMock.Setup(s => s.GetProductsByCurrentUser(userId)).Throws(new Exception("Simulated exception"));
            _storeDetailServiceMock.Setup(s => s.GetStoreByUserId(userId)).Returns((StoreDetails)null);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _controller.ViewProductList());
            Assert.AreEqual("Simulated exception", exception.Message);
        }
    }
}
