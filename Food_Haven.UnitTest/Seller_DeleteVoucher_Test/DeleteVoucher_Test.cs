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

namespace Food_Haven.UnitTest.Seller_DeleteVoucher_Test
{
    public class DeleteVoucher_Test
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
        public async Task DeleteVoucher_ValidId_ReturnsSuccessJson()
        {
            var voucherId = Guid.NewGuid();
            var voucherEntity = new Voucher { ID = voucherId, IsActive = true };
            var request = new DeleteVoucherRequest { Id = voucherId };
            _voucherServiceMock.Setup(v => v.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Voucher, bool>>>())).ReturnsAsync(voucherEntity);
            _voucherServiceMock.Setup(v => v.UpdateAsync(It.IsAny<Voucher>())).Returns(Task.CompletedTask);
            _voucherServiceMock.Setup(v => v.SaveChangesAsync()).ReturnsAsync(1);
            var result = await _controller.DeleteVoucher(request);
            Assert.IsInstanceOf<JsonResult>(result);
            var json = (JsonResult)result;
            var dict = json.Value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(json.Value));
            Assert.IsTrue(dict.ContainsKey("success"));
            Assert.AreEqual(true, dict["success"]);
            Assert.IsFalse(voucherEntity.IsActive); // Đã bị set false
        }

        [Test]
        public async Task DeleteVoucher_InvalidId_ReturnsNotFound()
        {
            var request = new DeleteVoucherRequest { Id = Guid.NewGuid() };
            _voucherServiceMock.Setup(v => v.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Voucher, bool>>>())).ReturnsAsync((Voucher)null);
            var result = await _controller.DeleteVoucher(request);
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFound = (NotFoundObjectResult)result;
            var dict = notFound.Value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(notFound.Value));
            Assert.IsFalse((bool)dict["success"]);
            Assert.AreEqual("Voucher not found.", dict["error"]);
        }

        [Test]
        public async Task DeleteVoucher_Exception_ReturnsServerError()
        {
            var voucherId = Guid.NewGuid();
            var voucherEntity = new Voucher { ID = voucherId, IsActive = true };
            var request = new DeleteVoucherRequest { Id = voucherId };
            _voucherServiceMock.Setup(v => v.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Voucher, bool>>>())).ReturnsAsync(voucherEntity);
            _voucherServiceMock.Setup(v => v.UpdateAsync(It.IsAny<Voucher>())).Throws(new Exception("Test exception"));
            var result = await _controller.DeleteVoucher(request);
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.AreEqual(500, objectResult.StatusCode);
            var dict = objectResult.Value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(objectResult.Value));
            Assert.IsFalse((bool)dict["success"]);
            Assert.IsTrue(dict["error"].ToString().Contains("Test exception"));
        }
    }
}
