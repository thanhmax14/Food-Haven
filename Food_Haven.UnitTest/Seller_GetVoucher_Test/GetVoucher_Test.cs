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
using NUnit.Framework;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Seller_GetVoucher_Test
{
    public class GetVoucher_Test
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
        public async Task GetVoucher_ValidId_ReturnsVoucherJson()
        {
            var voucherId = Guid.NewGuid();
            var voucher = new Voucher
            {
                ID = voucherId,
                Code = "SALE10",
                DiscountAmount = 10,
                DiscountType = "Percent",
                StartDate = DateTime.Today,
                ExpirationDate = DateTime.Today.AddDays(5),
                MaxUsage = 100,
                CurrentUsage = 10,
                IsActive = true,
                MaxDiscountAmount = 50,
                MinOrderValue = 50000
            };
            _voucherServiceMock.Setup(v => v.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Voucher, bool>>>())).ReturnsAsync(voucher);
            var result = await _controller.GetVoucher(voucherId);
            Assert.IsInstanceOf<JsonResult>(result);
            var json = (JsonResult)result;
            var dict = json.Value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(json.Value));
            Assert.AreEqual(voucherId, dict["id"]);
            Assert.AreEqual("SALE10", dict["code"]);
            Assert.AreEqual(10, dict["discountAmount"]);
            Assert.AreEqual("Percent", dict["discountType"]);
            Assert.AreEqual(voucher.StartDate.ToString("yyyy-MM-dd"), dict["startDate"]);
            Assert.AreEqual(voucher.ExpirationDate.ToString("yyyy-MM-dd"), dict["expirationDate"]);
            Assert.AreEqual(100, dict["maxUsage"]);
            Assert.AreEqual(10, dict["currentUsage"]);
            Assert.AreEqual(true, dict["isActive"]);
            Assert.AreEqual(50, dict["scope"]);
            Assert.AreEqual(50000, dict["minOrderValue"]);
        }

        [Test]
        public async Task GetVoucher_InvalidId_ReturnsNotFound()
        {
            _voucherServiceMock.Setup(v => v.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Voucher, bool>>>())).ReturnsAsync((Voucher)null);
            var result = await _controller.GetVoucher(Guid.NewGuid());
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
