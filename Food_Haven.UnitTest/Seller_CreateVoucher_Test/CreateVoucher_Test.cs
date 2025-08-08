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
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Seller_CreateVoucher_Test
{
    public class CreateVoucher_Test
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
        public async Task CreateVoucher_ValidData_ReturnsSuccessJson()
        {
            var user = new AppUser { Id = "user-id-123", UserName = "Shang12345" };
            var store = new StoreDetails { ID = Guid.NewGuid(), UserID = user.Id };
            var voucherViewModel = new VoucherViewModel
            {
                Code = "SALE10",
                DiscountAmount = 10,
                DiscountType = "Percent",
                StartDate = DateTime.Today.ToString("yyyy-MM-dd"),
                ExpirationDate = DateTime.Today.AddDays(5).ToString("yyyy-MM-dd"),
                MaxUsage = 100,
                CurrentUsage = 0,
                MinOrderValue = 50000,
                Scope = "50",
                IsActive = true
            };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailService2Mock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StoreDetails, bool>>>())).ReturnsAsync(store);
            _voucherServiceMock.Setup(v => v.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Voucher, bool>>>())).ReturnsAsync((Voucher)null);
            _voucherServiceMock.Setup(v => v.AddAsync(It.IsAny<Voucher>())).Returns(Task.CompletedTask);
            _voucherServiceMock.Setup(v => v.SaveChangesAsync()).ReturnsAsync(1);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            httpContext.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id)
            }));
            _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = httpContext };
            var result = await _controller.CreateVoucher(voucherViewModel);
            Assert.IsInstanceOf<JsonResult>(result);
            var json = (JsonResult)result;
            var dict = json.Value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(json.Value));
            Assert.IsTrue(dict.ContainsKey("success"));
            Assert.AreEqual(true, dict["success"]);
        }

        [Test]
        public async Task CreateVoucher_InvalidCode_ReturnsFieldError()
        {
            var user = new AppUser { Id = "user-id-123", UserName = "Shang12345" };
            var store = new StoreDetails { ID = Guid.NewGuid(), UserID = user.Id };
            var voucherViewModel = new VoucherViewModel
            {
                Code = "",
                DiscountAmount = 10,
                DiscountType = "Percent",
                StartDate = DateTime.Today.ToString("yyyy-MM-dd"),
                ExpirationDate = DateTime.Today.AddDays(5).ToString("yyyy-MM-dd"),
                MaxUsage = 100,
                CurrentUsage = 0,
                MinOrderValue = 50000,
                Scope = "50",
                IsActive = true
            };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailService2Mock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StoreDetails, bool>>>())).ReturnsAsync(store);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            httpContext.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id)
            }));
            _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = httpContext };
            var result = await _controller.CreateVoucher(voucherViewModel);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = (BadRequestObjectResult)result;
            var dict = badRequest.Value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(badRequest.Value));
            Assert.IsFalse((bool)dict["success"]);
            Assert.IsTrue(dict.ContainsKey("fieldErrors"));
            var fieldErrors = dict["fieldErrors"] as IDictionary<string, string>;
            Assert.IsTrue(fieldErrors.ContainsKey("code"));
        }

        [Test]
        public async Task CreateVoucher_DiscountAmountNegative_ReturnsFieldError()
        {
            var user = new AppUser { Id = "user-id-123", UserName = "Shang12345" };
            var store = new StoreDetails { ID = Guid.NewGuid(), UserID = user.Id };
            var voucherViewModel = new VoucherViewModel
            {
                Code = "SALE10",
                DiscountAmount = -10,
                DiscountType = "Percent",
                StartDate = DateTime.Today.ToString("yyyy-MM-dd"),
                ExpirationDate = DateTime.Today.AddDays(5).ToString("yyyy-MM-dd"),
                MaxUsage = 100,
                CurrentUsage = 0,
                MinOrderValue = 50000,
                Scope = "50",
                IsActive = true
            };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailService2Mock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StoreDetails, bool>>>())).ReturnsAsync(store);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            httpContext.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id)
            }));
            _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = httpContext };
            var result = await _controller.CreateVoucher(voucherViewModel);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = (BadRequestObjectResult)result;
            var dict = badRequest.Value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(badRequest.Value));
            Assert.IsFalse((bool)dict["success"]);
            var fieldErrors = dict["fieldErrors"] as IDictionary<string, string>;
            Assert.IsTrue(fieldErrors.ContainsKey("discountAmount"));
        }

        [Test]
        public async Task CreateVoucher_MaxUsageNegative_ReturnsFieldError()
        {
            var user = new AppUser { Id = "user-id-123", UserName = "Shang12345" };
            var store = new StoreDetails { ID = Guid.NewGuid(), UserID = user.Id };
            var voucherViewModel = new VoucherViewModel
            {
                Code = "SALE10",
                DiscountAmount = 10,
                DiscountType = "Percent",
                StartDate = DateTime.Today.ToString("yyyy-MM-dd"),
                ExpirationDate = DateTime.Today.AddDays(5).ToString("yyyy-MM-dd"),
                MaxUsage = -9,
                CurrentUsage = 0,
                MinOrderValue = 50000,
                Scope = "50",
                IsActive = true
            };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailService2Mock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StoreDetails, bool>>>())).ReturnsAsync(store);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            httpContext.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id)
            }));
            _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = httpContext };
            var result = await _controller.CreateVoucher(voucherViewModel);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = (BadRequestObjectResult)result;
            var dict = badRequest.Value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(badRequest.Value));
            Assert.IsFalse((bool)dict["success"]);
            var fieldErrors = dict["fieldErrors"] as IDictionary<string, string>;
            Assert.IsTrue(fieldErrors.ContainsKey("maxUsage"));
        }

        [Test]
        public async Task CreateVoucher_CurrentUsageNegative_ReturnsFieldError()
        {
            var user = new AppUser { Id = "user-id-123", UserName = "Shang12345" };
            var store = new StoreDetails { ID = Guid.NewGuid(), UserID = user.Id };
            var voucherViewModel = new VoucherViewModel
            {
                Code = "SALE10",
                DiscountAmount = 10,
                DiscountType = "Percent",
                StartDate = DateTime.Today.ToString("yyyy-MM-dd"),
                ExpirationDate = DateTime.Today.AddDays(5).ToString("yyyy-MM-dd"),
                MaxUsage = 100,
                CurrentUsage = -100,
                MinOrderValue = 50000,
                Scope = "50",
                IsActive = true
            };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailService2Mock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StoreDetails, bool>>>())).ReturnsAsync(store);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            httpContext.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id)
            }));
            _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = httpContext };
            var result = await _controller.CreateVoucher(voucherViewModel);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = (BadRequestObjectResult)result;
            var dict = badRequest.Value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(badRequest.Value));
            Assert.IsFalse((bool)dict["success"]);
            var fieldErrors = dict["fieldErrors"] as IDictionary<string, string>;
            Assert.IsTrue(fieldErrors.ContainsKey("currentUsage"));
        }

        [Test]
        public async Task CreateVoucher_CurrentUsageExceedsMaxUsage_ReturnsFieldError()
        {
            var user = new AppUser { Id = "user-id-123", UserName = "Shang12345" };
            var store = new StoreDetails { ID = Guid.NewGuid(), UserID = user.Id };
            var voucherViewModel = new VoucherViewModel
            {
                Code = "SALE10",
                DiscountAmount = 10,
                DiscountType = "Percent",
                StartDate = DateTime.Today.ToString("yyyy-MM-dd"),
                ExpirationDate = DateTime.Today.AddDays(5).ToString("yyyy-MM-dd"),
                MaxUsage = 90,
                CurrentUsage = 100,
                MinOrderValue = 50000,
                Scope = "50",
                IsActive = true
            };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailService2Mock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StoreDetails, bool>>>())).ReturnsAsync(store);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            httpContext.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id)
            }));
            _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = httpContext };
            var result = await _controller.CreateVoucher(voucherViewModel);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = (BadRequestObjectResult)result;
            var dict = badRequest.Value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(badRequest.Value));
            Assert.IsFalse((bool)dict["success"]);
            var fieldErrors = dict["fieldErrors"] as IDictionary<string, string>;
            Assert.IsTrue(fieldErrors.ContainsKey("currentUsage"));
        }

        [Test]
        public async Task CreateVoucher_StartDateAfterExpirationDate_ReturnsFieldError()
        {
            var user = new AppUser { Id = "user-id-123", UserName = "Shang12345" };
            var store = new StoreDetails { ID = Guid.NewGuid(), UserID = user.Id };
            var voucherViewModel = new VoucherViewModel
            {
                Code = "SALE10",
                DiscountAmount = 10,
                DiscountType = "Percent",
                StartDate = "2025-08-05",
                ExpirationDate = "2025-08-03",
                MaxUsage = 100,
                CurrentUsage = 0,
                MinOrderValue = 50000,
                Scope = "50",
                IsActive = true
            };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailService2Mock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StoreDetails, bool>>>())).ReturnsAsync(store);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            httpContext.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id)
            }));
            _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = httpContext };
            var result = await _controller.CreateVoucher(voucherViewModel);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = (BadRequestObjectResult)result;
            var dict = badRequest.Value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(badRequest.Value));
            Assert.IsFalse((bool)dict["success"]);
            var fieldErrors = dict["fieldErrors"] as IDictionary<string, string>;
            Assert.IsTrue(fieldErrors.ContainsKey("startDate"));
        }

        [Test]
        public async Task CreateVoucher_MinOrderValueNegative_ReturnsFieldError()
        {
            var user = new AppUser { Id = "user-id-123", UserName = "Shang12345" };
            var store = new StoreDetails { ID = Guid.NewGuid(), UserID = user.Id };
            var voucherViewModel = new VoucherViewModel
            {
                Code = "SALE10",
                DiscountAmount = 10,
                DiscountType = "Percent",
                StartDate = DateTime.Today.ToString("yyyy-MM-dd"),
                ExpirationDate = DateTime.Today.AddDays(5).ToString("yyyy-MM-dd"),
                MaxUsage = 100,
                CurrentUsage = 0,
                MinOrderValue = -10,
                Scope = "50",
                IsActive = true
            };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailService2Mock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StoreDetails, bool>>>())).ReturnsAsync(store);
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            httpContext.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id)
            }));
            _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = httpContext };
            var result = await _controller.CreateVoucher(voucherViewModel);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = (BadRequestObjectResult)result;
            var dict = badRequest.Value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(badRequest.Value));
            Assert.IsFalse((bool)dict["success"]);
            var fieldErrors = dict["fieldErrors"] as IDictionary<string, string>;
            Assert.IsTrue(fieldErrors.ContainsKey("minOrderValue"));
        }
    }
}
