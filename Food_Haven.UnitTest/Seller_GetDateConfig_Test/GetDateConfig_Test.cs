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
using Newtonsoft.Json.Linq;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Seller_GetDateConfig_Test
{
    public class GetDateConfig_Test
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
        public async Task GetDateConfig_TC01_ReturnsValidDateRange()
        {
            // Arrange
            var user = new AppUser { Id = "user1" };

            _userManagerMock
                .Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var storeId = Guid.NewGuid();
            var store = new StoreDetails { ID = storeId, UserID = user.Id };

            _storeDetailServiceMock
                .Setup(s => s.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync(store);

            var productId = Guid.NewGuid();
            var product = new Product { ID = productId, StoreID = storeId };

            _productServiceMock
                .Setup(p => p.ListAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null))
                .ReturnsAsync(new List<Product> { product });

            var variantId = Guid.NewGuid();
            var variant = new ProductTypes { ID = variantId, ProductID = productId };

            _productVariantServiceMock
                .Setup(v => v.ListAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>(), null, null))
                .ReturnsAsync(new List<ProductTypes> { variant });

            var orderId = Guid.NewGuid();
            var orderDetail = new OrderDetail
            {
                OrderID = orderId,
                ProductTypesID = variantId
            };

            // Define confirmedOrderDate before using it
            DateTime confirmedOrderDate = DateTime.Today;

            var orders = new List<Order>
            {
                new Order
                {
                    ID = orderId,
                    Status = "CONFIRMED",
                    CreatedDate = confirmedOrderDate
                },
                new Order
                {
                    ID = orderId,
                    Status = "CANCELLED BY USER",
                    CreatedDate = DateTime.Today
                }
            };

            _orderDetailServiceMock
                .Setup(o => o.ListAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>(), null, null))
                .ReturnsAsync(new List<OrderDetail> { orderDetail });

            _ordersServiceMock
                .Setup(o => o.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), null, null))
                .ReturnsAsync(orders);

            // Act
            var result = await _controller.GetDateConfig() as JsonResult;
            Assert.IsNotNull(result, "JsonResult is null");
            Assert.IsNotNull(result.Value, "JsonResult.Value is null");

            // Parse the result as JObject for safe property access
            var jObj = JObject.FromObject(result.Value);

            string minDateStr = jObj["minDate"]?.ToString();
            string maxDateStr = jObj["maxDate"]?.ToString();
            int defaultDays = jObj["defaultDays"]?.ToObject<int>() ?? 0;

            Assert.IsNotNull(minDateStr, "minDate is null");
            Assert.IsNotNull(maxDateStr, "maxDate is null");

            DateTime minDate = DateTime.Parse(minDateStr);
            DateTime maxDate = DateTime.Parse(maxDateStr);

            Assert.AreEqual(confirmedOrderDate.ToString("yyyy-MM-dd"), minDateStr);
            Assert.AreEqual(DateTime.Today.ToString("yyyy-MM-dd"), maxDateStr);
            Assert.AreEqual((maxDate - minDate).Days + 1, defaultDays, "defaultDays calculation mismatch");
            Assert.LessOrEqual(defaultDays, 30, "defaultDays exceeds 30");
        }





        [Test]
        public async Task GetDateConfig_TC02_ReturnsInternalServerError()
        {
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.GetDateConfig() as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);

            var jObj = JObject.FromObject(result.Value);
            Assert.AreEqual("Error retrieving date configuration", jObj["error"]?.ToString());
        }


    }
}
