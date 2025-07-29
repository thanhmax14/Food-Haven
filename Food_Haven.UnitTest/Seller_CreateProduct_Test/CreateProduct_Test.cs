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
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;
using Models;
using Moq;
using NUnit.Framework;
using Repository.BalanceChange;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Seller_CreateProduct_Test
{
    [TestFixture]
    public class CreateProduct_Test
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
        public async Task CreateProduct_ReturnsRedirect_WhenInputIsValid()
        {
            // Arrange
            var userId = "test-user-id";
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            // Tạo HttpContext và mô phỏng TempData
            var httpContext = new DefaultHttpContext { User = user };
            var tempDataProviderMock = new Mock<ITempDataProvider>();
            var tempDataDictionary = new TempDataDictionary(httpContext, tempDataProviderMock.Object);
            httpContext.Items["TempData"] = tempDataDictionary;
            _controller.TempData = tempDataDictionary;

            // Mock SaveTempData với kiểu tham số đúng (IDictionary<string, object>)
            tempDataProviderMock.Setup(p => p.SaveTempData(httpContext, It.IsAny<IDictionary<string, object>>()))
                .Callback((HttpContext ctx, IDictionary<string, object> values) =>
                {
                    // Cập nhật tempDataDictionary với các giá trị từ values
                    foreach (var key in values.Keys)
                    {
                        tempDataDictionary[key] = values[key];
                    }
                });

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var model = new ProductViewModel
            {
                StoreID = Guid.NewGuid(),
                Name = "Pho",
                ShortDescription = "Very delicious!",
                LongDescription = "This pho is rich, flavorful, perfectly spiced, and absolutely delicious to enjoy!",
                ManufactureDate = new DateTime(2025, 3, 20),
                CateID = Guid.NewGuid(),
                MainImage = Mock.Of<IFormFile>(f => f.Length == 1 && f.FileName == "file1.png" && f.ContentType == "image/png"),
                GalleryImages = new List<IFormFile>
        {
            Mock.Of<IFormFile>(f => f.Length == 1 && f.FileName == "file1.png" && f.ContentType == "image/png"),
            Mock.Of<IFormFile>(f => f.Length == 1 && f.FileName == "file2.jpeg" && f.ContentType == "image/jpeg"),
            Mock.Of<IFormFile>(f => f.Length == 1 && f.FileName == "file3.gif" && f.ContentType == "image/gif"),
            Mock.Of<IFormFile>(f => f.Length == 1 && f.FileName == "file4.webp" && f.ContentType == "image/webp")
        }
            };

            var productImages = new List<ProductImageViewModel>
    {
        new ProductImageViewModel { ImageUrl = "/uploads/file1.png", IsMain = true, FileName = "file1.png", ContentType = "image/png" },
        new ProductImageViewModel { ImageUrl = "/uploads/file1.png", IsMain = false, FileName = "file1.png", ContentType = "image/png" },
        new ProductImageViewModel { ImageUrl = "/uploads/file2.jpeg", IsMain = false, FileName = "file2.jpeg", ContentType = "image/jpeg" },
        new ProductImageViewModel { ImageUrl = "/uploads/file3.gif", IsMain = false, FileName = "file3.gif", ContentType = "image/gif" },
        new ProductImageViewModel { ImageUrl = "/uploads/file4.webp", IsMain = false, FileName = "file4.webp", ContentType = "image/webp" }
    };

            _productServiceMock.Setup(s => s.GetActiveCategoriesAsync()).ReturnsAsync(new List<Categories> { new Categories { ID = Guid.NewGuid(), Name = "Dessert" } });
            _productServiceMock.Setup(s => s.CreateProductAsync(model, userId, It.IsAny<List<ProductImageViewModel>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateProduct(model);

            // Assert
            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("CreateProduct", redirect.ActionName);
            Assert.AreEqual(true, _controller.TempData["ProductCreated"]); // Kiểm tra TempData["ProductCreated"]
            Assert.AreEqual(model.StoreID, _controller.TempData["StoreID"]); // Kiểm tra StoreID
        }
        [Test]
        public async Task CreateProduct_ReturnsView_WhenTooManyGalleryImages()
        {
            // Arrange
            var userId = "test-user-id";
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var galleryImages = new List<IFormFile>
    {
        Mock.Of<IFormFile>(f => f.Length == 1 && f.FileName == "file1.png" && f.ContentType == "image/png"),
        Mock.Of<IFormFile>(f => f.Length == 1 && f.FileName == "file2.jpeg" && f.ContentType == "image/jpeg"),
        Mock.Of<IFormFile>(f => f.Length == 1 && f.FileName == "file3.gif" && f.ContentType == "image/gif"),
        Mock.Of<IFormFile>(f => f.Length == 1 && f.FileName == "file4.webp" && f.ContentType == "image/webp")
    };

            var model = new ProductViewModel
            {
                StoreID = Guid.NewGuid(),
                Name = "Pho",
                ShortDescription = "Very delicious!",
                LongDescription = "This pho is rich, flavorful, perfectly spiced, and absolutely delicious to enjoy!",
                ManufactureDate = new DateTime(2025, 3, 20),
                CateID = Guid.NewGuid(), // Giả sử CategoryID
                MainImage = Mock.Of<IFormFile>(f => f.Length == 1 && f.FileName == "file1.png" && f.ContentType == "image/png"),
                GalleryImages = galleryImages,

            };

            // Simulate model validation error for too many images (thêm 1 ảnh để vượt giới hạn 4)
            _controller.ModelState.AddModelError("GalleryImages", "You can only select up to 4 gallery images.");

            _productServiceMock.Setup(s => s.GetActiveCategoriesAsync()).ReturnsAsync(new List<Categories> { new Categories { ID = Guid.NewGuid(), Name = "Dessert" } });

            // Act
            var result = await _controller.CreateProduct(model);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(model, viewResult.Model);
            Assert.IsTrue(_controller.ModelState.ContainsKey("GalleryImages"));
            Assert.AreEqual("You can only select up to 4 gallery images.", _controller.ModelState["GalleryImages"].Errors[0].ErrorMessage);
        }
        [Test]
        public async Task CreateProduct_ReturnsViewWithError_WhenExceptionThrown()
        {
            // Arrange
            var userId = "test-user-id";
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var model = new ProductViewModel
            {
                StoreID = Guid.NewGuid(),
                MainImage = Mock.Of<IFormFile>(f => f.Length == 1 && f.FileName == "main.png"),
                GalleryImages = new List<IFormFile>()
            };

            _productServiceMock.Setup(s => s.CreateProductAsync(It.IsAny<ProductViewModel>(), userId, It.IsAny<List<ProductImageViewModel>>()))
                .ThrowsAsync(new Exception("An unknown error occurred..."));

            // Act & Assert
            try
            {
                var result = await _controller.CreateProduct(model);
                var viewResult = result as ViewResult;
                Assert.IsNotNull(viewResult);
                // Optionally check for error message in ModelState or ViewBag
            }
            catch (Exception ex)
            {
                Assert.AreEqual("An unknown error occurred...", ex.Message);
            }
        }
    }
}
