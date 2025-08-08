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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Food_Haven.UnitTest.Seller_GetAllVoucher_Test
{
    public class GetAllVoucher_Test
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
        public async Task GetAllVoucher_LoggedIn_ReturnsVoucherList()
        {
            var user = new AppUser { Id = "user-id-123", UserName = "Shang12345" };
            var storeId = Guid.NewGuid();
            var store = new StoreDetails { ID = storeId, UserID = user.Id };
            var vouchers = new List<Voucher>
            {
                new Voucher
                {
                    ID = Guid.NewGuid(),
                    Code = "SALE10",
                    DiscountType = "Percent",
                    DiscountAmount = 10,
                    MinOrderValue = 50000,
                    ExpirationDate = DateTime.Now.AddDays(5),
                    MaxUsage = 100,
                    CurrentUsage = 10,
                    StoreID = storeId,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    StartDate = DateTime.Now,
                }
            };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailService2Mock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StoreDetails, bool>>>())).ReturnsAsync(store);
            _voucherServiceMock.Setup(v => v.GetAll()).Returns(vouchers.AsQueryable());
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }));
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            var result = await _controller.GetAllVoucher() as JsonResult;
            Assert.IsNotNull(result);
            var list = result.Value as System.Collections.IList;
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0);
        }

        [Test]
        public async Task GetAllVoucher_InvalidStoreId_ReturnsError()
        {
            var user = new AppUser { Id = "user-id-123", UserName = "Shang12345" };
            // Simulate invalid store Guid by returning null for FindAsync
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailService2Mock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StoreDetails, bool>>>())).ReturnsAsync((StoreDetails)null);
            _voucherServiceMock.Setup(v => v.GetAll()).Returns(new List<Voucher>().AsQueryable());
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }));
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            var result = await _controller.GetAllVoucher() as JsonResult;
            Assert.IsNotNull(result);
            // Accept both anonymous object and Dictionary<string, object>
            var errorMsg = result.Value?.GetType().GetProperty("error")?.GetValue(result.Value)?.ToString();
            Assert.IsNotNull(errorMsg);
            Assert.AreEqual("Store not found.", errorMsg);
        }
    }
}
